using UnityEngine;
using NaughtyAttributes;

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

    [Header("Enemies")]
    [SerializeField] public EnemyBehavior[] enemies;

    [Header("Units")]
    [SerializeField] public UnitBehavior[] units;

    private void Awake(){
        instance = this;
    }

    private void Start(){
        ResetGame();
        StartGame();
    }

    public void ResetGame(){

    }

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
