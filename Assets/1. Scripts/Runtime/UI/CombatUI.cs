using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatUI : MonoBehaviour
{
    [SerializeField] private TMP_Text DT_Text;
    [SerializeField] private Button startButton;
    [SerializeField] private Toggle gameSpeedToggle;
    [SerializeField] private GameObject[] WarningMarkRoot;
    [SerializeField] private Image WarningMarkPrefab;
    [SerializeField] private Image BossWarningMarkPrefab;

    public void Show_Placement(){
        GenerateWarningMark(CombatManager.instance.currentWaveChart);
        startButton.gameObject.SetActive(true);
        gameSpeedToggle.gameObject.SetActive(false);
    }

    public void Show_Combat()
    {
        ClearWarningMark();
        startButton.gameObject.SetActive(false);
        gameSpeedToggle.gameObject.SetActive(true);
    }

    public void Show_Purchase()
    {
        ClearWarningMark();
        startButton.gameObject.SetActive(true);
        gameSpeedToggle.gameObject.SetActive(false);
    }

    public void ClearWarningMark(){
        foreach(GameObject child in WarningMarkRoot){
            foreach(Transform inst in child.transform){
                Destroy(inst.gameObject);
            }
        }
    }

    public void ShowWarningMark(WaveModel waveChart){
        gameObject.SetActive(true);
        ClearWarningMark();
        GenerateWarningMark(waveChart);
    }

    public void GenerateWarningMark(WaveModel waveChart){
        for(int column = 0; column < CombatManager.instance.girdSize.y; column++){
            foreach(EnemyType enemyType in waveChart.commonEnemyTypes[column]){
                if(enemyType == EnemyType.PolarBear){
                    Image warningMark = Instantiate(BossWarningMarkPrefab, WarningMarkRoot[column].transform);
                    warningMark.color = enemyType.GetEnemyModel().color;
                }else{
                    Image warningMark = Instantiate(WarningMarkPrefab, WarningMarkRoot[column].transform);
                    warningMark.color = enemyType.GetEnemyModel().color;
                }
            }
        }
    }

    public void UpdateDT(){
        DT_Text.text = GameManager.instance.playerData.DT.ToString();
    }

    public void OnToggleGameSpeed(bool isOn)
    {
        GameManager.instance.SetGameSpeed(isOn ? 2f : 1f);
    }
}