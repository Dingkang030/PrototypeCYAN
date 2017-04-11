using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public abstract class BuffBehaviour
{
    public enum BuffType { None,Fly };
    public BuffType buffType;
    public float buffTime;
    public static BuffBehaviour GetBuff(BuffType _type, float _time)
    {
        switch (_type)
        {
            case BuffType.Fly:
                return new BuffFlying(_time);
            default:
                return null;
        }
    }
    protected BuffBehaviour(float _buffTime) { buffTime = _buffTime; }
    public abstract void AddBuffAction(UnitInfo _unit);
    public abstract IEnumerator BuffCoroutine(UnitInfo owner);
}
[System.Serializable]
public class BuffFlying : BuffBehaviour
{
    public BuffFlying(float _buffTime) : base(_buffTime)
    {
        buffType = BuffType.Fly;
    }

    public override void AddBuffAction(UnitInfo _unit)
    {
        GameObject buffActionPrefab = Resources.Load<GameObject>("Actions/AFlying");
        Transform newParent = _unit.GetAction().GetActionList()[0].transform.parent;
        ActionBehaviour newSkill = GameObject.Instantiate(buffActionPrefab, newParent).GetComponent<ActionBehaviour>();
        _unit.GetAction().GetActionList().Add(newSkill);
        newSkill.transform.localPosition = Vector3.zero;
    }
    public override IEnumerator BuffCoroutine(UnitInfo owner)
    {
        AddBuffAction(owner);
        Rigidbody rigidbody = owner.GetComponent<Rigidbody>();
        float buffTimeCount = buffTime;
        do
        {
            if (!owner.GetMovement().isOnGround)
            {
                Vector3 newVelocity = rigidbody.velocity;
                if (newVelocity.y < 0)
                {
                    /*newVelocity.y *= 0.8f;
                    rigidbody.velocity = newVelocity;*/
                }
            }
            buffTimeCount -= Time.deltaTime;
            yield return null;
        } while (buffTimeCount > 0);
        ActionBehaviour aBehaviour=owner.GetAction().GetBehaviour("Flying");
        if (aBehaviour)
        {
            owner.GetAction().RemoveAction(aBehaviour);
            GameObject.Destroy(aBehaviour.gameObject);
        }
        owner.RemoveBuff(this);
    }
}