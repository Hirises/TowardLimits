using TMPro;
using UnityEngine;

public class HealVFX : MonoBehaviour
{
    [SerializeField]
    private TMP_Text healText;

    public void Show(int heal){
        healText.text = heal.ToString();
        gameObject.SetActive(true);
    }

    public void Destroy(){
        GameObject.Destroy(gameObject);
    }
}
