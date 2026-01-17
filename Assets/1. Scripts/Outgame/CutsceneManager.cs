using System;
using System.Collections.Generic;
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

    private void Awake(){
        if(instance != null){
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void PlayCutScene(string cutSceneName){
        CutSceneData cutSceneData = cutSceneDatas.Find(data => data.cutSceneName.Equals(cutSceneName));
        Debug.Log("PlayCutScene: " + cutSceneName);
        if(cutSceneData != null){
            Debug.Log("PlayCutScene111: " + cutSceneName);
            PlayCutScene(cutSceneData);
        }
    }

    public void PlayCutScene(CutSceneData cutSceneData){
        currentAction = cutSceneData.GetEnumerator();
        cutsceneRoot.SetActive(true);
        isPlaying = true;

        if(GameManager.instance.SKIP_CUTSCENE){
            while(isPlaying){
                NextAction();
            }
            return;
        }
        
        NextAction();
    }

    private void Update(){
        if(currentAction != null){
            if(Input.anyKeyDown || Input.GetMouseButtonDown(0)){
                NextAction();
            }
        }
    }

    private void NextAction(){
        if(currentAction.MoveNext()){
            Run(currentAction.Current);
        }else{
            currentAction = null;
            cutsceneRoot.SetActive(false);
            isPlaying = false;
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
}
