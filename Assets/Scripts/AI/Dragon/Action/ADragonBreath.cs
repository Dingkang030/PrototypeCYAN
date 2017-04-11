using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ADragonBreath : ActionBehaviour
{
    public Transform mouseTransform;
    public Vector3 mouseAngleOffset;
    public float breathDistance=7.0f;
    public Vector3 extends;
    public Vector3 offset;
    GameObject breathParticle;
    public override string GetActionName()
    {
        return "DragonBreath";
    }
    public override void StopAction()
    {
        base.StopAction();
        
        Destroy(breathParticle);
        unitInfo.GetAnimator().speed = 1.0f;
    }
    protected override IEnumerator BehaviourCoroutine()
    {
        GenericIK ik = unitInfo.transform.GetComponent<GenericIK>();
        unitInfo.ChangeState(UnitInfo.UnitState.action);
        animator.CrossFade("DragonBreath",0.45f);
        unitInfo.GetMovement().Stop();
        unitInfo.GetMovement().StopRotating();
        ik.ToggleIK(false);
        

        yield return new WaitForStateChange(this, 2);
        animator.speed = 0.3f;
        breathParticle = Instantiate<GameObject>(Resources.Load<GameObject>("Effects/P_EF_Frost"));
        breathParticle.transform.parent = mouseTransform;
        breathParticle.transform.eulerAngles = mouseTransform.eulerAngles+ mouseAngleOffset;
        breathParticle.transform.localPosition = Vector3.zero;
        SetStopActionEvent(delegate (){
            unitInfo.GetAnimator().speed = 1.0f;
            Destroy(breathParticle);
        });
        Vector3 nOffsetEuler = mouseAngleOffset;
        nOffsetEuler.x -= mouseAngleOffset.x;
        while (behaviourState==2)
        {
            DamageInfo[] infos=DamageGenerator.ApplyBoxDamage(unitInfo, mouseTransform.position + Quaternion.Euler(mouseTransform.eulerAngles + nOffsetEuler) *offset, extends, Quaternion.Euler(mouseTransform.eulerAngles+ nOffsetEuler), damage, false, LayerMask.GetMask("Unit"));
            foreach (DamageInfo info in infos)
            {
                info.hitUnit.SetInvincibleTime(0.5f);
            }
            yield return null;
        }
        animator.speed = 1.0f;
        Destroy(breathParticle);
        yield return new WaitForStateChange(this, 4);
        StopAction();
    }
}
