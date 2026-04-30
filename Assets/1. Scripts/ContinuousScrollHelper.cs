using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 베이스 스프라이트를 그리드로 복제하여 최소 영역을 채우고, Scroll으로 무한 스크롤을 구성합니다.
/// 스크롤 델타는 이 트랜스폼의 로컬 XY 평면 기준입니다.
/// 부모(이 컴포넌트) 회전·스케일을 반영해 타일 간격은 스프라이트 로컬 축을 따라 잡습니다.
/// minSize 한 축을 0 이하로 두면 그 축으로는 복제하지 않고 그리드 인덱스 0 한 줄만 사용합니다(양 축 0이면 타일 1개).
/// </summary>
[ExecuteAlways]
public class ContinuousScrollHelper : MonoBehaviour
{
    /// <summary>그리드당 생성 가능한 타일 수 상한. 초과 시 생성하지 않고 에러 로그만 남깁니다.</summary>
    private const int MaxTileInstances = 8192;

    private const float MinLatticeDeterminant = 1e-10f;

    /// <summary>헬퍼 로컬에서 덮을 최소 크기. 한 축을 0 이하로 두면 그 축 방향으로는 타일을 복제하지 않고 인덱스 0 한 줄만 둡니다.</summary>
    [SerializeField] private Vector2 minSize = new Vector2(10f, 10f);
    [SerializeField] private GameObject baseSpriteObject;
    [SerializeField] private Vector2 scrollOffset;

    private readonly Dictionary<Vector2Int, GameObject> _tiles = new Dictionary<Vector2Int, GameObject>();

    /// <summary>헬퍼 로컬에서 (gx+1, gy) 타일 중심까지의 변위.</summary>
    private Vector3 _stepLocalX;

    /// <summary>헬퍼 로컬에서 (gx, gy+1) 타일 중심까지의 변위.</summary>
    private Vector3 _stepLocalY;

    private Vector3 _zeroCenterLocal;

    /// <summary>베이스 타일 중심 기준 헬퍼 로컬 XY 발자국 반extent.</summary>
    private Vector2 _tileFootprintHalfLocal;

    private Vector3 _lastStepLocalX;
    private Vector3 _lastStepLocalY;
    private Vector3 _lastMetricZero;
    private Vector2 _lastFootprintHalfLocal;
    private bool _haveMetricBaseline;

#if UNITY_EDITOR
    private bool _scheduledRebuild;
#endif

    private Vector3 ScrollOffsetLocal3 => new Vector3(scrollOffset.x, scrollOffset.y, 0f);

    private void OnEnable()
    {
        ApplySerializedSettings(forceRespawnAllTiles: true);
    }

    private void OnDestroy()
    {
        ClearTiles();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying)
            return;
            // ApplySerializedSettings(forceRespawnAllTiles: false);
        else
            ScheduleEditorRebuild();
    }

    private void ScheduleEditorRebuild()
    {
        if (_scheduledRebuild)
            return;
        _scheduledRebuild = true;
        EditorApplication.delayCall += EditorRebuildCallback;
    }

    private void EditorRebuildCallback()
    {
        _scheduledRebuild = false;
        if (this == null)
            return;
        ApplySerializedSettings(forceRespawnAllTiles: false);
    }
