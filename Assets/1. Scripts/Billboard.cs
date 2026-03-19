using UnityEngine;

/// <summary>
/// 빌보드 오브젝트
/// </summary>
public class Billboard : MonoBehaviour
{
    private void Awake(){
        transform.LookAt(Camera.main.transform);
    }
}
