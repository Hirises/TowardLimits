using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGhost : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color placementColor = new Color(0.4f, 1f, 0.4f, 0.65f);
    [SerializeField] private Color swapColor = new Color(0.4f, 1f, 1f, 0.65f);
    [SerializeField] private Color blockedColor = new Color(1f, 0.4f, 0.4f, 0.65f);

    public void SetPlacementState(bool canPlace, bool isSwap){
        spriteRenderer.color = !canPlace ? blockedColor : isSwap ? swapColor : placementColor;
    }

    public void Setup(UnitData unit)
    {
        if(unit != null && unit.unitModel != null){
            spriteRenderer.sprite = unit.unitModel.fullBack;
            gameObject.SetActive(true);
        }
    }

    public void Setup(UnitStatus status)
    {
        if(status != null){
            Setup(status.model);
        }
    }

    public void Setup(UnitModel model)
    {
        if(model != null && model.fullBack != null){
            spriteRenderer.sprite = model.fullBack;
            gameObject.SetActive(true);
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
