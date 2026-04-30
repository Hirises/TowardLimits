using System;
using UnityEngine;

//에디터 상에서만 위치 표기용으로 쓰고 런타임에는 지워버릴 용도
public class EditortimeMarker : MonoBehaviour
{
    private void Awake(){
        Destroy(gameObject);
    }
}