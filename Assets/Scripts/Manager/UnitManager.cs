using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : Singleton<UnitManager> {

    //static UnitManager instance;

    List<UnitInfo> unitList=new List<UnitInfo>();

    UnitInfo playerUnit;
    CharacterInput userInput;

    CameraSpring mainCameraObject;

    //public static UnitManager Instance
    //{
    //    get { return instance; }
    //}
    //public void Awake()
    //{
    //    instance = this;
    //}
    public UnitInfo[] units
    {
        get { return unitList.ToArray(); }
    }

    public UnitInfo GetPlayerUnit()
    {
        return unitList.Find(get => get.transform.tag == "Player");
    }

    public void AddUnit(UnitInfo newUnit)
    {
        UnitInfo u = unitList.Find(delegate (UnitInfo unit) { return unit == newUnit; });
        if (u == null)
            unitList.Add(newUnit);
    }
    public void RemoveUnit(UnitInfo newUnit)
    {
        UnitInfo u = unitList.Find(delegate (UnitInfo unit) { return unit == newUnit; });
        if (u != null)
            unitList.Remove(u);
    }
    
}
