using UnityEngine;

public interface IDamageable
{
    public void TakeDamage(int damage, DamageType type);
    public void Heal(int amount);
}
