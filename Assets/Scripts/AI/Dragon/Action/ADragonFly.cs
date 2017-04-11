using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ADragonFly : ActionBehaviour
{
    public float flyHeight=5.0f;
    public override string GetActionName()
    {
        return "DragonFly";
    }

    protected override IEnumerator BehaviourCoroutine()
    {
        animator.CrossFade(GetActionName(),0.1f);
        unitInfo.ChangeState(UnitInfo.UnitState.action);
        while (animator.GetFloat("posY") == 0)
            yield return null;
        Rigidbody rigid = unitInfo.GetMovement().GetRigidBody();
        Vector3 basePosition = rigid.position;
        rigid.useGravity = false;
        while (behaviourState < 2)
        {
            rigid.position = basePosition + Vector3.up * animator.GetFloat("posY") * flyHeight;
            yield return null;
        }
        //unitInfo.GetMovement().Move(Vector3.up, 3);
        animator.SetLayerWeight(1, 1);
        StopAction();
        unitInfo.GetMovement().SetMovingType(CharacterMovement.MOVING_TYPE.flying);
    }
}
