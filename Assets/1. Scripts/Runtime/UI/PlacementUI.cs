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
    [SerializeField] private Image BossWarningMarkPrefab;
    [SerializeField] private RectTransform InventoryArea;

    public void Show_Placement(){
        gameObject.SetActive(true);
        SetPlacementControlsVisible(true);
        inventory.Setup(GameManager.instance.playerData.units, OnStartDrag_Placement, null);
        UpdateDT();
        GenerateWarningMark(CombatManager.instance.currentWaveChart);
    }

    public void Show_Purchase(){
        gameObject.SetActive(true);
        SetPlacementControlsVisible(true);
        inventory.Setup(GameManager.instance.playerData.units, OnStartDrag_Purchase, null);
        UpdateDT();
    }

    public void ShowWarningMark(WaveModel waveChart){
        gameObject.SetActive(true);
        SetPlacementControlsVisible(false);
        ClearWarningMark();
        GenerateWarningMark(waveChart);
    }

    public void Hide(){
        gameObject.SetActive(false);
        inventory.Clear();
        ClearWarningMark();
    }

    public void GenerateWarningMark(WaveModel waveChart){
        for(int column = 0; column < CombatManager.instance.girdSize.y; column++){
            foreach(EnemyType enemyType in waveChart.commonEnemyTypes[column]){
                if(enemyType == EnemyType.PolarBear){
                    Image warningMark = Instantiate(BossWarningMarkPrefab, WarningMarkRoot[column].transform);
                    warningMark.color = enemyType.GetEnemyModel().color;
                }else{
                    Image warningMark = Instantiate(WarningMarkPrefab, WarningMarkRoot[column].transform);
                    warningMark.color = enemyType.GetEnemyModel().color;
                }
            }
        }
    }

    private void SetPlacementControlsVisible(bool isVisible){
        inventory.gameObject.SetActive(isVisible);
        if(DT_Text != null){
            DT_Text.transform.parent.gameObject.SetActive(isVisible);
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
