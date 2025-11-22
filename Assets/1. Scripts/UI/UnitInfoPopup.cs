using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitInfoPopup : MonoBehaviour
{
    public static UnitInfoPopup instance;

    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Image fullImage;
    [SerializeField] private TMP_Text ATK_Text;
    [SerializeField] private TMP_Text HP_Text;
    [SerializeField] private TMP_Text ATS_Text;
    [SerializeField] private TMP_Text Level_Text;
    [SerializeField] private TMP_Text[] Colored_Text;

    private void Awake(){
        if(instance != null){
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Show(UnitStatus status, float worldY = 0){
        root.transform.position = new Vector3(root.transform.position.x, worldY, root.transform.position.z);
        root.SetActive(true);
        nameText.text = status.data.unitName;
        descriptionText.text = status.data.unitDescription;
        fullImage.sprite = status.data.fullFront;
        ATK_Text.text = status.data.attack.ToString();
        HP_Text.text = status.data.maxHealth.ToString();
        ATS_Text.text = status.data.attackSpeed.ToString();
        Level_Text.text = status.level.ToString();
        for(int i = 0; i < Colored_Text.Length; i++){
            Colored_Text[i].color = status.data.unitColor;
        }
    }

    public void Hide(){
        root.SetActive(false);
    }
}
