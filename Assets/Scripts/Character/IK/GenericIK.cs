using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class IKTarget
{
    public Transform bodyBone;
    public float weight = 0.0f;
    Vector3 localEuler;
    public Vector3 offsetEuler;
    public Vector3 rotationLimit;
    public float parentDistance = -1;
    public bool isToggle=true;
    public Vector3 localBaseEuler
    {
        get
        {
            return localEuler;
        }
    }
    public void SetupDistance()
    {
        parentDistance = Vector3.Distance(bodyBone.position, bodyBone.parent.position);
    }
    public void SetuplocalEuler(Transform root)
    {
        Transform p = bodyBone.parent;
        Quaternion localRotation = bodyBone.localRotation;
        while(true)
        {
            Quaternion l = p.localRotation;
            localRotation = l*localRotation;
            if (root == p)
                break;
            else p = p.parent;
        }
        localEuler = localRotation.eulerAngles;
        /*
        Vector3 offset = bodyBone.localEulerAngles;
        
        while(true)
        {
            Vector3 le = p.localEulerAngles;
            if (le.x > 180) le.x -= 360;
            if (le.y > 180) le.y -= 360;
            if (le.z > 180) le.z -= 360;
            offset += le;
            Debug.Log("(" + bodyBone + ")" + bodyBone.localEulerAngles + " + (" + p + ")" + le + " = " + offset);
            if (root == p)
                break;
            else p = p.parent;
        }
        localEuler = offset;*/
    }
}

public class GenericIK : MonoBehaviour
{

    public List<IKTarget> ikTargets;
    public Transform root;
    public Transform target;
    public bool toggle;
    void Awake()
    {
        foreach (IKTarget t in ikTargets)
        {
            t.SetupDistance();
            t.SetuplocalEuler(root);
        }
    }
    public void ToggleIK(bool _t)
    {
        toggle = _t;
    }
    public void SetAllTarget(Transform _t)
    {
        target = _t;
    }
    void LateUpdate()
    {
        if (!toggle)
            return;
        foreach (IKTarget t in ikTargets)
        {
            if (!t.isToggle)
                continue;
            if (t.parentDistance == -1)
                t.SetupDistance();
            Vector3 maxTargetPos = t.bodyBone.parent.position + (target.position - t.bodyBone.parent.position).normalized * (t.parentDistance * t.weight);
            if (maxTargetPos != Vector3.zero)
            {
                Vector3 lookDir = (target.position - t.bodyBone.parent.position).normalized;
                Quaternion lookRot = Quaternion.Euler(t.localBaseEuler+t.offsetEuler);
                if (lookDir!=Vector3.zero)
                    lookRot = Quaternion.LookRotation(lookDir) * lookRot;
                Quaternion diff = lookRot * Quaternion.Inverse(t.bodyBone.rotation);
                Vector3 diffEuler = diff.eulerAngles;
                int[] p = { 1, 1, 1 };
                if (diffEuler.x > 180)
                {
                    diffEuler.x -= 360;
                    p[0] = -1;
                }
                if (diffEuler.y > 180)
                {
                    diffEuler.y -= 360;
                    p[1] = -1;
                }
                if (diffEuler.z > 180)
                {
                    diffEuler.z -= 360;
                    p[2] = -1;
                }
                if (Mathf.Abs(diffEuler.x) > t.rotationLimit.x)
                    diffEuler.x = t.rotationLimit.x * p[0];
                if (Mathf.Abs(diffEuler.y) > t.rotationLimit.y)
                    diffEuler.y = t.rotationLimit.y * p[1];
                if (Mathf.Abs(diffEuler.z) > t.rotationLimit.z)
                    diffEuler.z = t.rotationLimit.z * p[2];
                lookRot =  Quaternion.Euler(diffEuler)* t.bodyBone.rotation;
                t.bodyBone.rotation = lookRot;
            }
        }
    }
}
