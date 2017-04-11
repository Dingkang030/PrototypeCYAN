using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKHand : IKController {
    public float handHeight;
    public bool handleTarget;
    protected override void Start()
    {
        base.Start();
        leftTransform = animator.GetBoneTransform(HumanBodyBones.LeftHand);
        rightTransform = animator.GetBoneTransform(HumanBodyBones.RightHand);
    }
    protected override void OnAnimatorIK()
    {
        Vector3 dir = transform.rotation* leftDirection;

        RaycastHit leftHit;

        float leftParamater = animator.GetFloat("leftHand");
        if (Physics.Raycast(leftTransform.position - dir * rayHeight, dir, out leftHit, rayHeight*2, ikMask)||handleTarget)
        {
            leftWeight = 1;//Mathf.MoveTowards(leftWeight, 1, Time.deltaTime* ikSpeed);
        }
        else
        {
            leftWeight = 0;// Mathf.MoveTowards(leftWeight, 0, Time.deltaTime* ikSpeed);
        }
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftWeight);
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftWeight);

        if (leftWeight > 0)
        {
            if (!handleTarget || leftTarget == null)
            {
                if (handleTarget && leftTarget == null)
                { }
                else
                {
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHit.point + (transform.rotation * leftOffset) + (-dir * handHeight * (1 - leftParamater)));
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, Quaternion.FromToRotation(transform.up, leftHit.normal));
                }
            }
            else
            {
                animator.SetIKPosition(AvatarIKGoal.LeftHand, leftTarget.position + (-dir * handHeight * (1 - leftParamater)));
                animator.SetIKRotation(AvatarIKGoal.LeftHand, leftTarget.rotation * Quaternion.FromToRotation(transform.up, leftTarget.up));
            }
        }
        //---------------------------------------------------------------------
        // Right----------------------------------------------------------------
        dir = transform.rotation * rightDirection;
        RaycastHit rightHit;

        float rightParamater = animator.GetFloat("rightHand");
        if (Physics.Raycast(rightTransform.position - dir * rayHeight, dir, out rightHit, rayHeight * 2, ikMask) || handleTarget)
        {
            rightWeight = 1;// Mathf.MoveTowards(rightWeight, 1, Time.deltaTime* ikSpeed);
        }
        else
        {
            rightWeight = 0;// Mathf.MoveTowards(rightWeight, 0, Time.deltaTime* ikSpeed);
        }
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, rightWeight);
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, rightWeight);
        if (rightWeight > 0)
        {
            if (!handleTarget||rightTarget==null)
            {
                if (handleTarget && rightTarget == null)
                    return;
                animator.SetIKPosition(AvatarIKGoal.RightHand, rightHit.point + (transform.rotation * rightOffset) + (-dir * handHeight * (1 - rightParamater)));
                animator.SetIKRotation(AvatarIKGoal.RightHand, transform.rotation * Quaternion.FromToRotation(transform.up, rightHit.normal));
            }
            else
            {
                animator.SetIKPosition(AvatarIKGoal.RightHand, rightTarget.TransformPoint(rightOffset)+(-dir * handHeight * (1 - rightParamater)));
                animator.SetIKRotation(AvatarIKGoal.RightHand, rightTarget.rotation * Quaternion.FromToRotation(transform.up, rightTarget.up));
            }
        }

    }
}
