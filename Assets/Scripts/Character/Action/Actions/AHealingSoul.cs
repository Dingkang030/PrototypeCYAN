using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AHealingSoul : ActionBehaviour {
    public float castTime;
    public PHarpoon harpoon;
    public override string GetActionName()
    {
        return "HealingSoul";
    }

    protected override IEnumerator BehaviourCoroutine()
    {
        if (harpoon.harpoonState==PHarpoon.HARPOON_STATE.FIXED)
        {
            if (harpoon.fixedTarget!=null&&harpoon.fixedTarget.tag=="Monster")
            {
                Vector3 f = (harpoon.transform.position - unitInfo.transform.position).normalized;
                f.y = 0;
                unitInfo.GetMovement().RotateTo(f, 10);
                SoulInfo soulInfo = harpoon.fixedTarget.GetComponent<SoulInfo>();
                CameraSpring.Instance.ChangeCamInfo("Player_HealingSoul");
                if (soulInfo.IsAnEvil())
                {
                    //harpoon.transform.rotation = Quaternion.LookRotation(harpoon.transform.position-unitInfo.transform.position);
                    CameraSpring.Instance.SetFixTarget(harpoon.transform);
                    animator.CrossFade("HealingSoul_Ready", 0.025f);
                    unitInfo.ChangeState(UnitInfo.UnitState.action);
                    float percentage = 50;
                    float warning = 0;
                    float warningTime = 0;
                    float warningTimeDelay = 1;
                    //float timeCheck=castTime;
                    //while (timeCheck>0)
                    //{
                    //    timeCheck -= Time.deltaTime;
                    //    yield return null;
                    //}
                    harpoon.harpoonState = PHarpoon.HARPOON_STATE.RETURNING_WITH_SOUL;
                    LineRenderer lineRenderer = harpoon.chainDrawer.GetComponent<LineRenderer>();
                    lineRenderer.material.color = Color.green;
                    while (percentage<100&&percentage>=0)
                    {
                        f = (harpoon.transform.position - unitInfo.transform.position).normalized;
                        //f.y = 0;
                        unitInfo.GetMovement().RotateTo(f, 10);
                        if (warningTime > 0)
                        {
                            warningTime -= Time.deltaTime;
                            if (warningTime <= 0)
                            {
                                warningTimeDelay = UnityEngine.Random.Range(3.0f, 4.0f);
                                lineRenderer.material.color = Color.green;
                            }
                        }
                        else if (warningTimeDelay <= 0 && warningTime <= 0)
                        {
                            warningTime = UnityEngine.Random.Range(0.5f, 1.0f);
                            lineRenderer.material.color = Color.red;
                        }
                        else if (warningTimeDelay > 0)
                            warningTimeDelay -= Time.deltaTime;
                        if (Input.GetKey(KeyCode.Mouse0))
                        {
                            if (warningTime > 0)
                            { warning += Time.deltaTime; }
                            else { percentage += Time.deltaTime; }
                            animator.Play("HealingSoul_Pulling");
                        }
                        else
                        {
                            animator.Play("HealingSoul_Ready");
                            percentage -= Time.deltaTime;
                        }
                        yield return null;
                    }
                    //soulInfo.SetEvilGuage(soulInfo.GetEvilGuage()-100);
                }
            }
        }
        yield return null;
        CameraSpring.Instance.ChangeCamInfo("Player");
        StopAction();
    }
    
}
