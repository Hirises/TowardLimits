using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlacementUI : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private TMP_Text DT_Text;
    [SerializeField] private GameObject[] WarningMarkRoot;
    [SerializeField] private Image WarningMarkPrefab;
    [SerializeField] private RectTransform InventoryArea;

    public void Show_Placement(){
        gameObject.SetActive(true);
        inventory.Setup(GameManager.instance.playerData.units, OnStartDrag_Placement);
        UpdateDT();
        GenerateWarningMark();
    }

    public void Show_Purchase(){
        gameObject.SetActive(true);
        inventory.Setup(GameManager.instance.playerData.units, OnStartDrag_Purchase);
        UpdateDT();
    }

    public void Hide(){
        gameObject.SetActive(false);
        inventory.Clear();
        ClearWarningMark();
    }

    public void GenerateWarningMark(){
        for(int column = 0; column < CombatManager.instance.girdSize.y; column++){
            foreach(EnemyType enemyType in CombatManager.instance.currentWaveChart.commonEnemyTypes[column]){
                Image warningMark = Instantiate(WarningMarkPrefab, WarningMarkRoot[column].transform);
                warningMark.color = enemyType.GetEnemyData().color;
            }
        }
    }

    public void ClearWarningMark(){
        foreach(GameObject child in WarningMarkRoot){
            foreach(Transform inst in child.transform){
                Destroy(inst.gameObject);
            }
        }
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
        CombatManager.instance.StartDrag_Purchase(icon, status);
    }

    public bool IsInInventoryArea(Vector3 mousePosition){
        return RectTransformUtility.RectangleContainsScreenPoint(InventoryArea, mousePosition);
    }
}
