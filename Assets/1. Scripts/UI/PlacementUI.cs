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
}
