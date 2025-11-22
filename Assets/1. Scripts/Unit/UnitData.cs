using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "UnitData.asset", menuName = "Unit Data")]
public class UnitData : ScriptableObject
{
    public UnitType unitType;
    public string unitName;
    [TextArea] public string unitDescription;
    public int maxHealth;
    public Color unitColor;
    public Sprite fullFront;
    public Sprite chibiIcon;
}
