using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class UnitModel : ICloneable
{
    [Header("Basic")]
    public UnitType unitType;
    public string unitName;
    [TextArea] public string unitDescription;
    public Color unitColor = Color.white;
    [NonSerialized] public bool isOverriden = false;

    [Header("Images")]
    public Sprite fullFront;
    public Sprite fullBack;
    public Sprite holdMotion;
    public Sprite chibiIcon;
    public UnitBehavior unitBehavior;

    [Header("Stats")]
    public int maxHealth = 100;
    public int zeroDEF = 0;
    public int infDEF = 0;
    public DamageType attackType = DamageType.None;
    public int attack = 10;
    public float attackSpeed = 1;
    public int attackRange = 100;
    public float bulletSpeed = 15;

    [Header("Calculus")]
    public CalculusResultElement[] derivativeTo;
    public CalculusResultElement[] integralTo;

    [Header("UnitC")]
    [FormerlySerializedAs("ATKSPD_buff")]
    public float unit_c_ATKSPD_buff = 0;
    [FormerlySerializedAs("DMG_buff")]
    public int unit_c_DMG_buff = 0;
    [FormerlySerializedAs("buffDuration")]
    public float unit_c_buffDuration = 3;

    [Header("UnitX2")]
    [FormerlySerializedAs("damageRatio")]
    public float unit_x2_damageRatio = 0.5f;
    [FormerlySerializedAs("skillDamage")]
    [Min(0)] public int unit_x2_skillDamage = 40;
    [FormerlySerializedAs("skillBulletSpeed")]
    [Min(0.01f)] public float unit_x2_skillBulletSpeed = 15;

    [Header("UnitX3")]
    [FormerlySerializedAs("attackCount")]
    public int unit_x3_attackCount = 3;

    [Header("UnitABS")]
    [FormerlySerializedAs("healAmount")]
    public int unit_abs_healAmount = 10;

    public object Clone()
    {
        return MemberwiseClone();
    }
}

[System.Serializable]
public class CalculusResultElement{
    public UnitType unitType;
    [Range(0f, 1f)]
    public float probability = 1f;
    public int amount = 1;
}
