using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager instance;

    [Header("UI")]
    [SerializeField] private GameObject cutsceneRoot;
    [SerializeField] private Image leftImage;
    [SerializeField] private Image rightImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text text;

    [Header("CutScene Datas")]
    [SerializeField] private List<CutSceneData> cutSceneDatas;

    private IEnumerator<CutSceneAction> currentAction;
    public bool isPlaying = false;
    public float originGameSpeed;
    public bool isWaiting = false;
    public CancellationTokenSource playingToken;

    private void Awake(){
        if(instance != null){
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public async UniTask PlayCutSceneAndWait(string cutSceneName)
    {
        PlayCutScene(cutSceneName);
        if(!isPlaying) return;
        await UniTask.WaitUntilCanceled(playingToken.Token);
    }

    public async UniTask WaitUntilEnd()
    {
        if(!isPlaying) return;
        await UniTask.WaitUntilCanceled(playingToken.Token);
    }

    public void PlayCutScene(string cutSceneName){
        CutSceneData cutSceneData = cutSceneDatas.Find(data => data.cutSceneName.Equals(cutSceneName));
        Debug.Log("TryPlayCutScene: " + cutSceneName);
        if(cutSceneData != null){
            Debug.Log("PlayCutScene: " + cutSceneName);
            PlayCutScene(cutSceneData);
        }
    }

    public void PlayCutScene(CutSceneData cutSceneData){
        currentAction = cutSceneData.GetEnumerator();
        cutsceneRoot.SetActive(true);
        isPlaying = true;
        isWaiting = false;
        originGameSpeed = Time.timeScale;
        GameManager.instance.SetGameSpeed(0f);
        playingToken = new CancellationTokenSource();

        if(GameManager.instance.SKIP_CUTSCENE){
            while(isPlaying){
                NextAction(force: true);
            }
            return;
        }
        
        NextAction();
    }

    private void Update(){
        if(currentAction != null){
            if((Input.anyKeyDown || Input.GetMouseButtonDown(0)) && !LoadingScene.instance.isLoading){
                NextAction();
            }
        }
    }

    private void NextAction(bool force = false){
        if(isWaiting && !force){
            return;
        }
        if(currentAction.MoveNext()){
            Run(currentAction.Current);
        }else{
            GameManager.instance.SetGameSpeed(originGameSpeed);
            currentAction = null;
            cutsceneRoot.SetActive(false);
            isPlaying = false;
            if(playingToken != null)
            {
                playingToken.Cancel();
                playingToken.Dispose();
                playingToken = null;
            }
        }
    }

    private void Run(CutSceneAction action){
        if(!string.IsNullOrEmpty(action.name)) nameText.text = action.name;
        text.text = action.text;
        if(action.sprite != null){
            if(action.isLeft){
                leftImage.sprite = action.sprite;
                rightImage.sprite = null;
            }else{
                leftImage.sprite = null;
                rightImage.sprite = action.sprite;
            }
            leftImage.gameObject.SetActive(action.isLeft);
            rightImage.gameObject.SetActive(!action.isLeft);
        }
        if(action.clearSprite){
            leftImage.sprite = null;
            rightImage.sprite = null;
            leftImage.gameObject.SetActive(false);
            rightImage.gameObject.SetActive(false);
        }
        if(action.callFunction){
            GameObject gameObject = GameObject.Find(action.clazz);
            if(gameObject != null){
                gameObject.SendMessage(action.method);
            }
            NextAction();
        }
    }

    //컷씬중에서 임의로 정지할때 사용
    public void WaitUntil()
    {
        isWaiting = true;
        GameManager.instance.SetGameSpeed(1f);
    }

    //컷씬중에서 임의로 정지할때 사용
    public void WaitUntilAndHide()
    {
        WaitUntil();
        cutsceneRoot.SetActive(false);
    }

    public void Continue()
    {
        if(!isWaiting) return;
        cutsceneRoot.SetActive(true);
        isWaiting = false;
        GameManager.instance.SetGameSpeed(0f);
        NextAction();
    }
}
