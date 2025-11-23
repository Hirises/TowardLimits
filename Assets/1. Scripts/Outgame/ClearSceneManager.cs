using Unity.VisualScripting;
using UnityEngine;

public class ClearSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject SuccessPanel;
    [SerializeField] private GameObject FailurePanel;

    private bool isSuccess = false;

    private void Start(){
        Debug.Log("ClearSceneManager Start");
        if(GameManager.instance.playerData.Prove >= 100){
            isSuccess = true;
            CutsceneManager.instance.PlayCutScene("Clear");
        }else{
            isSuccess = false;
            CutsceneManager.instance.PlayCutScene("Fail");
        }

        SuccessPanel.SetActive(isSuccess);
        FailurePanel.SetActive(!isSuccess);
    }

    private void Update(){
        if(Input.anyKeyDown || Input.GetMouseButtonDown(0)){
            if(!CutsceneManager.instance.isPlaying){
                LoadingScene.instance.ShowAndLoad("Title", GameManager.instance.MIN_LOADING_DELAY);
            }
        }
    }
}
