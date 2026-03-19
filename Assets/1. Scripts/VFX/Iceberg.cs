using UnityEngine;

/// <summary>
/// 빙하 이미지
/// </summary>
public class Iceberg : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private Vector2 speedRange;

    private float speed;

    private void Awake(){
        spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];
        speed = Random.Range(speedRange.x, speedRange.y);
    }

    private void Update(){
        if(CombatManager.instance.phase == CombatManager.Phase.Combat){
            transform.position -= Vector3.forward * speed * Time.deltaTime;
        }
        if(transform.position.z <= RelavtiveLineHandler.instance.BottomRowZ){
            Destroy(gameObject);
        }
    }
}
