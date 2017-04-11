using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class APullHarpoon : ActionBehaviour {
    public PHarpoon harpoonProjectile;
    public override string GetActionName()
    {
        return "PullHarpoon";
    }
    protected override IEnumerator BehaviourCoroutine()
    {
        if (harpoonProjectile.gameObject.activeSelf)
        {
            //unitInfo.ChangeState(UnitInfo.UnitState.action);
            //animator.CrossFade("PullHarpoon", 0.025f);
            //harpoon.fixedTarget = null;
            //harpoon.joint.connectedBody = null;
            harpoonProjectile.ReturnHarpoon(PHarpoon.HARPOON_STATE.RETURNING);
        }
        yield return null;
        StopAction();
    }
}
