using UnityEngine;
using UnityEngine.UI;

public class SkillGage : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private GameObject skillButtons;

    private float gage;

    public void Show(){
        gameObject.SetActive(true);
    }

    public void Hide(){
        gameObject.SetActive(false);
    }

    public void SetGage(float vale){
        gage = Mathf.Clamp(vale, 0, GameManager.instance.commonSettings.maxSkillGage);
        fillImage.fillAmount = gage / GameManager.instance.commonSettings.maxSkillGage;

        if(gage >= GameManager.instance.commonSettings.skillCost){
            skillButtons.SetActive(true);
        }else{
            skillButtons.SetActive(false);
        }
    }

    public void UpdateGage(){
        SetGage(gage + GameManager.instance.commonSettings.skillGagePerSecond * Time.deltaTime);
    }

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
