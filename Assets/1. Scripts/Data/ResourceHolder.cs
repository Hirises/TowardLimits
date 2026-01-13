using UnityEngine;

[SerializeField]
[CreateAssetMenu(fileName = "ResourceHolder.asset", menuName = "Unique/Resource Holder")]
public class ResourceHolder : ScriptableObject
{
    [Header("VFX")]
    public DamageVFX damageVFX;
}
