using UnityEngine;
using NaughtyAttributes;
using RotaryHeart.Lib.SerializableDictionary;
using System.Collections.Generic;

/// <summary>
/// 인게임 메니저
/// </summary>
public class CombatManager : MonoBehaviour
{
    public static CombatManager instance;

    [Header("Grid")]
    [Label("그리드 크기 Row, Column")]
    [SerializeField] public Vector2Int girdSize;
    [Label("슬롯 (좌상단에서 시작. hor->ver)")]
    [SerializeField] public Slot[] slots;

    [Header("Entities")]
    [SerializeField] public SerializableDictionaryBase<EnemyType, EnemyBehavior> enemyMap;
    [SerializeField] public SerializableDictionaryBase<UnitType, UnitBehavior> unitMap;

    private List<EnemyBehavior> enemiesList = new List<EnemyBehavior>();

    private void Awake(){
        instance = this;
    }

    private void Start(){
        SetupGame();
        StartGame();
    }

    /// <summary>
    /// 게임을 종료하고 자원을 반환합니다.
    /// </summary>
    public void EndGame(){
        foreach(Slot slot in slots){
            if(slot.unit != null){
                Destroy(slot.unit.gameObject);
            }
            slot.unit = null;
        }
        foreach(EnemyBehavior enemy in enemiesList){
            Destroy(enemy.gameObject);
        }
        enemiesList.Clear();
    }

    /// <summary>
    /// 게임을 초기화하고 준비합니다 (이미 모든 자원은 반환되어있는 빈 상태라고 가정)
    /// </summary>
    public void SetupGame(){
        PlayerData playerData = GameManager.instance.playerData;

        //배치자료대로 유닛 생성
        foreach(Slot slot in slots){
            UnitType unitType = playerData.units[slot.position.x][slot.position.y];
            if(unitType == UnitType.None){
                continue;
            }
            
            UnitBehavior unit = Instantiate(unitMap[unitType], slot.transform);
            slot.unit = unit;
        }
    }

    /// <summary>
    /// 게임을 시작합니다
    /// </summary>
    public void StartGame(){
        foreach(Slot slot in slots){
            if(slot.unit != null){
                slot.unit.OnSummon();
            }
        }
    }

    public Slot GetSlot(int row, int column){
        return slots[row * girdSize.y + column];
    }
}
