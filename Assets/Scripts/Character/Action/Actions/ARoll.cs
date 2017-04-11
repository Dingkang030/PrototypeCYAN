using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARoll : ActionBehaviour {
    [SerializeField]
    float speed = 4;
    public override string GetActionName()
    {
        return "Roll";
    }
    protected override IEnumerator BehaviourCoroutine()
    {
        Vector3 axis=Vector3.zero;
        axis = Camera.main.transform.TransformVector(new Vector3(Input.GetAxis("Horizontal"),0, Input.GetAxis("Vertical")));
        axis.y = 0;
        axis.Normalize();
        animator.CrossFade("roll", 0.25f);
        unitInfo.ChangeState(UnitInfo.UnitState.action);
        CharacterMovement movement = unitInfo.GetComponent<CharacterMovement>();
        CharacterInput input = unitInfo.GetComponent<CharacterInput>();
        input.SetZoomAble(false);


        SetStopActionEvent(delegate ()
        {
            input.SetZoomAble(true);
            if (CharacterInput.GetMovingInput() == Vector3.zero)
                movement.Stop();
        });

        if (axis == Vector3.zero)
            axis = unitInfo.transform.forward;
        movement.transform.rotation=Quaternion.LookRotation(axis);
        movement.StopRotating();
        movement.Move(axis, speed, speed, speed);
        yield return new WaitForStateChange(this, 2);
        StopAction();
    }

}
