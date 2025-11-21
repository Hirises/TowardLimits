using NaughtyAttributes;
using UnityEngine;

/// <summary>
/// 전체 씬 홀더
/// </summary>
public class GameManager : MonoBehaviour
{
    //싱글톤
    public static GameManager instance;

    private void Awake(){
        if(instance != null){
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
