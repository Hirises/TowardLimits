using System;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class EnemyModel : ICloneable
{
    [Header("Basic")]
    public EnemyType enemyType;
    public EnemyBehavior enemyBehavior;
    [NonSerialized] public bool isOverriden = false;

    [Header("Stats")]
    [SerializeField] public int health;
    [SerializeField] public bool rangeAttack;
    [SerializeField] public float attackSpeed;
    [SerializeField] public int damage;
    [SerializeField] public int persuade;
    [SerializeField] public float speed;
    [SerializeField] public Color color;

    [Header("Level up")]
    [SerializeField] public int health_add;
    [SerializeField] public int damage_add;
    [SerializeField] public float speed_add;

    public int GetHealth(){
        return health + health_add * GameManager.instance.playerData.stage;
    }

    public int GetDamage(){
        return damage + damage_add * GameManager.instance.playerData.stage;
    }

    public float GetSpeed(){
        return speed + speed_add * GameManager.instance.playerData.stage;
    }

    public object Clone(){
        return MemberwiseClone();
    }
}