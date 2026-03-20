using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    public static LoadingScene instance;

    private void Awake(){
        if(instance != null){
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text loadingText;
    [SerializeField] private LoadingText[] loadingTextList;

    public void Show(){
        loadingText.text = loadingTextList[UnityEngine.Random.Range(0, loadingTextList.Length)].text;
        root.SetActive(true);
    }
    
    public async UniTask ShowAndLoad(string sceneName, float minDelay = 0f){
        if(GameManager.instance.SKIP_INTENTIONAL_DELAY){
            minDelay = 0f;
        }

        Show();
        //wait for both mindelay and scene load
        if(minDelay <= float.Epsilon){
            await SceneManager.LoadSceneAsync(sceneName).ToUniTask();
        }else{
            await UniTask.WhenAll(
                UniTask.Delay(TimeSpan.FromSeconds(minDelay)),
                SceneManager.LoadSceneAsync(sceneName).ToUniTask()
            );
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        Hide();
    }
    
    public void Hide(){
        root.SetActive(false);
    }
}

[System.Serializable]
public class LoadingText{
    [TextArea] public string text;
}