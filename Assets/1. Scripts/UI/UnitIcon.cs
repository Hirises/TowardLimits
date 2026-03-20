using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using UnityEngine.Events;

public class UnitIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private Image iconBorder;
    [SerializeField] private CanvasGroup canvasGroup;

    [ReadOnly] public int index;
    
    private UnityAction<int> onPointerEnter;
    private UnityAction<int> onPointerExit;
    private UnityAction<int, bool> onDrag;
    private UnityAction<int> onClick;
    public void ReigsterEvents(UnityAction<int> onPointerEnter, UnityAction<int> onPointerExit, UnityAction<int, bool> onDrag, UnityAction<int> onClick){
        this.onPointerEnter = onPointerEnter;
        this.onPointerExit = onPointerExit;
        this.onDrag = onDrag;
        this.onClick = onClick;
    }

    public void Setup(UnitData status, int index){
        this.index = index;
        icon.sprite = status.chibiIcon;
        iconBorder.color = status.unitColor;
        SetAlpha(1f);
    }

    public void SetAlpha(float alpha){
        canvasGroup.alpha = alpha;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onPointerExit?.Invoke(index);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onPointerEnter?.Invoke(index);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        onDrag?.Invoke(index, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        onDrag?.Invoke(index, false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke(index);
    }
}
