using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlacementUI : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private TMP_Text DT_Text;
    [SerializeField] private GameObject[] WarningMarkRoot;
    [SerializeField] private RectTransform InventoryArea;

    public void Show_Placement(){
        gameObject.SetActive(true);
        inventory.Setup(GameManager.instance.playerData.units, OnStartDrag_Placement);
        UpdateDT();
    }

    public void Show_Purchase(){
        gameObject.SetActive(true);
        inventory.Setup(GameManager.instance.playerData.units, OnStartDrag_Purchase);
        UpdateDT();
    }

    public void Hide(){
        gameObject.SetActive(false);
        inventory.Clear();
    }

    public void UpdateUnit(){
        inventory.UpdateUnit(GameManager.instance.playerData.units);
    }

    public void UpdateDT(){
        DT_Text.text = GameManager.instance.playerData.DT.ToString();
    }

    public void OnStartDrag_Placement(UnitIcon icon, UnitStatus status){
        CombatManager.instance.StartDrag(icon, status);
    }

    public void OnStartDrag_Purchase(UnitIcon icon, UnitStatus status){
        //CombatManager.instance.StartDrag(icon, status);
    }

    public bool IsInInventoryArea(Vector3 mousePosition){
        return RectTransformUtility.RectangleContainsScreenPoint(InventoryArea, mousePosition);
    }
}
