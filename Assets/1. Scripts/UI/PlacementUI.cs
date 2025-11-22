using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlacementUI : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private TMP_Text DT_Text;
    [SerializeField] private GameObject[] WarningMarkRoot;

    public void Show(){
        gameObject.SetActive(true);
        inventory.Setup(GameManager.instance.playerData.units);
        SetDT(GameManager.instance.playerData.DT);
    }

    public void Hide(){
        gameObject.SetActive(false);
        inventory.Clear();
    }

    public void SetUnits(List<UnitStatus> units){
        inventory.UpdateUnit(units);
    }

    public void SetDT(int dt){
        DT_Text.text = dt.ToString();
    }
}
