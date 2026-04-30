using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 라인 관리자
/// 에디터 상에서 다양한 라인을 시각적으로 확인하기 위해 사용함
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
    [SerializeField] public Transform RowBase;
    [SerializeField] private Transform TopRow;
    public float TopRowZ => TopRow.position.z;
    [SerializeField] private Transform BottomRow;
    public float BottomRowZ => BottomRow.position.z;
    [SerializeField] private Transform MiddleRow;
    public float MiddleRowZ => MiddleRow.position.z;

    [Header("Deco Lines")]
    [SerializeField] private Transform[] LeftDecoLines;
    [SerializeField] private Transform[] RightDecoLines;

    public float ColumnX(int column) => Columns[column].position.x;

    private void Awake(){
        instance = this;
    }

    private void OnDestroy(){
        instance = null;
    }

    public Transform RandomDecoLine(){
        if(UnityEngine.Random.Range(0, 2) == 0){
            return LeftDecoLines[UnityEngine.Random.Range(0, LeftDecoLines.Length)];
        } else {
            return RightDecoLines[UnityEngine.Random.Range(0, RightDecoLines.Length)];
        }
    }

#if UNITY_EDITOR
    private void Update(){
        for(int i = 0; i < Columns.Length; i++){
            for(int j = 0; j < Rows.Length; j++){
                combatManager.GetSlotAt(j, i).transform.position = new Vector3(Columns[i].position.x, 0.01f, Rows[j].position.z);
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

        //Middle Row
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(-10, transform.position.y, MiddleRowZ),
        new Vector3(10, transform.position.y, MiddleRowZ));
    }
#endif

}
