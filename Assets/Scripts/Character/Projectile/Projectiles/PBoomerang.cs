using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PBoomerang : ProjectileBase {
    [SerializeField]
    float rotatingSpeed = 20;
    [SerializeField]
    float closeDistance = 0.5f;
    [SerializeField]
    Vector3 baseOffset;
    bool returning = false;
    
    public float turingSpeed=10.0f;
    public float turingaccel = 10.0f;

    void Start()
    {
        AddOnTriggerHitEvent(BoomerangHitEvent);
    }
    public override void SetReachedEvent()
    {
        objectMovement.AddReachedEvent(OnReachedEvent);
    }
    void OnReachedEvent()
    {
        if(!returning)
        {
            returning = true;
            objectMovement.MoveTo(owner.transform.position + baseOffset, speed, speed, speed);
        }
        else
        {
            owner.GetComponent<ActionComponent>().UseSkill("CatchBoomerang", true);
            GameObject.Destroy(gameObject);
        }
    }
    void Update()
    {
        transform.GetChild(0).localEulerAngles += new Vector3(0, rotatingSpeed, 0);
        if (returning)
        {
            objectMovement.MoveTo(owner.transform.position + baseOffset, speed, speed, speed);
        }
    }
    public override void SetTargetPosition(Vector3 _pos, float _maxSpeed = 3, float _accelator = 0.1F, float _speed = 3)
    {
        base.SetTargetPosition(_pos, _maxSpeed, _accelator, _speed);
        objectMovement.MoveTo(_pos, _maxSpeed, _accelator,_speed);
    }
    List<Transform> hittedObjects=new List<Transform>();
    void BoomerangHitEvent(Collider col)
    {
        if(col.transform!=owner.transform)
        {
            if (col.gameObject.layer == LayerMask.NameToLayer("Ignore Ground") ||col.gameObject.layer==LayerMask.NameToLayer("Unit")|| col.gameObject.layer == LayerMask.NameToLayer("ground")|| col.gameObject.layer == LayerMask.NameToLayer("eventObject"))
            {
                if (hittedObjects.Find(get => get == col.transform) == null)
                {
                    hittedObjects.Add(col.transform);
                    GameObject effect = Instantiate<GameObject>(Resources.Load<GameObject>("Effects/P_EF_Attack"));
                    effect.transform.position = col.ClosestPointOnBounds(transform.position);
                    Destroy(effect, 3.0f);
                }
            }
            returning = true;
            objectMovement.MoveTo(owner.transform.position + baseOffset, speed, speed, speed);
            if(col.gameObject.layer==LayerMask.NameToLayer("eventObject"))
            {
                col.gameObject.GetComponent<EventObject>().ExecuteEvent(owner.transform);
            }
        }
    }
}
