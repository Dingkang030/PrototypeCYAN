using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AHookShot : ActionBehaviour {
    public PHarpoon harpoon;
    public float flySpeed;
    public float grabSpeed;
    public float attachHeigt;
    public override string GetActionName()
    {
        return "HookShot";
    }

    protected override IEnumerator BehaviourCoroutine()
    {
        if (harpoon.harpoonState == PHarpoon.HARPOON_STATE.FIXED)
        {
            SetStopActionEvent(
                delegate()
                {
                    harpoon.DettachObject(harpoon.transform);
                    harpoon.ResetProjectile();
                    unitInfo.GetMovement().SetMovingType(CharacterMovement.MOVING_TYPE.ground);
                    CameraSpring.Instance.ChangeCamInfo("Player");
                }
                );
            bool returning = false;
            //Rigidbody targetBody = harpoon.fixedTarget.GetComponent<Rigidbody>();
            Rigidbody rigidbody = unitInfo.GetComponent<Rigidbody>();
            Rigidbody harpoonBody=harpoon.GetComponent<Rigidbody>();
            unitInfo.GetMovement().SetMovingType(CharacterMovement.MOVING_TYPE.flying);
            Rigidbody targetRigid=harpoon.fixedTarget.GetComponent<Rigidbody>();
            if (targetRigid != null && targetRigid.mass <= rigidbody.mass)
            {
                harpoon.ReturnHarpoon(PHarpoon.HARPOON_STATE.RETURNING_WITH_OBJECT);
                returning = true;
            }
            else
            {
                unitInfo.ChangeState(UnitInfo.UnitState.action);
                animator.CrossFade(GetActionName(), 0.025f);
                harpoon.ReturnHarpoon(PHarpoon.HARPOON_STATE.FLYING);
                CameraSpring.Instance.ChangeCamInfo("Player_Flying");
            }
            while (harpoon.harpoonState != PHarpoon.HARPOON_STATE.NONE&&harpoon.harpoonState!=PHarpoon.HARPOON_STATE.HANGING)
                yield return null;
            
            if(!returning)
            {
                harpoon.chainDrawer.transform.position = Vector3.up * 10000f;
                CameraSpring.Instance.ChangeCamInfo("Player_Aim");
                animator.CrossFade(GetActionName()+"_hang", 0.25f);
                unitInfo.transform.position = harpoon.transform.position - Vector3.up * attachHeigt;
                harpoon.AttachObjectToObject(harpoon.transform,unitInfo.transform,true);
                while (!Input.GetKey(KeyCode.Space))
                {
                    yield return null;
                }
                harpoon.DettachObject(harpoon.transform);
                harpoon.ResetProjectile();
            }
            
            unitInfo.GetMovement().SetMovingType(CharacterMovement.MOVING_TYPE.ground);
            unitInfo.EquipWeapon(1);
        }
        StopAction();
        yield return null;
    }
    
}
