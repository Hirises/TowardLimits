using UnityEngine;

/// <summary>
/// 빌보드 오브젝트
/// 스프라이트가 카메라를 향하도록 (x축만) 돌려줌
/// </summary>
public class Billboard : MonoBehaviour
{
    private void Start(){
        transform.rotation = Quaternion.Euler(Camera.main.transform.rotation.eulerAngles.x, 
            transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
    }
}
