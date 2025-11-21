using UnityEngine;
using NaughtyAttributes;

/// <summary>
/// 인게임 메니저
/// </summary>
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    [Label("그리드 크기 Column, Row")]
    [SerializeField] public Vector2Int girdSize;
    [Label("슬롯 (좌상단에서 시작. hor->ver)")]
    [SerializeField] public Slot[] slots;

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

    public Slot GetSlot(int column, int row){
        return slots[column * girdSize.y + row];
    }
}
