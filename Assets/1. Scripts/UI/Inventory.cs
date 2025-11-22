using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private GameObject iconRoot;
    [SerializeField] private UnitIcon iconPrefab;
    [SerializeField] private int iconCount;

    private List<UnitStatus> statuses;
    private int page;

    public void Setup(List<UnitStatus> units){
        page = 0;
        UpdateUnit(units);
    }

    public void UpdateUnit(List<UnitStatus> units){
        statuses = units;
        UpdatePage();
    }

    public void UpdatePage(){
        Clear();
        for(int i = iconCount * page; i < iconCount * (page + 1); i++){
            if(i >= statuses.Count){
                break;
            }
            UnitIcon icon = Instantiate(iconPrefab, iconRoot.transform);
            icon.Setup(statuses[i].unitType.GetUnitData(), i);
            i++;
        }
    }

    public void NextPage(){
        if(page >= Mathf.CeilToInt(statuses.Count / iconCount)){
            return;
        }
        page++;
        UpdatePage();
    }

    public void PrevPage(){
        if(page <= 0){
            return;
        }
        page--;
        UpdatePage();
    }

    public void Clear(){
        foreach(Transform child in iconRoot.transform){
            Destroy(child.gameObject);
        }
    }
}
