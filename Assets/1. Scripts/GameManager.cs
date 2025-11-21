using NaughtyAttributes;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;

/// <summary>
/// 전체 씬 홀더
/// </summary>
public class GameManager : MonoBehaviour
{
    //싱글톤
    public static GameManager instance;

    [Header("Debug")]
    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private UnitType[] unitMap;

    public PlayerData playerData;

    private void Awake(){
        if(instance != null){
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        playerData = new PlayerData();
        playerData.units = new UnitType[gridSize.x][];
        for(int k = 0; k < gridSize.x; k++){
            playerData.units[k] = new UnitType[gridSize.y];
        }
        int i = 0;
        int j = 0;
        foreach(UnitType unitType in unitMap){
            playerData.units[i][j] = unitType;
            j++;
            if(j >= gridSize.y){
                i++;
                j = 0;
            }
        }
    }
}
