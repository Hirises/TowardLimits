using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using NaughtyAttributes;

public class UnitIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private Image iconBorder;

    [ReadOnly] public int index;

    public void Setup(UnitData status, int index){
        this.index = index;
        icon.sprite = status.chibiIcon;
        iconBorder.color = status.unitColor;
    }

    public void OnDrag(PointerEventData eventData)
    {
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {

    }
}
