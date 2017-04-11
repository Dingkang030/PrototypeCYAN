using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulInfo : MonoBehaviour {

    public float evilGuage=100.0f;
    public BuffBehaviour.BuffType buffType;
    public float buffTime;
    //public BuffBehaviour soulBuff;

	void Start () {
		
	}

    

    public bool IsAnEvil()
    {
        return evilGuage > 0;
    }
    

    public void SetEvilGuage(float _amount)
    {
        evilGuage = _amount;
    }
    public float GetEvilGuage()
    {
        return evilGuage;
    }

	void Update () {
		
	}
}
