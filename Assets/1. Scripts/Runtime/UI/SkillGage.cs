using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillGage : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Animator[] skillButtons;

    private float gage;
    private bool preivous = false;

    public void Show(){
        gameObject.SetActive(true);
    }

    public void Hide(){
        gameObject.SetActive(false);
    }

    public void SetGage(float value){
        gage = Mathf.Clamp(value, 0, GameManager.instance.commonSettings.maxSkillGage);
        fillImage.fillAmount = gage / GameManager.instance.commonSettings.maxSkillGage;

        if(preivous == false && gage >= GameManager.instance.commonSettings.skillCost){ 
            foreach(var btn in skillButtons)
            {
                btn.ResetTrigger("Pressed");
                btn.ResetTrigger("Selected");
                btn.ResetTrigger("Highlighted");
                btn.gameObject.SetActive(true);
            }
            preivous = true;
        }
        if(preivous == true && gage < GameManager.instance.commonSettings.skillCost){
            foreach(var btn in skillButtons)
            {
                if(EventSystem.current.currentSelectedGameObject == btn.gameObject)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                }
                btn.gameObject.SetActive(false);
            }
            preivous = false;
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
