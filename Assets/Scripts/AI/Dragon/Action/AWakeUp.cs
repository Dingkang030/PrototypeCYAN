using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AWakeUp : ActionBehaviour
{
    public override string GetActionName()
    {
        return "WakeUp";
    }

    protected override IEnumerator BehaviourCoroutine()
    {
        Debug.Log("WakeUp");
        unitInfo.ChangeState(UnitInfo.UnitState.action);
        animator.CrossFade(GetActionName(), 0.15f);
        unitInfo.ToggleInvincible(true);
        yield return new WaitForStateChange(this, 2);
        unitInfo.ToggleInvincible(false);
        unitInfo.SetInvincibleTime(1.0f);
        StopAction();
    }
}
