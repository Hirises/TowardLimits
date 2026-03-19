using TMPro;
using UnityEngine;

public class DamageVFX : MonoBehaviour
{
    [SerializeField]
    private TMP_Text damageText;

    public void Show(int damage){
        damageText.text = damage.ToString();
        gameObject.SetActive(true);
    }

    public void Destroy(){
        GameObject.Destroy(gameObject);
    }
}
