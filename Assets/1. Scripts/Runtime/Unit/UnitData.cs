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
    public int ZeroDEF => unitModel.zeroDEF;
    public int InfDEF => unitModel.infDEF;
    public DamageType attackType => unitModel.attackType;
    public int attack => unitModel.attack;
    public float attackSpeed => unitModel.attackSpeed;
    public int attackRange => unitModel.attackRange;
    public float bulletSpeed => unitModel.bulletSpeed;
    public CalculusResultElement[] derivativeTo => unitModel.derivativeTo;
    public CalculusResultElement[] integralTo => unitModel.integralTo;

    public float unit_c_ATKSPD_buff => unitModel.unit_c_ATKSPD_buff;
    public int unit_c_DMG_buff => unitModel.unit_c_DMG_buff;
    public float unit_x2_damageRatio => unitModel.unit_x2_damageRatio;
    public int unit_x3_attackCount => unitModel.unit_x3_attackCount;
}
