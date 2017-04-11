using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AFlying : ActionBehaviour {
    public float velocityY;
    public override string GetActionName()
    {
        return "Flying";
    }

    protected override IEnumerator BehaviourCoroutine()
    {
        float flyingTime;
        while(true)
        {
            if(Input.GetKeyDown(KeyCode.Space))
                unitInfo.GetMovement().GetRigidBody().velocity += Vector3.up * velocityY;
            yield return null;
        }
    }
    
}
