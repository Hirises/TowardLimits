using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CommonSettingsModel
{
    [SerializeField] public List<UnitType> initalUnitlist;
    [SerializeField] public int skillCooldown;
}