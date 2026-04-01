using UnityEngine;

/// <summary>
/// 빌보드 오브젝트
/// </summary>
public class Billboard : MonoBehaviour
{
    private void Start(){
        transform.rotation = Quaternion.Euler(Camera.main.transform.rotation.eulerAngles.x, 
            transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
    }
}
