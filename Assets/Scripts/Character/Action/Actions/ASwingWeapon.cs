using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ASwingWeapon : ActionBehaviour {

    [System.Serializable]
    class AttackCollider
    {
        public Vector3 center;
        public Vector3 halfExtents;
        public Vector3 orientation;
    };
    [SerializeField]
    AttackCollider[] hData;
    
    public GameObject hitEffect;
    public float stopFrame = 0.05f;
    public override string GetActionName()
    {
        return "SwingWeapon";
    }
    public override bool PlayAction()
    {
        if (!actionComponent.IsUsableWeapon("Harpoon"))
            return false;
        return base.PlayAction();
    }
    protected override IEnumerator BehaviourCoroutine()
    {
        // type 0
        CharacterMovement movement = unitInfo.GetComponent<CharacterMovement>();
        unitInfo.ChangeState(UnitInfo.UnitState.action);
        int comboLength = hData.Length;// 기본 3타 현재 1타

        SetStopActionEvent(delegate ()
        {
            unitInfo.ChangeState(UnitInfo.UnitState.idle);
            unitInfo.GetMovement().Stop();
        });

        for (int i = 0; i < comboLength; i++)
        {
            animator.CrossFade("SwingWeapon_"+(i), 0.1f);
            bool combo = false;
            Vector3 axis = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
            {
                axis = Vector3.forward;
                SetRotation(movement, Vector3.forward);
            }
            else
            {
                SetRotation(movement, axis);
            }
            movement.Stop();
            axis = Camera.main.transform.TransformVector(axis);
            axis.y = 0;
            axis.Normalize();
            movement.Move(axis, 2, 2, 2);
            //ResetBehaviourEvent();//타격판정꺼줌
            while(behaviourMessage!="swing_comboCheckEnd")
            {
                yield return new WaitForFixedUpdate();
                if (!combo&&Input.GetKeyDown(KeyCode.Mouse0))
                {
                    combo = true;
                }
                if (behaviourMessage== "swing_hitEvent")
                {
                    eventToggle = false;
                    DamageInfo[] infos = DamageGenerator.ApplyBoxDamage(unitInfo, unitInfo.transform.TransformPoint(hData[i].center), hData[i].halfExtents, Quaternion.Euler(hData[i].orientation), damage, false, LayerMask.GetMask("Unit", "Ignore Ground"));
                    if (infos.Length > 0)
                    {
                        foreach (DamageInfo info in infos)
                        {
                            if (hitEffect)
                            {
                                GameObject effect = Instantiate<GameObject>(hitEffect, info.hitPoint,unitInfo.transform.rotation);
                                //effect.transform.position = info.hitPoint;
                                Destroy(effect, 3.0f);
                            }
                            //Debug.DrawLine(info.hitPoint, info.hitPoint + Vector3.up * 3f,Color.blue,5);
                        }

                        GameObject cameraObject = ControlManager.Instance.mainCameraObject;
                        float sF = stopFrame;
                        if (Vector3.Distance(cameraObject.transform.position, unitInfo.transform.position) <= 3)
                        {
                            if (i == comboLength - 1)
                            {
                                sF *= 2;
                                cameraObject.GetComponent<CameraShaker>().ShakeCamera(0.2f, 0.3f, 0.03f);
                            }
                            else
                            {
                                //cameraObject.GetComponent<CameraShaker>().ShakeCamera(0.1f, 0.1f, 0.03f);
                            }
                            HitEffector.Instance.StopFrameEffect(sF);
                        }
                    }
                }
                //yield return null;
            }
            if (!combo)
            {
                break;
            }
            else if(i<comboLength-1)behaviourMessage = "none";
        }
        yield return new WaitForMessageReceiving(this, "swing_Ended");
        StopAction();
    }
    void SetRotation(CharacterMovement movement,Vector3 axis)
    {
        axis = Camera.main.transform.TransformVector(axis);
        axis.y = 0;
        axis.Normalize();
        movement.RotateTo(Quaternion.LookRotation(axis), 10);
    }
}
