using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 에디터용 스크립트
/// </summary>
[ExecuteInEditMode]
public class RelavtiveLineHandler : MonoBehaviour
{
    public static RelavtiveLineHandler instance;

    [Header("Manager Ref")]
    [SerializeField] private CombatManager combatManager;

    [Header("Line Ref")]
    [SerializeField] private Transform[] Columns;
    [SerializeField] private Transform[] Rows;
    [SerializeField] private Transform TopRow;
    public float TopRowZ => TopRow.position.z;
    [SerializeField] private Transform BottomRow;
    public float BottomRowZ => BottomRow.position.z;

    private void Awake(){
        instance = this;
    }

#if UNITY_EDITOR
    private void Update(){
        for(int i = 0; i < Columns.Length; i++){
            for(int j = 0; j < Rows.Length; j++){
                combatManager.GetSlot(j, i).transform.position = new Vector3(Columns[i].position.x, 0, Rows[j].position.z);
            }
        }
    }

    private void OnDrawGizmos(){
        for(int i = 0; i < Columns.Length; i++){
            Gizmos.color = Color.red;
            Gizmos.DrawLine(new Vector3(Columns[i].position.x, transform.position.y, TopRowZ), 
            new Vector3(Columns[i].position.x, transform.position.y, BottomRowZ));
        }
        for(int i = 0; i < Rows.Length; i++){
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(new Vector3(-10, transform.position.y, Rows[i].position.z),
             new Vector3(10, transform.position.y, Rows[i].position.z));
        }
    }
#endif

}
