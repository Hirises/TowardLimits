using TMPro;
using UnityEngine;

public class AttackSkillCountVFX : MonoBehaviour
{
    [SerializeField]
    private TMP_Text countText;

    public void Show(int count){
        countText.text = count.ToString();
        gameObject.SetActive(true);
    }

    public void Destroy(){
        GameObject.Destroy(gameObject);
    }
}
