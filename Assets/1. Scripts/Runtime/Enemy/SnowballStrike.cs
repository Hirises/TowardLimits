using UnityEngine;

public class SnowballStrike : MonoBehaviour
{
    [SerializeField] private Transform snowball;
    [SerializeField] private SpriteRenderer shadow;
    [SerializeField] private GameObject fragmentsRoot;
    [SerializeField] private SpriteRenderer[] fragments;
    [SerializeField, Min(0.01f)] private float fallDuration = 1f;
    [SerializeField, Min(0)] private float fallHeight = 3f;
    [SerializeField, Min(0)] private float impactHeight = 0.6f;
    [SerializeField, Range(0.01f, 1f)] private float initialShadowScale = 0.2f;
    [SerializeField, Min(0.01f)] private float impactDuration = 0.25f;
    [SerializeField, Min(0)] private float fragmentDistance = 0.8f;

    private Slot targetSlot;
    private int damage;
    private float elapsedTime;
    private Vector3 shadowScale;
    private Color shadowColor;
    private bool isStarted;
    private bool hasLanded;

    public void StartStrike(Slot slot, int damage){
        targetSlot = slot;
        this.damage = damage;
        transform.position = slot.UnitPoint.transform.position;
        elapsedTime = 0;
        hasLanded = false;
        shadowScale = shadow.transform.localScale;
        shadowColor = shadow.color;
        fragmentsRoot.SetActive(false);
        UpdateFall(0);
        isStarted = true;
    }

    public void Cancel(){
        isStarted = false;
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    private void Update(){
        if(!isStarted) return;
        if(targetSlot == null){
            Cancel();
            return;
        }

        elapsedTime += Time.deltaTime;
        if(!hasLanded){
            float progress = Mathf.Clamp01(elapsedTime / fallDuration);
            UpdateFall(progress);
            if(progress >= 1f){
                Land();
            }
        }else{
            float progress = Mathf.Clamp01(elapsedTime / impactDuration);
            for(int i = 0; i < fragments.Length; i++){
                float angle = 2f * Mathf.PI * i / fragments.Length;
                Vector3 direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
                fragments[i].transform.localPosition = direction * (fragmentDistance * progress)
                    + Vector3.up * (impactHeight + 0.4f * Mathf.Sin(progress * Mathf.PI));
                Color color = fragments[i].color;
                color.a = 1f - progress;
                fragments[i].color = color;
            }
            if(progress >= 1f){
                Destroy(gameObject);
            }
        }
    }

    private void UpdateFall(float progress){
        snowball.localPosition = Vector3.up * (impactHeight + fallHeight * (1f - progress * progress));
        shadow.transform.localScale = shadowScale * Mathf.Lerp(initialShadowScale, 1f, progress);
        Color color = shadowColor;
        color.a *= Mathf.Lerp(0.5f, 1f, progress);
        shadow.color = color;
    }

    private void Land(){
        hasLanded = true;
        elapsedTime = 0;
        snowball.gameObject.SetActive(false);
        shadow.gameObject.SetActive(false);
        fragmentsRoot.SetActive(true);
        foreach(SpriteRenderer fragment in fragments){
            fragment.transform.localPosition = Vector3.up * impactHeight;
        }
        if(targetSlot.unit != null){
            targetSlot.unit.TakeDamage(damage, DamageType.None);
        }
    }
}
