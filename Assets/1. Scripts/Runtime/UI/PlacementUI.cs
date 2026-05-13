using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlacementUI : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private RectTransform InventoryArea;

    public void Show_Placement(){
        gameObject.SetActive(true);
        SetPlacementControlsVisible(true);
        inventory.Setup(GameManager.instance.playerData.units, OnStartDrag_Placement, null);
    }

    public void Show_Purchase(){
        gameObject.SetActive(true);
        SetPlacementControlsVisible(true);
        inventory.Setup(GameManager.instance.playerData.units, OnStartDrag_Purchase, null);
    }

    public void Hide(){
        gameObject.SetActive(false);
        inventory.Clear();
    }

    private void SetPlacementControlsVisible(bool isVisible){
        inventory.gameObject.SetActive(isVisible);
    }

    public void UpdateUnit(){
        inventory.UpdateUnit(GameManager.instance.playerData.units);
    }

    public void OnStartDrag_Placement(UnitIcon icon, UnitStatus status){
        CombatManager.instance.StartDrag(icon, status);
    }

    public void OnStartDrag_Purchase(UnitIcon icon, UnitStatus status){
        CombatManager.instance.StartDrag_Purchase(icon, status);
    }

    public bool IsInInventoryArea(Vector3 mousePosition){
        return RectTransformUtility.RectangleContainsScreenPoint(InventoryArea, mousePosition);
    }
}
