using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[System.Serializable]
public class AIStateActionDragon : AIStateAction
{
    public CharacterMovement.MOVING_TYPE movingType;
}
[System.Serializable]
public class WeaknessPoint
{
    public Transform transform;
    public Vector3 appearTo;
}
public class AIDragon : AIBase
{
    UnitInfo unitInfo;
    ActionComponent actionComponent;
    CharacterMovement movement;
    GenericIK ik;

    public Transform pivot;
    

    enum ActionReadyType { none,Roar,Fly};
    ActionReadyType readyType;

    [Header("Unit Speed")]

    public float walkSpeed;
    public float sprintSpeed;
    public float flySpeed;

    public float turnSpeed=2.0f;


    public float lookSpeed=2.0f;

    [Header("Combat State")]

    public List<AIStateActionDragon> aiStateAction = new List<AIStateActionDragon>();
    public List<WeaknessPoint> weaknessPoints = new List<WeaknessPoint>();
    public EventObject weaknessPointObject;

    [Header("State On ground")]
    public float minMovingDistance;
    public float maxMovingDistance;
    [Header("State Flying")]
    public float minAirMovingDistance;
    public float maxAirMovingDistance;
    

    [Header("Detection set")]


    public Transform actionTarget;
    public Vector3 actionTargetMoveTo;
    
    

    float delayCheck;

    protected override void Start()
    {
        actionComponent = GetComponent<ActionComponent>();
        unitInfo = GetComponent<UnitInfo>();
        movement = GetComponent<CharacterMovement>();
        ik = unitInfo.GetComponent<GenericIK>();
        unitInfo.AddDamageEvent(delegate(UnitInfo _causer, float _damage)
        {
            if (unitInfo.GetHP()<=0)
                {
                    SetState(AIState.die);
                    return;
                }
                float[] roarHPs = { 75, 50, 25 };
                for(int i=0;i< roarHPs.Length;i++)
                    if(unitInfo.GetLastHP()> roarHPs[i]&& unitInfo.GetHP()<= roarHPs[i])
                    {
                        if(unitInfo.GetCondition()>=ConditionData.UnitCondition.stuned)
                            unitInfo.GetComponent<ActionComponent>().UseSkill("WakeUp", true);
                        readyType=ActionReadyType.Roar;
                        unitInfo.RemoveCrowdControlAll();
                }
            }
        );
        unitInfo.AddStateChangeEvent(delegate (UnitInfo.UnitState _new, UnitInfo.UnitState _old)
            {

            }
        );
        weaknessPointObject.AddEvent(
            delegate(Transform caller)
            {
                Debug.Log(caller);
                StartCoroutine(DisappearWeakness());
                unitInfo.AddCrowdControl(ConditionData.UnitCondition.stuned, 5,
                   delegate () 
                   {
                       unitInfo.GetComponent<ActionComponent>().UseSkill("WakeUp", true);
                       StartCoroutine(AppearWeakness(UnityEngine.Random.Range(0, weaknessPoints.Count)));
                   }
                   );
                actionComponent.StopAction();
                movement.Stop();
                movement.StopRotating();
            }
        );
        actionTarget = new GameObject(transform.name + " Action Target").transform;
        StartCoroutine(AppearWeakness(UnityEngine.Random.Range(0, weaknessPoints.Count)));
        base.Start();
        //actionTarget = pivot;
    }
    

    bool IsInRange(float value,float min,float max)
    {
        return (value >= min && value <= max);
    }

    IEnumerator AppearWeakness(int index)
    {
        weaknessPointObject.SetEnable(true);
        float speed=1.5f;
        weaknessPointObject.transform.SetParent(weaknessPoints[index].transform);
        weaknessPointObject.transform.localPosition = Vector3.zero;
        weaknessPointObject.transform.localEulerAngles = Vector3.zero;
        while (Vector3.Distance(weaknessPoints[index].appearTo, weaknessPointObject.transform.localPosition)>0.01f|| weaknessPointObject.transform.localScale.x<0.99f)
        {
            weaknessPointObject.transform.localScale = Vector3.Lerp(weaknessPointObject.transform.localScale, Vector3.one, Time.deltaTime * speed*2);
            weaknessPointObject.transform.localPosition = Vector3.MoveTowards(weaknessPointObject.transform.localPosition, weaknessPoints[index].appearTo, Time.deltaTime* speed);
            yield return null;
        }
        yield return null;
    }
    IEnumerator DisappearWeakness()
    {
        weaknessPointObject.SetEnable(false);
        float speed = 1.5f;
        while (weaknessPointObject.transform.localPosition.magnitude > 0.01f||weaknessPointObject.transform.localScale.magnitude > 0.1f)
        {
            weaknessPointObject.transform.localScale = Vector3.Lerp(weaknessPointObject.transform.localScale, Vector3.zero, Time.deltaTime * speed*2);
            weaknessPointObject.transform.localPosition = Vector3.Lerp(weaknessPointObject.transform.localPosition, Vector3.zero, Time.deltaTime * speed);
            yield return null;
        }
        yield return null;
    }

