using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillGage : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Animator[] skillButtons;

    private float gage;

    public void Show(){
        SetGage(gage);
        gameObject.SetActive(true);
    }

    public void Hide(){
        gameObject.SetActive(false);
    }

    public void SetGage(float value){
        gage = Mathf.Clamp(value, 0, GameManager.instance.commonSettings.maxSkillGage);
        fillImage.fillAmount = gage / GameManager.instance.commonSettings.maxSkillGage;

        foreach(var btn in skillButtons)
        {
            btn.gameObject.SetActive(gage >= GameManager.instance.commonSettings.skillCost);
        }
    }

    public void UpdateGage(){
        SetGage(gage + GameManager.instance.commonSettings.skillGagePerSecond * Time.deltaTime);
    }

    //외부 연결
    public void TryPerformSkill(int line){
        if(gage >= GameManager.instance.commonSettings.skillCost){
            SetGage(gage - GameManager.instance.commonSettings.skillCost);
            ForcePerformSkill(line);
        }
    }

    public void ForcePerformSkill(int line){
        CombatManager.instance.ForcePerformSkill(line);
    }
}
