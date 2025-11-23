using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneManager : MonoBehaviour
{

    [SerializeField] private GameObject space;
    private bool startCutScene = false;

    private void Start(){
        startCutScene = false;
    }

    void Update()
    {
        if(Input.anyKeyDown || Input.GetMouseButtonDown(0)){
            if(!CutsceneManager.instance.isPlaying && !startCutScene){
                startCutScene = true;
                CutsceneManager.instance.PlayCutScene("GameStart");
            }
        }
    }

    public void ShowSpace(){
        space.SetActive(true);
    }

    public void HideSpace(){
        space.SetActive(false);
    }

    public void StartNewGame(){
        startCutScene = false;
        GameManager.instance.playerData = new PlayerData();
        LoadingScene.instance.ShowAndLoad("BaseCamp", GameManager.instance.MIN_LOADING_DELAY);
    }
}
