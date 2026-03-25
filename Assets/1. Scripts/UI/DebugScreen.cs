using System;
using System.IO;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

public class DebugScreen : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text override_text;

    private void Awake(){
        root.SetActive(false);
    }
    
    private void Update(){
        if(Input.GetKeyDown(KeyCode.F1)){
            UpdateInfo();
            root.SetActive(!root.activeSelf);
        }
    }

    private void UpdateInfo(){
        UpdateOverrideText();
    }

    private void UpdateOverrideText(){
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Override Info");
        sb.Append($"Enemy ({DataFetcher.enemyData.Values.Where(x => x.isOverriden).Count()}): ");
        foreach(EnemyModel enemyModel in DataFetcher.enemyData.Values){
            if(enemyModel.isOverriden){
                sb.Append($"{enemyModel.enemyType}, ");
            }
        }
        sb.AppendLine();
        sb.Append($"Unit ({DataFetcher.unitData.Values.Where(x => x.isOverriden).Count()}): ");
        foreach(UnitModel unitModel in DataFetcher.unitData.Values){
            if(unitModel.isOverriden){
                sb.Append($"{unitModel.unitType}, ");
            }
        }
        sb.AppendLine();
        sb.AppendLine($"Stage ({DataFetcher.stageData.Where(x => x.isOverriden).Count()})");
        sb.AppendLine($"Wave ({DataFetcher.waveData.Where(x => x.isOverriden).Count()})");
        override_text.text = sb.ToString();
    }

    public void OpenOverrideFolder(){
        string filePath = Path.Combine(Application.persistentDataPath, "OverrideData");
        if(!Directory.Exists(filePath)){
            Directory.CreateDirectory(filePath);
        }
        Application.OpenURL(filePath);
    }

    public void RefreshOverrideData(){
        DataFetcher.FetchData();
        UpdateInfo();
    }
}