using System.Threading.Tasks;
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
        loadingText.text = loadingTextList[Random.Range(0, loadingTextList.Length)].text;
        root.SetActive(true);
    }

    public async void ShowAndLoad(string sceneName, float minDelay = 0f){
        Show();
        //wait for both mindelay and scene load
        await Task.WhenAll(
            Task.Delay((int)(minDelay * 1000)),
            Task.FromResult(SceneManager.LoadSceneAsync(sceneName))
        );
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