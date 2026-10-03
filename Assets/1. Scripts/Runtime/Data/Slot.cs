using System.Diagnostics.CodeAnalysis;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// 유닛을 배치 가능한 슬롯
/// </summary>
public class Slot : MonoBehaviour
{
    [SerializeField] public Vector2Int position;
    [SerializeField] public GameObject slotEmpty;
    [SerializeField] public GameObject slotOccupied;
    [SerializeField] public GameObject UnitPoint;
    [SerializeField] private BoxCollider placementCollider;
    [ReadOnly] public UnitBehavior unit;
    public bool isEmpty => unit == null;

    [Header("Buff")]
    //버프 값 (유닛 스텟에 합연산으로 계산)
    public int DMG_buff = 0;
    public float ATKSPD_buff = 0;
    public float RANGE_buff = 0;
    public float BULLETSPD_buff = 0;

    private bool showBase = false;
    private readonly Vector2[] screenCorners = new Vector2[4];

    public bool ContainsScreenPoint(Camera camera, Vector2 point){
        Vector3 center = placementCollider.center;
        Vector3 halfSize = placementCollider.size * 0.5f;
        for(int i = 0; i < 4; i++){
            Vector3 corner = center + new Vector3(i == 0 || i == 3 ? -halfSize.x : halfSize.x, 0, i < 2 ? -halfSize.z : halfSize.z);
            Vector3 projected = camera.WorldToScreenPoint(placementCollider.transform.TransformPoint(corner));
            if(projected.z <= 0) return false;
            screenCorners[i] = projected;
        }
        bool hasPositive = false;
        bool hasNegative = false;
        for(int i = 0; i < 4; i++){
            Vector2 edge = screenCorners[(i + 1) % 4] - screenCorners[i];
            Vector2 offset = point - screenCorners[i];
            float cross = edge.x * offset.y - edge.y * offset.x;
            hasPositive |= cross > 0.001f;
            hasNegative |= cross < -0.001f;
        }
        return !(hasPositive && hasNegative) && (hasPositive || hasNegative);
    }

    public void ResetBuff(){
        DMG_buff = 0;
        ATKSPD_buff = 0;
        RANGE_buff = 0;
        BULLETSPD_buff = 0;
    }

    /// <summary>
    /// <see cref="UnitBehavior.OnPlacement"/>에서 호출됩니다. 직접 부르지 마세요
    /// </summary>
    /// <param name="unit"></param>
    internal void SetUnit_Internal([MaybeNull] UnitBehavior unit){
        this.unit = unit;
        UpdateSprite();
    }

    public void ShowBase(bool show){
        showBase = show;
        UpdateSprite();
    }

    private void UpdateSprite(){
        bool isEmpty = unit == null;
        slotOccupied.SetActive(!isEmpty && showBase);
        slotEmpty.SetActive(isEmpty && showBase);
    }
}
