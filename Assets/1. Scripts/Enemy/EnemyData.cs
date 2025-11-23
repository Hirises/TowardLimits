using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData.asset", menuName = "Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Basic")]
    public EnemyType enemyType;
    public EnemyBehavior enemyBehavior;
    public int health;
    public int damage;
    public int persuade;
    public float speed;
    public Color color;

    [Header("Level up")]
    public int health_add;
    public int damage_add;
    public float speed_add;

    public int GetHealth(){
        return health + health_add * GameManager.instance.playerData.stage;
    }

    public int GetDamage(){
        return damage + damage_add * GameManager.instance.playerData.stage;
    }

    public float GetSpeed(){
        return speed + speed_add * GameManager.instance.playerData.stage;
    }
}
