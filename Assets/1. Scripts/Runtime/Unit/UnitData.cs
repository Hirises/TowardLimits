using UnityEngine;
using Sirenix.OdinInspector;
using System;

[CreateAssetMenu(fileName = "UnitData.asset", menuName = "Unit Data")]
public class UnitData : ScriptableObject
{
    [InlineProperty, HideLabel] public UnitModel unitModel;


    public string unitName => unitModel.unitName;
    public string unitDescription => unitModel.unitDescription;
    public Color unitColor => unitModel.unitColor;
    public int maxHealth => unitModel.maxHealth;
    public int attack => unitModel.attack;
    public float attackSpeed => unitModel.attackSpeed;
    public int attackRange => unitModel.attackRange;
    public float bulletSpeed => unitModel.bulletSpeed;
    public CalculusResultElement[] derivativeTo => unitModel.derivativeTo;
    public CalculusResultElement[] integralTo => unitModel.integralTo;

    public float ATKSPD_buff => unitModel.ATKSPD_buff;
    public int DMG_buff => unitModel.DMG_buff;
    public float damageRatio => unitModel.damageRatio;
    public int attackCount => unitModel.attackCount;
}
