using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ADragonBreathAir : ActionBehaviour
{
    public Transform mouseTransform;
    public Vector3 mouseAngleOffset;
    public float breathDistance=7.0f;
    public override string GetActionName()
    {
        return "DragonBreathAir";
    }
    protected override IEnumerator BehaviourCoroutine()
    {
        GameObject prefab = Resources.Load<GameObject>("Effects/P_Breath");
        Vector3 forward = (targetObject.transform.position-mouseTransform.position).normalized;
        Vector3 euler = Quaternion.LookRotation(forward).eulerAngles;
        ProjectileBase pb=ProjectileBase.CreateProjectile(prefab, mouseTransform.position, euler, mouseTransform.position+forward*breathDistance, unitInfo.gameObject);
        pb.AddOnTriggerHitEvent(
            delegate (Collider col)
            {
                if (col.gameObject.layer == LayerMask.NameToLayer("ground"))
                    Destroy(pb.gameObject);
                if (col.tag=="Player")
                {
                    Debug.Log(col);
                    UnitInfo target = col.GetComponent<UnitInfo>();
                    Vector3 point=col.ClosestPointOnBounds(pb.transform.position);
                    unitInfo.GiveDamage(target, damage, point, 1);
                    Destroy(pb.gameObject);
                }
            }
        );
        yield return null;
        StopAction();
    }
}
