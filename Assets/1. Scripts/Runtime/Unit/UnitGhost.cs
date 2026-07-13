using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGhost : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

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