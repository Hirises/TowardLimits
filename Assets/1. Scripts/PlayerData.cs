using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    public List<UnitStatus> units;  //보유한 유닛
    public int DT;
    public int Persuaded;   //설득됨!
    public Polar direction; //어디를 향하는가?
}
