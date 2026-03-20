using System;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class EnemyModel
{
    [Header("Stats")]
    [SerializeField] public int health;
    [SerializeField] public int damage;
    [SerializeField] public int persuade;
    [SerializeField] public float speed;
    [SerializeField] public Color color;

    [Header("Level up")]
    [SerializeField] public int health_add;
    [SerializeField] public int damage_add;
    [SerializeField] public float speed_add;
}