    protected override IEnumerator idle()
    {
        while (true)
        {

            if (targetUnit != null)
                SetState(AIState.combat, true);

            yield return null;
        }
    }
    protected override IEnumerator die()
    {
        ik.ToggleIK(false);
        yield return null;
    }
    protected override IEnumerator combat()
    {
        Vector3 forward = (targetUnit.transform.position - pivot.position);
        forward.y = 0;
        forward.Normalize();
        float targetAngle = Quaternion.Angle(transform.rotation, Quaternion.LookRotation(forward));

        float targetDistance = Vector3.Distance(pivot.position, actionTarget.transform.position);
        
        float delay = 0;
        
        float rotateDelay = 0;


        actionComponent.GetBehaviour("DragonDash").SetTarget(targetUnit.transform);
        actionComponent.GetBehaviour("DragonBreathAir").SetTarget(targetUnit.transform);
        ik.SetAllTarget(actionTarget);
        actionComponent.UseSkill("DragonRoar", true);
        
        while (true)
        {
            if (unitInfo.GetCondition() >= ConditionData.UnitCondition.stuned)
            {
                yield return null;
                continue;
            }
            forward = (targetUnit.transform.position - transform.position);
            forward.Normalize();
            targetAngle = Quaternion.Angle(transform.rotation, Quaternion.LookRotation(forward));
            if (delay > 0&&!actionComponent.IsUsingAction())
                delay = Mathf.Clamp(delay-Time.deltaTime,0,10);
            if(rotateDelay>0)
                rotateDelay=Mathf.Clamp(rotateDelay - Time.deltaTime, 0, 10);
            ActionBehaviour currentAction = actionComponent.GetUsingAction();
            if (readyType == ActionReadyType.Roar)
            {// POINT -- ACTION ETC
                if (currentAction == null || currentAction.GetActionName() != "WakeUp")
                {
                    actionComponent.UseSkill("DragonRoar", true);
                    readyType=ActionReadyType.Fly;
                    delay = 0.5f;
                }
            }
            else if (delay == 0 && readyType == ActionReadyType.Fly)
            {
                actionComponent.UseSkill("DragonFly", true);
                readyType = ActionReadyType.none;
                delay = 0.5f;
            }
            else if (delay == 0)
            {
                if(!weaknessPointObject.isEnable)
                {
                    StartCoroutine(AppearWeakness(UnityEngine.Random.Range(0, weaknessPoints.Count)));
                }
                bool isGroundMoving = movement.GetMovingType() == CharacterMovement.MOVING_TYPE.ground;
                if (!movement.isOnGround && isGroundMoving)
                {
                    yield return null;
                    continue;
                }
                if(isGroundMoving)
                    targetDistance = Vector3.Distance(transform.position, targetUnit.transform.position);
                else
                {
                    Vector2 p1 = new Vector2(transform.position.x, transform.position.z);
                    Vector2 p2 = new Vector2(targetUnit.transform.position.x, targetUnit.transform.position.z);
                    targetDistance = Vector2.Distance(p1, p2);
                }
                foreach (AIStateActionDragon sA in aiStateAction)
                {
                    //POINT -- ACTION
                    if (sA.enable&&sA.movingType== movement.GetMovingType()&&targetAngle < sA.angleLimit && !sA.behaviour.IsCoolDown() && IsInRange(targetDistance, sA.minDistance, sA.maxDistance))
                    {
                        movement.Stop();
                        movement.StopRotating();
                        actionComponent.UseSkill(sA.behaviour, false);
                        delay = 0.5f;
                        break;
                    }
                }
                if (!actionComponent.IsUsingAction()&&(!isGroundMoving || (isGroundMoving && movement.isOnGround)))
                {
                    float minD = (isGroundMoving) ? minMovingDistance : minAirMovingDistance;
                    float maxD = (isGroundMoving) ? maxMovingDistance : maxAirMovingDistance;
                    if (IsInRange(targetDistance, minD, maxD))
                    {
                        float speed = (isGroundMoving) ? walkSpeed : flySpeed;
                        //POINT -- moving
                        forward.y = 0;
                        movement.Move(forward, speed, 0.2f);
                        movement.RotateTo(forward, 5);
                        actionTarget.position = Vector3.Lerp(actionTarget.position, targetUnit.transform.position, Time.deltaTime * lookSpeed);
                    }
                    else
                    {
                        //POINT -- rotating
                        if (movement.IsMoving())
                        {
                            movement.Stop();
                            movement.StopRotating();
                        }
                        ik.ToggleIK(true);
                        float a = Quaternion.Angle(transform.rotation, Quaternion.LookRotation(forward));
                        Quaternion rot = Quaternion.LookRotation(forward);
                        forward.y = 0;
                        if (a > 40 && !movement.IsRotating() && rotateDelay == 0)
                        {
                            movement.RotateTo(forward, turnSpeed);
                            rotateDelay = 3;
                        }
                        if (a < 90)
                            actionTarget.position = Vector3.Lerp(actionTarget.position, targetUnit.transform.position, Time.deltaTime * lookSpeed);
                    }
                }
            }
            yield return null;
        }
    }
    
}
