using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "UnitData.asset", menuName = "Unit Data")]
public class UnitData : ScriptableObject
{
    [Header("Basic")]
    public UnitType unitType;
    public string unitName;
    [TextArea] public string unitDescription;
    public Color unitColor = Color.white;

    [Header("Stats")]
    public int maxHealth = 100;
    public int attack = 10;
    public float attackSpeed = 1;
    public int attackRange = 100;
    public float bulletSpeed = 15;

    [Header("Images")]
    public Sprite fullFront;
    public Sprite chibiIcon;

    [Header("Calculus")]
    public UnitType derivativeTo;
    public int derivativeAmount = 1;
    public UnitType integralTo;
}
