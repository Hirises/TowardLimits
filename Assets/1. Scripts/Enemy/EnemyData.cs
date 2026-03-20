using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData.asset", menuName = "Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Basic")]
    public EnemyType enemyType;
    public EnemyBehavior enemyBehavior;

    [Title("Model")]
    [InlineProperty, HideLabel] public EnemyModel enemyModel;

    public int health => enemyModel.health;
    public int damage => enemyModel.damage;
    public int persuade => enemyModel.persuade;
    public float speed => enemyModel.speed;
    public Color color => enemyModel.color;
    public int health_add => enemyModel.health_add;
    public int damage_add => enemyModel.damage_add;
    public float speed_add => enemyModel.speed_add;

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
