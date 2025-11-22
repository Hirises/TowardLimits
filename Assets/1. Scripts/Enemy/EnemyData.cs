using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData.asset", menuName = "Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Basic")]
    public EnemyType enemyType;
    public int health;
    public int damage;
    public int persuade;
    public float speed;
}
