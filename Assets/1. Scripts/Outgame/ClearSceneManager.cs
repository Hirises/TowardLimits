using UnityEngine;

public class ClearSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject SuccessPanel;
    [SerializeField] private GameObject FailurePanel;

    private bool isSuccess = false;

    private void Start(){
        if(GameManager.instance.playerData.Prove >= 100){
            isSuccess = true;
        }else{
            isSuccess = false;
        }

        SuccessPanel.SetActive(isSuccess);
        FailurePanel.SetActive(!isSuccess);
    }
}
