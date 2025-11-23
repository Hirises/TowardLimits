using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseCampManager : MonoBehaviour
{
    public static BaseCampManager instance;

    [SerializeField] private Inventory inventoryUI;
    [SerializeField] private TMP_Text stageText;
    [SerializeField] private string stageTextFormat = "Stage {0}";
    [SerializeField] private TMP_Text persuadedText;
    [SerializeField] private TMP_Text proveText;
    private void Awake(){
        instance = this;
    }

    private void Start(){
        Setup();
    }

    public void Setup(){
        stageText.text = string.Format(stageTextFormat, GameManager.instance.playerData.stage);
        persuadedText.text = $"{GameManager.instance.playerData.Persuaded}%";
        proveText.text = $"{GameManager.instance.playerData.Prove}%";
        inventoryUI.Setup(GameManager.instance.playerData.units, (icon, status) => {
            //no drag action -> pass
        });
    }

    public void TowardNorth(){
        StartGame(Polar.North);
    }

    public void TowardSouth(){
        StartGame(Polar.South);
    }

    public void StartGame(Polar direction){
        GameManager.instance.playerData.direction = direction;
        StageData[] stageDatas = GameManager.instance.stageDatas.Where(stage => stage.stageNumber.x <= GameManager.instance.playerData.stage 
            && stage.stageNumber.y >= GameManager.instance.playerData.stage
            && (stage.direction == direction || stage.direction == Polar.Both)).ToArray();
        GameManager.instance.currentStage = stageDatas[Random.Range(0, stageDatas.Length)];
        LoadingScene.instance.ShowAndLoad("Game", GameManager.instance.MIN_LOADING_DELAY);
    }
}
