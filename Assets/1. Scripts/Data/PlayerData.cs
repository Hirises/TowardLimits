using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerData
{
    public List<UnitStatus> units;  //보유한 유닛
    public int DT;
    public int Persuaded;   //설득됨!
    public int Prove;   //설득함!
    public Polar direction; //어디를 향하는가?
    public int stage;   //현재 스테이지 카운트

    public PlayerData(){
        units = new List<UnitStatus>(GameManager.instance.initalUnitlist.Select(type => UnitStatus.FromType(type)));
        DT = 0;
        Persuaded = 0;
        direction = Polar.North;
        stage = 0;
        Prove = 0;
    }
}
