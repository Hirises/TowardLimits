using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData.asset", menuName = "Enemy Data")]
public class EnemyData : ScriptableObject
{
    [InlineProperty, HideLabel] public EnemyModel enemyModel;
}
