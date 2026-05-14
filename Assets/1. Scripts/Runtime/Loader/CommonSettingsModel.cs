using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CommonSettingsModel
{
    [SerializeField] public List<UnitType> initalUnitlist;
    [SerializeField] public float skillCost;
    [SerializeField] public float skillGagePerSecond;
    [SerializeField] public float startSkillGage;
    [SerializeField] public float maxSkillGage;
    [SerializeField] public float backgroundScrollSpeed;
}