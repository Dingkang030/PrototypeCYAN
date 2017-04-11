using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PHarpoon : ProjectileBase {
    [SerializeField]
    float rotatingSpeed = 20;
    [SerializeField]
    float closeDistance = 0.5f;
    [SerializeField]
    Vector3 lineOffset;
    [SerializeField]
    float harpoonLength;
    [SerializeField]
    float thrustingLength;

    public Transform fixedTarget;
    public PhysicMaterialData fixedTargetPMat;
    public enum HARPOON_STATE { NONE, MOVING, FIXED, RETURNING, RETURNING_WITH_SOUL, RETURNING_WITH_OBJECT, FLYING, HANGING }
    public HARPOON_STATE harpoonState;
    
    public float turingSpeed=10.0f;
    public float turingaccel = 10.0f;

    public Transform hand;

    new Rigidbody rigidbody;

    Collider collider;

    public ChainDrawer chainDrawer;

    public bool AttachObjectToObject(Transform A,Transform B,bool isHingeJoint=false)
    {
        Rigidbody rigidA = A.GetComponent<Rigidbody>();
        Rigidbody rigidB = B.GetComponent<Rigidbody>();
        if (rigidA == null || rigidB == null)
        {
            return false;
        }
        Joint AJoint;
        if(!isHingeJoint)
            AJoint = A.gameObject.AddComponent<FixedJoint>();
        else
            AJoint = A.gameObject.AddComponent<HingeJoint>();

        AJoint.connectedBody = rigidB;
        return true;
    }
    public bool DettachObject(Transform A)
    {
        Joint AJoint = A.GetComponent<Joint>();
        if (AJoint == null)
        {
            return false;
        }
        Component.Destroy(AJoint);
        return true;
    }
    void Start()
    {
        collider = GetComponentInChildren<Collider>();
        rigidbody = GetComponent<Rigidbody>();
        AddOnTriggerHitEvent(
            delegate(Collider col)
            {
                //Debug.Log("h");
                //Collider col = coll.collider;
                if (col.gameObject == chainDrawer)
                    return;
                if (col.gameObject == owner)
                    return;
                if (harpoonState==HARPOON_STATE.MOVING)
                {   
                    objectMovement.Stop();
                    if (col.gameObject.tag == "Wood")
                    {
                        AttachObjectToObject(transform, col.transform);
                        rigidbody.isKinematic = true;
                        harpoonState = HARPOON_STATE.FIXED;
                        fixedTarget = col.transform;
                        /*if (col.attachedRigidbody != null)
                            chainDrawer.headConnectedRigid = col.attachedRigidbody;*/
                        fixedTargetPMat = new PhysicMaterialData()
                        {
                            dynamicFriction = col.material.dynamicFriction,
                            staticFriction = col.material.staticFriction,
                            frictionCombine = col.material.frictionCombine
                        };
                        RaycastHit hit;
                        if (Physics.Raycast(transform.position, transform.forward, out hit, harpoonLength * 3, LayerMask.GetMask(LayerMask.LayerToName(col.gameObject.layer))))
                        {
                            transform.position = hit.point - transform.forward * (thrustingLength);
                        }
                    }
                    else if (col.gameObject.tag == "Monster")
                    {
                        AttachObjectToObject(transform, col.transform);
                        rigidbody.isKinematic = true;
                        harpoonState = HARPOON_STATE.FIXED;
                        fixedTarget = col.transform;
                        /*if (col.attachedRigidbody != null)
                            chainDrawer.headConnectedRigid = col.attachedRigidbody;*/
                        fixedTargetPMat = new PhysicMaterialData()
                        {
                            dynamicFriction = col.material.dynamicFriction,
                            staticFriction = col.material.staticFriction,
                            frictionCombine = col.material.frictionCombine
                        };
                    }
                    else if (col.gameObject.layer == LayerMask.NameToLayer("ground"))
                    {
                        ReturnHarpoon(HARPOON_STATE.RETURNING);
                    }
                }
            }
        );
        objectMovement.AddReachedEvent(OnReachedEvent);
        gameObject.SetActive(false);
    }
    void OnTriggerExit(Collider col)
    {
        if(harpoonState!=HARPOON_STATE.MOVING&&harpoonState != HARPOON_STATE.RETURNING_WITH_OBJECT&& harpoonState != HARPOON_STATE.FIXED)
        {
            collider.isTrigger = false;
        }
    }
    public void ReturnHarpoon(HARPOON_STATE _type)
    {
        switch (_type)
        {
            case HARPOON_STATE.RETURNING:
                rigidbody.useGravity = true;
                rigidbody.isKinematic = false;
                rigidbody.constraints = RigidbodyConstraints.None;
                rigidbody.velocity = Vector3.zero;
                //collider.isTrigger = false;
                DettachObject(transform);
                //chainDrawer.headRigid = null;
                chainDrawer.StartCoroutine(chainDrawer.Trace(false));
                break;
            case HARPOON_STATE.RETURNING_WITH_SOUL:
                rigidbody.useGravity = true;
                rigidbody.isKinematic = false;
                rigidbody.velocity = Vector3.zero;
                rigidbody.constraints = RigidbodyConstraints.None;
                //collider.isTrigger = false;
                //DettachObject(transform);
                //AttachObjectToObject(fixedTarget, transform);
                chainDrawer.StartCoroutine(chainDrawer.Trace(false));
                break;
            case HARPOON_STATE.RETURNING_WITH_OBJECT:
                rigidbody.useGravity = true;
                rigidbody.isKinematic = false;
                rigidbody.constraints = RigidbodyConstraints.None;
                rigidbody.velocity = Vector3.zero;
                //DettachObject(transform);
                //AttachObjectToObject(fixedTarget, transform);
                chainDrawer.StartCoroutine(chainDrawer.Trace(false));
                Collider col = GetComponent<Collider>();
                if (col)
                {
                    col.material.dynamicFriction = collider.material.dynamicFriction;
                    col.material.staticFriction = collider.material.staticFriction;
                    col.material.frictionCombine = collider.material.frictionCombine;
                }
                break;
            case HARPOON_STATE.FLYING:
                chainDrawer.StartCoroutine(chainDrawer.Trace(true));
                rigidbody.constraints = RigidbodyConstraints.None;
                rigidbody.velocity = Vector3.zero;
                break;
            default:
                return;
        }
        harpoonState = _type;
        chainDrawer.SetOnReachedEvent(OnReachedEvent);

    }
    void OnReachedEvent()
    {
        switch (harpoonState)
        {
            case HARPOON_STATE.NONE:
                break;
            case HARPOON_STATE.MOVING:
                ReturnHarpoon(HARPOON_STATE.RETURNING);
                
                break;
            case HARPOON_STATE.RETURNING:
                ResetProjectile();
                owner.GetComponent<UnitInfo>().EquipWeapon(1);
                break;
            case HARPOON_STATE.FIXED:
                break;
            case HARPOON_STATE.RETURNING_WITH_SOUL:
                SoulInfo soulInfo = fixedTarget.GetComponent<SoulInfo>();
                if (soulInfo)
                {
                    if (soulInfo.buffType != BuffBehaviour.BuffType.None)
                    {
                        owner.GetComponent<UnitInfo>().AddBuff(soulInfo.buffType, soulInfo.buffTime);
                    }
                }
                //rigidbody.isKinematic = true;
                ResetProjectile();
                owner.GetComponent<UnitInfo>().EquipWeapon(1);
                break;
            case HARPOON_STATE.RETURNING_WITH_OBJECT:
                //rigidbody.isKinematic = true;
                DettachObject(fixedTarget);
                ResetProjectile();
                owner.GetComponent<UnitInfo>().EquipWeapon(1);
                break;
            case HARPOON_STATE.FLYING:
                //rigidbody.isKinematic = true;
                //ResetProjectile();
                harpoonState = HARPOON_STATE.HANGING;
                owner.transform.eulerAngles = new Vector3(0,owner.transform.eulerAngles.y, 0);
                break;
            default:
                break;
        }
    }
    public void ResetProjectile()
    {
        //transform.position = hand.position;
        rigidbody.constraints = RigidbodyConstraints.None;
        owner.GetComponent<UnitInfo>().EquipWeapon(1);
        transform.parent = null;
        rigidbody.velocity = Vector3.zero;
        rigidbody.useGravity = false;
        harpoonState = HARPOON_STATE.NONE;
        gameObject.SetActive(false);
        collider.isTrigger = true;
        
        if(fixedTargetPMat!=null)
        {
            Collider col = fixedTarget.GetComponent<Collider>();
            if(col)
            {
                col.material.dynamicFriction = fixedTargetPMat.dynamicFriction;
                col.material.staticFriction = fixedTargetPMat.staticFriction;
                col.material.frictionCombine = fixedTargetPMat.frictionCombine;
            }
        }
        chainDrawer.headConnectedRigid = GetComponent<Rigidbody>();
        chainDrawer.gameObject.SetActive(false);
        DettachObject(transform);
    }
    public HARPOON_STATE GetHarpoonState()
    {
        return harpoonState;
    }
    void Update()
    {
        if (!isToggle)
            return;
        if (harpoonState == HARPOON_STATE.MOVING)
        {
            transform.GetChild(0).localEulerAngles += new Vector3(0, 0, rotatingSpeed);
        }
        if(harpoonState >= HARPOON_STATE.RETURNING)
        {
            Vector3 dir = transform.position - owner.transform.TransformPoint(chainDrawer.tailConnectedOffset);

            RaycastHit hit;
            if (Physics.Raycast(transform.position,rigidbody.velocity.normalized,out hit, 3,LayerMask.GetMask("Unit","ground")))
            {
                if (hit.transform == owner.transform)
                    collider.isTrigger = true;
                else
                    collider.isTrigger = false;
            }
        }
    }
    public override void Shot(Vector3 _start, Vector3 _dest, Vector3 _euler, float _maxSpeed = 3, float _accelator = 0.1F, float _sSpeed = 3)
    {
        base.Shot(_start, _dest, _euler, _maxSpeed, _accelator, _sSpeed);
        harpoonState = HARPOON_STATE.MOVING;
        //rigidbody.isKinematic = true;
        collider.isTrigger = true;
        rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    }
    public override void SetTargetPosition(Vector3 _pos, float _maxSpeed = 3, float _accelator = 0.1F, float _speed = 3)
    {
        base.SetTargetPosition(_pos, _maxSpeed, _accelator, _speed);
    }
}
