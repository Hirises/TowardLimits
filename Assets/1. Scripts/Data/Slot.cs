using System.Diagnostics.CodeAnalysis;
using Sirenix.OdinInspector;
using UnityEngine;

public class Slot : MonoBehaviour
{
    [SerializeField] public Vector2Int position;
    [SerializeField] public GameObject slotEmpty;
    [SerializeField] public GameObject slotOccupied;
    [SerializeField] public GameObject UnitPoint;
    [ReadOnly] public UnitBehavior unit;

    public int DMG_buff = 0;
    public float ATKSPD_buff = 0;
    public float RANGE_buff = 0;
    public float BULLETSPD_buff = 0;

    private bool showBase = false;

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
        UpdateBaseActiveState();
    }

    public void ShowBase(bool show){
        showBase = show;
        UpdateBaseActiveState();
    }

    private void UpdateBaseActiveState(){
        bool isEmpty = unit == null;
        slotOccupied.SetActive(!isEmpty && showBase);
        slotEmpty.SetActive(isEmpty && showBase);
    }
}
