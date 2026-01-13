using TMPro;
using UnityEngine;

public class DamageVFX : MonoBehaviour
{
    [SerializeField]
    private TMP_Text damageText;
    [SerializeField]
    private Animator animator;

    public void Show(int damage){
        damageText.text = damage.ToString();
        gameObject.SetActive(true);
        animator.Play("DamageVFX");
    }
}
