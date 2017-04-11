using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct DamageInfo
{
    public UnitInfo hitUnit;
    public Vector3 hitPoint;
}
public class DamageGenerator {
    
    static public DamageInfo[] ApplyRadialDamage(UnitInfo causer, Vector3 position, float radius, float damage,bool affectToCauser, params string[] tags)
    {
        Collider[] cols = HitBox.LaunchHitSphere(position, radius);
        return CheckTag(cols, causer, affectToCauser, damage, position, tags);
    }
    static public DamageInfo[] ApplyRadialDamage(UnitInfo causer, Vector3 position, float radius, float damage, bool affectToCauser, LayerMask mask)
    {
        Collider[] cols = HitBox.LaunchHitSphere(position, radius, mask);
        return Check(cols, causer, affectToCauser, damage,position);
    }
    static public DamageInfo[] ApplyRadialDamage(UnitInfo causer, Vector3 position, float radius, float damage, bool affectToCauser, LayerMask mask, params string[] tags)
    {
        Collider[] cols = HitBox.LaunchHitSphere(position, radius, mask);
        return CheckTag(cols, causer, affectToCauser, damage, position, tags);
    }


    static public DamageInfo[] ApplyBoxDamage(UnitInfo causer, Vector3 position, Vector3 extends, Quaternion rotation, float damage, bool affectToCauser, params string[] tags)
    {
        Collider[] cols = HitBox.LaunchHitBox(position, extends, rotation);
        return CheckTag(cols, causer, affectToCauser, damage, position, tags);
    }
    static public DamageInfo[] ApplyBoxDamage(UnitInfo causer, Vector3 position, Vector3 extends, Quaternion rotation, float damage, bool affectToCauser, LayerMask mask,params string[] tags)
    {
        Collider[] cols = HitBox.LaunchHitBox(position, extends, rotation, mask);
        return CheckTag(cols, causer, affectToCauser, damage, position,tags);
    }
    static public DamageInfo[] ApplyBoxDamage(UnitInfo causer, Vector3 position, Vector3 extends, Quaternion rotation, float damage, bool affectToCauser, LayerMask mask)
    {
        Collider[] cols = HitBox.LaunchHitBox(position, extends, rotation,mask);
        return Check(cols,causer,affectToCauser,damage, position);
    }

    static DamageInfo[] Check(Collider[] cols,UnitInfo causer,bool affectToCauser,float damage,Vector3 damagePoint)
    {
        List<DamageInfo> damageInfoList = new List<DamageInfo>();
        if (cols != null && cols.Length > 0)
        {
            foreach (Collider tr in cols)
            {
                UnitInfo target = tr.attachedRigidbody.GetComponent<UnitInfo>();
                if (target == null)
                    continue;
                if (!affectToCauser && target == causer)
                    continue;
                Vector3 point = tr.ClosestPointOnBounds(damagePoint);
                if (causer.GiveDamage(target, damage, point))
                {
                    DamageInfo dInfo;
                    dInfo.hitPoint = point;
                    dInfo.hitUnit = target;
                    damageInfoList.Add(dInfo);
                }
            }
        }
        return damageInfoList.ToArray();
    }
    static DamageInfo[] CheckTag(Collider[] cols, UnitInfo causer, bool affectToCauser, float damage, Vector3 damagePoint, params string[] tags)
    {
        List<DamageInfo> damageInfoList = new List<DamageInfo>();
        if (cols != null && cols.Length > 0)
        {
            foreach (Collider tr in cols)
            {
                UnitInfo target = tr.attachedRigidbody.GetComponent<UnitInfo>();
                if (target == null)
                    continue;
                if (!affectToCauser && target == causer)
                    continue;
                foreach (string tag in tags)
                {
                    if (tr.tag == tag)
                    {
                        Vector3 point = tr.ClosestPointOnBounds(damagePoint);
                        if (causer.GiveDamage(target, damage, point))
                        {
                            DamageInfo dInfo;
                            dInfo.hitPoint = point;
                            dInfo.hitUnit = target;
                            damageInfoList.Add(dInfo);
                        }
                    }
                }
            }
        }
        return damageInfoList.ToArray();
    }
}
