using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AHangHarpoon : ActionBehaviour {

    [SerializeField]
    PHarpoon harpoon;

    public override string GetActionName()
    {
        return "HangHarpoon";
    }

    protected override IEnumerator BehaviourCoroutine()
    {
        unitInfo.ChangeState(UnitInfo.UnitState.action);
        animator.CrossFade(GetActionName(), 0.15f);
        while (true)
            yield return null;
        yield return null;
    }
    
	
	// Update is called once per frame
	void Update () {
		
	}
}
