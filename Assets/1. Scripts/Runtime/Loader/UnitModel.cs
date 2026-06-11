using System;
using Sirenix.OdinInspector;
using UnityEngine;

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
    public float ATKSPD_buff = 0;
    public int DMG_buff = 0;
    public float buffDuration = 3;

    [Header("UnitX2")]
    public float damageRatio = 0.5f;

    [Header("UnitX3")]
    public int attackCount = 3;

    [Header("UnitABS")]
    public int healAmount = 10;

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