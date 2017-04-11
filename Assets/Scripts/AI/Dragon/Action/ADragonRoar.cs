using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ADragonRoar : ActionBehaviour
{
    public float roarRadius=5.0f;
    public float knockBackDistance = 3.0f;
    public Transform headTransform;
    public override string GetActionName()
    {
        return "DragonRoar";
    }

    protected override IEnumerator BehaviourCoroutine()
    {
        unitInfo.ChangeState(UnitInfo.UnitState.action);
        animator.CrossFade(GetActionName(), 0.25f);
        unitInfo.ToggleInvincible(true);
        yield return new WaitForStateChange(this, 2);

        GameObject cameraObject = ControlManager.Instance.mainCameraObject;
        if (Vector3.Distance(cameraObject.transform.position, unitInfo.transform.position) <= roarRadius * 5)
        {
            cameraObject.GetComponent<CameraShaker>().ShakeCamera(0.6f,1f, 0.4f);
        }
        DamageInfo[] dInfos = DamageGenerator.ApplyRadialDamage(unitInfo, transform.position, roarRadius, 1, false, LayerMask.GetMask("Unit"));
        foreach (DamageInfo dinfo in dInfos)
        {
            Vector3 forward = unitInfo.transform.position - dinfo.hitPoint;
            Vector3 euler=Quaternion.LookRotation(forward).eulerAngles;
            euler.x = 0;
            euler.z = 0;
            dinfo.hitUnit.transform.eulerAngles = euler;
            dinfo.hitUnit.GetComponent<Rigidbody>().velocity = Vector3.up * 1+ (dinfo.hitUnit.transform.position- (transform.position).normalized * knockBackDistance);
            dinfo.hitUnit.GetAction().StopAction();
            dinfo.hitUnit.AddCrowdControl(ConditionData.UnitCondition.knockBack, 1,
               delegate() { dinfo.hitUnit.GetComponent<ActionComponent>().UseSkill("WakeUp", true); });
        }
        yield return new WaitForStateChange(this, 3);
        unitInfo.ToggleInvincible(false);
        unitInfo.ChangeState(UnitInfo.UnitState.idle);
        StopAction();
    }
    public override void StopAction()
    {
        base.StopAction();
    }
}
