using UnityEngine;
using System.Collections;

public class IKHandling : MonoBehaviour {

    public enum IK_TYPE { RIGHT_HAND, LEFT_HAND, RIGHT_ELBOW, LEFT_ELBOW, HEAD, BODY, RIGHT_SHOULDER, LEFT_SHOULDER, COUNT=8 };
    public enum ANIM_LAYER { BASE, UPPER };
    public Animator anim;
    

    [Header("<Right Hand>")]
    public Transform right_hand_target;

    [Header("<Left Hand>")]
    public Transform left_hand_target;

    public Transform lookAt;
    
    [Header("[0 RHAND 1 LHAND 2 RELBOW 3 LEBLOW 4 HEAD 5 BODY]")]
    [Header("<Weight>")]
    public float[] ik_weight;
    public float[] ik_max_weight;
    public bool[] ik_toggle=new bool[(int)IK_TYPE.COUNT];
    public bool isGeneric;
    public Transform head;
	// Use this for initialization
    
    public void SetIKWeight(IK_TYPE _ik_type,bool _toggle)
    {
        if (_ik_type == IK_TYPE.COUNT)
            return;
        ik_toggle[(int)_ik_type] = _toggle;
    }

	void Awake () {
        anim = GetComponent<Animator>();
        ik_weight = new float[(int)IK_TYPE.COUNT];
        ik_max_weight = new float[(int)IK_TYPE.COUNT];
        //sik_toggle = new bool[(int)IK_TYPE.COUNT];

        for(int i=0;i<(int)IK_TYPE.COUNT;i++)
        {
            ik_max_weight[i] = 1.0f;
        }
	}
	
	// Update is called once per frame
    void FixedUpdate()
    {
        for (int i = 0; i < (int)IK_TYPE.COUNT;i++ )
        { 
            if (ik_toggle[i]&&ik_weight[i]<ik_max_weight[i])
                ik_weight[i] = Mathf.MoveTowards(ik_weight[i], ik_max_weight[i], 5.0f * Time.fixedDeltaTime);
            else if(!ik_toggle[i]&&ik_weight[i]>0.0f)
                ik_weight[i] = Mathf.MoveTowards(ik_weight[i], 0.0f, 5.0f * Time.fixedDeltaTime);
        }

	}
    void OnAnimatorIK()
    {
        Vector3 lookTarget = new Vector3();
        if (lookAt != null)
        {
            lookTarget = lookAt.position+lookAt.forward*0.1f;
        }
        anim.SetLookAtWeight(1.0f, -1, ik_weight[(int)IK_TYPE.HEAD]);
        anim.SetLookAtPosition(lookTarget);
        /*anim.SetLookAtWeight(1, 0, ik_weight[(int)IK_TYPE.HEAD]);
        anim.SetLookAtPosition(lookTarget);*/
        
        if (left_hand_target!=null)
        {
            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, ik_weight[(int)IK_TYPE.LEFT_HAND]);
            anim.SetIKPosition(AvatarIKGoal.LeftHand, left_hand_target.position);
            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, ik_weight[(int)IK_TYPE.LEFT_HAND]);
            anim.SetIKRotation(AvatarIKGoal.LeftHand, left_hand_target.rotation);
            
        }

        if (right_hand_target!=null)
        {
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, ik_weight[(int)IK_TYPE.RIGHT_HAND]);
            anim.SetIKPosition(AvatarIKGoal.RightHand, right_hand_target.position);
            anim.SetIKRotationWeight(AvatarIKGoal.RightHand, ik_weight[(int)IK_TYPE.RIGHT_HAND]);
            anim.SetIKRotation(AvatarIKGoal.RightHand, right_hand_target.rotation);
        }
        
    }
}
