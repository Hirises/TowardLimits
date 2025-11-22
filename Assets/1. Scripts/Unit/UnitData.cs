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
    public Color unitColor;

    [Header("Stats")]
    public int maxHealth;
    public int attack;
    public float attackSpeed;
    public int attackRange;
    public float bulletSpeed;

    [Header("Images")]
    public Sprite fullFront;
    public Sprite chibiIcon;
}
