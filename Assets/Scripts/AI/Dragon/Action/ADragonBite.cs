using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ADragonBite : ActionBehaviour {
    
    public Transform headTransform;
    public Vector3 hitOffset;
    public Vector3 hitSize;
    public float knockBackDistance;
    GenericIK ik;
    public override string GetActionName()
    {
        return "DragonBite";
    }
    protected override IEnumerator BehaviourCoroutine()
    {
        unitInfo.ChangeState(UnitInfo.UnitState.action);
        animator.CrossFade(GetActionName(), 0.45f);
        unitInfo.GetMovement().Stop();
        unitInfo.GetMovement().StopRotating();
        if(ik==null)
            ik = unitInfo.GetComponent<GenericIK>();
        ik.ToggleIK(false);
        while (behaviourState < 2)
        {
            if (eventToggle)
            {
                DamageInfo[] dinfos = DamageGenerator.ApplyBoxDamage(unitInfo, unitInfo.transform.position+ unitInfo.transform.rotation*hitOffset, hitSize,unitInfo.transform.rotation, damage, false, LayerMask.GetMask("Unit"));
                foreach (DamageInfo dinfo in dinfos)
                {
                    dinfo.hitUnit.GetAction().StopAction();
                    Vector3 f = unitInfo.transform.position - dinfo.hitPoint;
                    Vector3 euler = Quaternion.LookRotation(f).eulerAngles;
                    euler.x = 0;
                    euler.z = 0;
                    dinfo.hitUnit.transform.eulerAngles = euler;
                    dinfo.hitUnit.GetComponent<Rigidbody>().velocity = Vector3.up * 1 + (dinfo.hitUnit.transform.position - unitInfo.transform.position).normalized * knockBackDistance;
                    dinfo.hitUnit.GetAction().StopAction();
                    dinfo.hitUnit.AddCrowdControl(ConditionData.UnitCondition.knockBack, 1,
                       delegate () { dinfo.hitUnit.GetComponent<ActionComponent>().UseSkill("WakeUp", true); });
                }
                eventToggle = false;
            }
            yield return null;
        }
        //yield return new WaitForStateChange(this, 3);
        unitInfo.ChangeState(UnitInfo.UnitState.idle);
        StopAction();
    }
}
