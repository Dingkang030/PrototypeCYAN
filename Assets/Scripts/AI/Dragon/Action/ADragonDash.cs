using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ADragonDash : ActionBehaviour
{

    public float dashDistance=3.0f;
    public float dashHeight=8.0f;

    public float offsetZ = 0.0f;

    public float impactRadius = 4.0f;
    public float knockBackDistance = 4.0f;

    public override string GetActionName()
    {
        return "DragonDash";
    }

    protected override IEnumerator BehaviourCoroutine()
    {
        unitInfo.ChangeState(UnitInfo.UnitState.action);
        animator.CrossFade(GetActionName(), 0);
        unitInfo.GetMovement().Stop();
        unitInfo.GetMovement().StopRotating();
        float deltaZ=0;
        float deltaY = 0;
        Transform unitTransform = unitInfo.transform;
        Vector3 startPosition = unitTransform.position;
        Vector3 newPosition = unitTransform.position;
        Rigidbody rigid = unitInfo.GetMovement().GetRigidBody();
        GenericIK ik = unitTransform.GetComponent<GenericIK>();
        ik.ToggleIK(false);
        Vector3 forward = unitTransform.forward;
        float distance = dashDistance- offsetZ;
        if (targetObject != null)
        {
            forward = (targetObject.position - unitTransform.position).normalized;
            distance = Vector3.Distance(targetObject.position, unitTransform.position) - offsetZ;
        }
        bool rotated = false;
        while (behaviourState < 2)
        {
            deltaZ = animator.GetFloat("posZ");
            deltaY = animator.GetFloat("posY");
            if (deltaY >= 0.9f)
            {
                rotated = false;
            }
            if (deltaZ > 0.1f&& !rotated)
            {
                rotated = true;
                forward.y = 0;
                unitInfo.GetMovement().RotateTo(forward, 5);
                unitInfo.GetMovement().GetRigidBody().useGravity = false;
                //unitInfo.GetMovement().enabled = false;
            }
            newPosition = startPosition+forward * deltaZ * distance + unitTransform.up* deltaY * dashHeight;
            rigid.position=(newPosition);

            yield return null;
        }
        GameObject cameraObject = ControlManager.Instance.mainCameraObject;
        if(Vector3.Distance(cameraObject.transform.position,unitTransform.position)<=impactRadius*2)
        {
            cameraObject.GetComponent<CameraShaker>().ShakeCamera(1, 0.3f);
        }
        DamageInfo[] dInfos = DamageGenerator.ApplyRadialDamage(unitInfo, unitTransform.position, impactRadius, 1, false, LayerMask.GetMask("Unit"));
        foreach (DamageInfo dinfo in dInfos)
        {
            Vector3 f = unitInfo.transform.position - dinfo.hitPoint;
            Vector3 euler = Quaternion.LookRotation(f).eulerAngles;
            euler.x = 0;
            euler.z = 0;
            dinfo.hitUnit.transform.eulerAngles = euler;
            dinfo.hitUnit.GetComponent<Rigidbody>().velocity = Vector3.up * 1 + (dinfo.hitUnit.transform.position - unitTransform.position).normalized * knockBackDistance;
            dinfo.hitUnit.GetAction().StopAction();
            dinfo.hitUnit.AddCrowdControl(ConditionData.UnitCondition.knockBack, 1,
               delegate () { dinfo.hitUnit.GetComponent<ActionComponent>().UseSkill("WakeUp", true); });
        }
        EffectGenerator effect = unitInfo.GetComponent<EffectGenerator>();
        effect.GenerateEffect("DashDust");
        yield return new WaitForStateChange(this, 3);
        Reset();
        StopAction();
    }
    public void Reset()
    {
        unitInfo.ChangeState(UnitInfo.UnitState.idle);
        unitInfo.GetMovement().enabled = true;
        unitInfo.GetMovement().GetRigidBody().useGravity = true;
    }
    public override void StopAction()
    {
        Reset();
        base.StopAction();
    }
}
