using UnityEngine;
using System.Collections;

public class FootIKContorller : MonoBehaviour {
    IKHandling ik;
    Animator anim;
    public Transform rFoot;
    public Transform lFoot;
    Vector3 lPos;
    Vector3 rPos;
    Rigidbody rigidbody;
    public float findHeight;
    public Vector3 rOffset;
    public Vector3 lOffset;

    public bool useHandsInstead;

    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
        ik = GetComponent<IKHandling>();
        rigidbody = GetComponent<Rigidbody>();
        if(rFoot==null)
            rFoot = anim.GetBoneTransform(HumanBodyBones.RightFoot);
        if(lFoot==null)
            lFoot = anim.GetBoneTransform(HumanBodyBones.LeftFoot);
	}
	
	// Update is called once per frame
    void FixedUpdate()
    {

	}

    void OnAnimatorIK()
    {
        //Debug.Log("test");
        if (Time.deltaTime == 0)
            return;
        float left_weight=anim.GetFloat("leftFoot");
        float right_weight = anim.GetFloat("rightFoot");
        if (rFoot == null)
            rFoot = anim.GetBoneTransform(HumanBodyBones.RightFoot);
        if (lFoot == null)
            lFoot = anim.GetBoneTransform(HumanBodyBones.LeftFoot);
        /*rPosDetected = Physics.Raycast(rFoot.position, -Vector3.up, out rPos, 3f, LayerMask.GetMask("ground"));
        lPosDetected = Physics.Raycast(lFoot.position, -Vector3.up, out lPos, 3f, LayerMask.GetMask("ground"));*/
        RaycastHit rHit;
        RaycastHit lHit;
        bool ikTrue=false;
        if (Physics.Raycast(lFoot.position +Vector3.up * findHeight, - Vector3.up, out lHit , findHeight*2, LayerMask.GetMask("ground")))
        {
            if(useHandsInstead)
                anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, left_weight);
            else anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, left_weight);
            lPos = lHit.point + transform.TransformVector(lOffset);
            ikTrue = true;
        }
        else
        {
            if (useHandsInstead)
                anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
            else anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0);
        }
        if (!useHandsInstead)
            anim.SetIKPosition(AvatarIKGoal.LeftFoot, lPos);
        else anim.SetIKPosition(AvatarIKGoal.LeftHand, lPos);
        Quaternion lFrot = Quaternion.FromToRotation(transform.up, lHit.normal) * transform.rotation;
        if (!useHandsInstead)
            anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, left_weight);
        else
            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, left_weight);
        if (!useHandsInstead)
            anim.SetIKRotation(AvatarIKGoal.LeftFoot, lFrot);
        else
            anim.SetIKRotation(AvatarIKGoal.LeftHand, lFrot);

        if (Physics.Raycast(rFoot.position + Vector3.up * findHeight, -Vector3.up, out rHit,findHeight*2, LayerMask.GetMask("ground")))
        {
            if (!useHandsInstead)
                anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, right_weight);
            else
                anim.SetIKPositionWeight(AvatarIKGoal.RightHand, right_weight);
            rPos = rHit.point + transform.TransformVector(rOffset);
            ikTrue = true;
        }
        else
        {
            if (!useHandsInstead)
                anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0);
            else
                anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
        }
        if (!useHandsInstead)
            anim.SetIKPosition(AvatarIKGoal.RightFoot, rPos);
        else
            anim.SetIKPosition(AvatarIKGoal.RightHand, rPos);
        Quaternion rFrot = Quaternion.FromToRotation(transform.up, rHit.normal) * transform.rotation;
        if (!useHandsInstead)
            anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, right_weight);
        else
            anim.SetIKRotationWeight(AvatarIKGoal.RightHand, right_weight);
        if (!useHandsInstead)
            anim.SetIKRotation(AvatarIKGoal.RightFoot, rFrot);
        else
            anim.SetIKRotation(AvatarIKGoal.RightHand, rFrot);
        /*
        if (ikTrue)
        {
            Vector3 smaller = (lPos.y > rPos.y) ? rPos-offset : lPos-offset;
            if (smaller.y < transform.position.y)
            {
                colliderController.ExpandCollider(smaller.y - transform.position.y,0.1f);
            }
            else colliderController.ResetCollider(0.1f);
        }
        else colliderController.ResetCollider(0.1f);*/

    }
}
