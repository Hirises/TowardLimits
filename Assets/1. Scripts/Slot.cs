using UnityEngine;

public class Slot : MonoBehaviour
{
    [SerializeField] public Vector2Int position;
    [SerializeField] public GameObject slotBase;
    public UnitBehavior unit;

    public int DMG_buff = 0;
    public float ATKSPD_buff = 0;
    public float RANGE_buff = 0;
    public float BULLETSPD_buff = 0;

    public void ResetBuff(){
        DMG_buff = 0;
        ATKSPD_buff = 0;
        RANGE_buff = 0;
        BULLETSPD_buff = 0;
    }

    public void ShowBase(bool show){
        slotBase.SetActive(show);
    }
}
