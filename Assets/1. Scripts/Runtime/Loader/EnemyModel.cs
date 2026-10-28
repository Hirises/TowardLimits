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
    [SerializeField] public int zeroDEF;
    [SerializeField] public int infDEF;
    [SerializeField] public bool rangeAttack;
    [SerializeField] public bool stopAtMiddle;
    [SerializeField] public bool attackWhileMoving;
    [SerializeField] public float attackSpeed;
    [SerializeField] public int damage;
    [SerializeField] public int rangeDamage;
    [SerializeField] public int persuade;
    [SerializeField] public float speed;
    [SerializeField] public float bulletSpeed;
    [SerializeField] public Color color;

    [Header("PolarBear")]
    public EnemyType enemy_polarbear_snowballType = EnemyType.Snowball;

    [Header("CaptainPolarBear")]
    [Min(0)] public float enemy_captainpolarbear_throwInterval = 1f;
    [Min(0)] public int enemy_captainpolarbear_throwDamage = 50;
    [Min(0)] public int enemy_captainpolarbear_throwCount = 1;

    [Header("Level up")]
    [SerializeField] public int health_add;
    [SerializeField] public int damage_add;
    [SerializeField] public int rangeDamage_add;
    [SerializeField] public float speed_add;

    public int GetHealth(){
        return health + health_add * GameManager.instance.playerData.stage;
    }

    public int GetDamage(){
        return damage + damage_add * GameManager.instance.playerData.stage;
    }

    public int GetRangeDamage(){
        return rangeDamage + rangeDamage_add * GameManager.instance.playerData.stage;
    }

    public float GetSpeed(){
        return speed + speed_add * GameManager.instance.playerData.stage;
    }

    public object Clone(){
        return MemberwiseClone();
    }
}
