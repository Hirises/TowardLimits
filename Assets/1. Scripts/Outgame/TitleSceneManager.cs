using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneManager : MonoBehaviour
{
    void Update()
    {
        if(Input.anyKeyDown || Input.GetMouseButtonDown(0)){
            StartNewGame();
        }
    }

    public void StartNewGame(){
        GameManager.instance.playerData = new PlayerData();
        LoadingScene.instance.ShowAndLoad("BaseCamp", GameManager.instance.MIN_LOADING_DELAY);
    }
}
