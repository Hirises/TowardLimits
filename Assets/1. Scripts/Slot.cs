using UnityEngine;

public class Slot : MonoBehaviour
{
    [SerializeField] public Vector2Int position;
    [SerializeField] public GameObject slotBase;
    public UnitBehavior unit;

    public void ShowBase(bool show){
        slotBase.SetActive(show);
    }
}
