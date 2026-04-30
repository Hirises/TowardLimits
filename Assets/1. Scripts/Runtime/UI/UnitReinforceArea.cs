using System;
using Unity.VisualScripting;
using UnityEngine;

public class UnitReinforceArea : MonoBehaviour
{
    [SerializeField] private RectTransform area;
    [SerializeField] private Inventory inventoryUI;
    [SerializeField] private UnitInfoPopup unitInfoPopup;

    private bool isDragging = false;
    private Action endDrag;

    public void StartDrag(UnitIcon icon, UnitStatus status){
        isDragging = true;
        icon.SetAlpha(0.5f);
        endDrag = () => {
            icon.SetAlpha(1f);
            if(RectTransformUtility.RectangleContainsScreenPoint(area, Input.mousePosition)){
                if(status.unitType != UnitType.UnitC){
                    return;
                }
                GameManager.instance.playerData.units.Remove(status);
                unitInfoPopup.FixedUnit.level++;
                inventoryUI.UpdateUnit(GameManager.instance.playerData.units);
                unitInfoPopup.UpdateInfo();
            }
        };
    }

    private void Update(){
        if(Input.GetMouseButtonUp(0)){
            if(isDragging){
                endDrag?.Invoke();
                isDragging = false;
            }
        }
    }

}