#endif

    /// <summary>
    /// 로컬 XY 기준으로 모든 타일을 이동하고, minSize 영역 밖으로 나간 타일은 제거한 뒤 필요한 타일을 생성합니다.
    /// </summary>
    public void Scroll(Vector2 deltaLocal)
    {
        scrollOffset += deltaLocal;
        ApplySerializedSettings(forceRespawnAllTiles: false);
    }

    /// <summary>
    /// 설정값과 베이스 스프라이트를 기준으로 타일을 처음부터 다시 채웁니다. scrollOffset은 유지합니다.
    /// </summary>
    public void Rebuild()
    {
        ApplySerializedSettings(forceRespawnAllTiles: true);
    }

    private void ApplySerializedSettings(bool forceRespawnAllTiles)
    {
        DestroyOrphanTileChildrenNotTracked();

        if (!TryComputeGridMetrics(out _stepLocalX, out _stepLocalY, out _zeroCenterLocal, out _tileFootprintHalfLocal))
        {
            _haveMetricBaseline = false;
            ClearTiles();
            return;
        }

        bool metricsChanged = !_haveMetricBaseline ||
            Vector3.Distance(_stepLocalX, _lastStepLocalX) > 1e-4f ||
            Vector3.Distance(_stepLocalY, _lastStepLocalY) > 1e-4f ||
            Vector3.Distance(_zeroCenterLocal, _lastMetricZero) > 1e-4f ||
            Vector2.Distance(_tileFootprintHalfLocal, _lastFootprintHalfLocal) > 1e-4f;

        _lastStepLocalX = _stepLocalX;
        _lastStepLocalY = _stepLocalY;
        _lastMetricZero = _zeroCenterLocal;
        _lastFootprintHalfLocal = _tileFootprintHalfLocal;
        _haveMetricBaseline = true;

        if (forceRespawnAllTiles || metricsChanged || _tiles.Count == 0)
        {
            ClearTiles();
            if (!TryComputeNeededCoords(out var coords))
                return;
            foreach (var coord in coords)
                SpawnTile(coord);
        }
        else
        {
            RefreshTiles();
        }
    }

    private void RefreshTiles()
    {
        DestroyOrphanTileChildrenNotTracked();

        if (_stepLocalX.sqrMagnitude <= Mathf.Epsilon || _stepLocalY.sqrMagnitude <= Mathf.Epsilon)
            return;

        if (!TryComputeNeededCoords(out var needed))
        {
            ClearTiles();
            return;
        }

        var toRemove = new List<Vector2Int>();
        foreach (var kv in _tiles)
        {
            if (!needed.Contains(kv.Key))
                toRemove.Add(kv.Key);
        }

        foreach (var key in toRemove)
        {
            DestroySafe(_tiles[key]);
            _tiles.Remove(key);
        }

        foreach (var coord in needed)
        {
            if (!_tiles.TryGetValue(coord, out var go) || go == null)
            {
                if (go != null)
                    _tiles.Remove(coord);
                SpawnTile(coord);
            }
            else
            {
                ApplyTilePosition(go, coord);
            }
        }
    }

    /// <summary>
    /// 필요한 그리드 좌표 집합을 계산합니다. 타일 수가 <see cref="MaxTileInstances"/>를 넘거나 값이 비정상이면 false입니다.
    /// </summary>
    private bool TryComputeNeededCoords(out HashSet<Vector2Int> coords)
    {
        coords = new HashSet<Vector2Int>();

        if (!float.IsFinite(minSize.x) || !float.IsFinite(minSize.y))
            return false;
        if (minSize.x < 0f || minSize.y < 0f)
            return false;
        if (!float.IsFinite(scrollOffset.x) || !float.IsFinite(scrollOffset.y))
            return false;

        bool collapseX = minSize.x <= 0f;
        bool collapseY = minSize.y <= 0f;

        if (collapseX && collapseY)
        {
            coords.Add(Vector2Int.zero);
            return true;
        }

        float halfW = collapseX ? 0f : minSize.x * 0.5f;
        float halfH = collapseY ? 0f : minSize.y * 0.5f;
        float hx = _tileFootprintHalfLocal.x;
        float hy = _tileFootprintHalfLocal.y;
        float zx = _zeroCenterLocal.x;
        float zy = _zeroCenterLocal.y;
        if (!float.IsFinite(zx) || !float.IsFinite(zy))
            return false;

        Vector2 sx = new Vector2(_stepLocalX.x, _stepLocalX.y);
        Vector2 sy = new Vector2(_stepLocalY.x, _stepLocalY.y);
        float det = sx.x * sy.y - sx.y * sy.x;
        if (!float.IsFinite(det) || Mathf.Abs(det) < MinLatticeDeterminant)
        {
            Debug.LogError(
                $"ContinuousScrollHelper '{gameObject.name}': 타일 스텝 벡터가 XY 평면에서 기저를 이루지 않습니다(det≈{det}). 스프라이트 축 설정을 확인하세요.",
                this);
            return false;
        }

        Vector2 origin = new Vector2(zx + scrollOffset.x, zy + scrollOffset.y);

        float rx0 = -halfW - hx;
        float rx1 = halfW + hx;
        float ry0 = -halfH - hy;
        float ry1 = halfH + hy;

        Vector2[] rectCorners =
        {
            new Vector2(rx0, ry0),
            new Vector2(rx1, ry0),
            new Vector2(rx1, ry1),
            new Vector2(rx0, ry1),
        };

        float gxMinF = float.PositiveInfinity;
        float gxMaxF = float.NegativeInfinity;
        float gyMinF = float.PositiveInfinity;
        float gyMaxF = float.NegativeInfinity;

        foreach (Vector2 rc in rectCorners)
        {
            Vector2 d = rc - origin;
            float gx = (d.x * sy.y - d.y * sy.x) / det;
            float gy = (sx.x * d.y - sx.y * d.x) / det;
            if (!float.IsFinite(gx) || !float.IsFinite(gy))
                return false;
            if (!collapseX)
            {
                gxMinF = Mathf.Min(gxMinF, gx);
                gxMaxF = Mathf.Max(gxMaxF, gx);
            }

            if (!collapseY)
            {
                gyMinF = Mathf.Min(gyMinF, gy);
                gyMaxF = Mathf.Max(gyMaxF, gy);
            }
        }

        int gxMin;
        int gxMax;
        int gyMin;
        int gyMax;

        if (collapseX)
        {
            gxMin = gxMax = 0;
        }
        else
        {
            gxMin = Mathf.FloorToInt(gxMinF) - 1;
            gxMax = Mathf.CeilToInt(gxMaxF) + 1;
        }

        if (collapseY)
        {
            gyMin = gyMax = 0;
        }
        else
        {
            gyMin = Mathf.FloorToInt(gyMinF) - 1;
            gyMax = Mathf.CeilToInt(gyMaxF) + 1;
        }

        long nx = (long)gxMax - gxMin + 1;
        long ny = (long)gyMax - gyMin + 1;
        if (nx <= 0 || ny <= 0)
            return false;

        if (nx > MaxTileInstances || ny > MaxTileInstances || nx * ny > MaxTileInstances)
        {
            Debug.LogError(
                $"ContinuousScrollHelper '{gameObject.name}': 타일 수({nx * ny:N0})가 상한 {MaxTileInstances}을 초과했습니다. " +
                $"minSize={minSize}, scrollOffset={scrollOffset}. 셀 간격이 너무 작거나 minSize가 너무 큽니다.",
                this);
            return false;
        }

        for (int gx = gxMin; gx <= gxMax; gx++)
        {
            for (int gy = gyMin; gy <= gyMax; gy++)
                coords.Add(new Vector2Int(gx, gy));
        }

        return true;
    }

    private bool TryComputeGridMetrics(
        out Vector3 stepLocalX,
        out Vector3 stepLocalY,
        out Vector3 zeroCenterLocal,
        out Vector2 tileFootprintHalfLocal)
    {
        stepLocalX = stepLocalY = zeroCenterLocal = Vector3.zero;
        tileFootprintHalfLocal = Vector2.zero;

        if (baseSpriteObject == null)
            return false;

        var sr = baseSpriteObject.GetComponent<SpriteRenderer>();
        if (sr == null || sr.sprite == null)
            return false;

        Bounds wb = sr.bounds;
        zeroCenterLocal = transform.InverseTransformPoint(wb.center);

        if (!float.IsFinite(zeroCenterLocal.x) || !float.IsFinite(zeroCenterLocal.y) || !float.IsFinite(zeroCenterLocal.z))
            return false;

        Bounds spriteLocalBounds = sr.sprite.bounds;
        Vector3 spriteSize = spriteLocalBounds.size;
        if (!float.IsFinite(spriteSize.x) || !float.IsFinite(spriteSize.y) || !float.IsFinite(spriteSize.z))
            return false;

        Transform st = sr.transform;
        Vector3 stepWorldX = st.TransformVector(new Vector3(spriteSize.x, 0f, 0f));
        Vector3 stepWorldY = st.TransformVector(new Vector3(0f, spriteSize.y, 0f));

        stepLocalX = transform.InverseTransformVector(stepWorldX);
        stepLocalY = transform.InverseTransformVector(stepWorldY);

        if (!Vec3AllFinite(stepLocalX) || !Vec3AllFinite(stepLocalY))
            return false;

        if (stepLocalX.sqrMagnitude <= Mathf.Epsilon || stepLocalY.sqrMagnitude <= Mathf.Epsilon)
            return false;

        tileFootprintHalfLocal = ComputeTileFootprintHalfExtentsLocal(sr, zeroCenterLocal);

        return float.IsFinite(tileFootprintHalfLocal.x) && float.IsFinite(tileFootprintHalfLocal.y);
    }

    private Vector2 ComputeTileFootprintHalfExtentsLocal(SpriteRenderer sr, Vector3 zeroCenterLocal)
    {
        Bounds lb = sr.sprite.bounds;
        Vector3 ext = lb.extents;
        Transform st = sr.transform;
        float maxDx = 0f;
        float maxDy = 0f;

        for (int ix = -1; ix <= 1; ix += 2)
        {
            for (int iy = -1; iy <= 1; iy += 2)
            {
                for (int iz = -1; iz <= 1; iz += 2)
                {
                    Vector3 cornerSpriteSpace = lb.center + new Vector3(ix * ext.x, iy * ext.y, iz * ext.z);
                    Vector3 world = st.TransformPoint(cornerSpriteSpace);
                    Vector3 hl = transform.InverseTransformPoint(world);
                    maxDx = Mathf.Max(maxDx, Mathf.Abs(hl.x - zeroCenterLocal.x));
                    maxDy = Mathf.Max(maxDy, Mathf.Abs(hl.y - zeroCenterLocal.y));
                }
            }
        }

        return new Vector2(maxDx, maxDy);
    }

    private static bool Vec3AllFinite(Vector3 v)
    {
        return float.IsFinite(v.x) && float.IsFinite(v.y) && float.IsFinite(v.z);
    }

    private void SpawnTile(Vector2Int coord)
    {
        GameObject go = Instantiate(baseSpriteObject, transform);
        go.name = $"{baseSpriteObject.name}_{coord.x}_{coord.y}";
        go.SetActive(true);
        ApplyTilePosition(go, coord);
        _tiles[coord] = go;
    }

    private void ApplyTilePosition(GameObject tile, Vector2Int coord)
    {
        var csr = tile.GetComponent<SpriteRenderer>();
        if (csr == null)
            return;

        Vector3 targetLocal = _zeroCenterLocal + ScrollOffsetLocal3 + coord.x * _stepLocalX + coord.y * _stepLocalY;
        Vector3 targetCenterWorld = transform.TransformPoint(targetLocal);

        Vector3 delta = targetCenterWorld - csr.bounds.center;
        tile.transform.position += delta;
    }

    private void ClearTiles()
    {
        DestroyOrphanTileChildrenNotTracked();

        foreach (var kv in _tiles)
            DestroySafe(kv.Value);
        _tiles.Clear();
    }

    /// <summary>
    /// 리컴파일 등으로 <see cref="_tiles"/>가 비워져도 씬에 남은 타일 클론을 제거합니다.
    /// 직계 자식 중 <see cref="baseSpriteObject"/>와 현재 딕셔너리에 있는 오브젝트만 둡니다.
    /// </summary>
    private void DestroyOrphanTileChildrenNotTracked()
    {
        var staleKeys = new List<Vector2Int>();
        foreach (var kv in _tiles)
        {
            if (kv.Value == null)
                staleKeys.Add(kv.Key);
        }

        foreach (Vector2Int k in staleKeys)
            _tiles.Remove(k);

        var keep = new HashSet<GameObject>();
        if (baseSpriteObject != null)
            keep.Add(baseSpriteObject);

        foreach (var kv in _tiles)
        {
            if (kv.Value != null)
                keep.Add(kv.Value);
        }

        Transform self = transform;
        for (int i = self.childCount - 1; i >= 0; i--)
        {
            GameObject child = self.GetChild(i).gameObject;
            if (keep.Contains(child))
                continue;
            DestroySafe(child);
        }
    }

    private static void DestroySafe(Object obj)
    {
        if (obj == null)
            return;
#if UNITY_EDITOR
        if (!Application.isPlaying)
            DestroyImmediate(obj);
        else
#endif
            Destroy(obj);
    }
}
