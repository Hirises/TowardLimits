using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private GameObject iconRoot;
    [SerializeField] private UnitIcon iconPrefab;
    [SerializeField] private int iconCount;

    private UnitIcon[] icons;
    private List<UnitStatus> statuses;
    private int page;
    private Action<UnitIcon, UnitStatus> onStartDrag;

    public void Setup(List<UnitStatus> units, Action<UnitIcon, UnitStatus> onStartDrag){
        this.onStartDrag = onStartDrag;
        page = 0;
        UpdateUnit(units);
    }

    public void UpdateUnit(List<UnitStatus> units){
        statuses = units;
        UpdatePage();
    }

    public void UpdatePage(){
        Clear();
        icons = new UnitIcon[iconCount];
        for(int i = iconCount * page; i < iconCount * (page + 1); i++){
            if(i >= statuses.Count){
                break;
            }
            UnitIcon icon = Instantiate(iconPrefab, iconRoot.transform);
            icons[i] = icon;
            icon.Setup(statuses[i].unitType.GetUnitData(), i);
            icon.ReigsterEvents(OnIconHoverUp, OnIconHoverDown, OnIconDrag);
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

    public void OnIconHoverUp(int index){
        UnitInfoPopup.instance.Show(statuses[index],
          icons[index].transform.position.y - 40);
    }

    public void OnIconHoverDown(int index){
        UnitInfoPopup.instance.Hide();
    }

    public void OnIconDrag(int index, bool beginDrag){
        //pass
        Debug.Log("OnIconDrag: " + index + " " + beginDrag);
        if(beginDrag){
            onStartDrag?.Invoke(icons[index], statuses[index]);
        }
    }
}
