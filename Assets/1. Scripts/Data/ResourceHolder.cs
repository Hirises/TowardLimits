using UnityEngine;

[SerializeField]
[CreateAssetMenu(fileName = "ResourceHolder.asset", menuName = "Unique/Resource Holder")]
public class ResourceHolder : ScriptableObject
{
    private static ResourceHolder instance;
    public static ResourceHolder Instance
    {
        get {
            if(instance == null){
                instance = Resources.Load<ResourceHolder>("ResourceHolder");
            }
            return instance;
        }
    }
    
    [Header("VFX")]
    public DamageVFX damageVFXPrefab;

    [Header("Skill")]
    public float skillCoolTime = 15f;
}
