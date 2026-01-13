using UnityEngine;

public abstract class LivingEntity : MonoBehaviour, IDamageable
{
    public abstract void TakeDamage(int damage);
}
