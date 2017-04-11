using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class AIStateAction
{
    public ActionBehaviour behaviour;
    public float minDistance;
    public float maxDistance;
    public float angleLimit;
    public float delay;
    public bool isForced;
    public bool enable=true;
}

public abstract class AIBase : MonoBehaviour {

    public enum AIState { idle, combat, die };
    [SerializeField]
    protected bool play;
    [SerializeField]
    protected AIState currentState;
    [Header("Target Finder")]
    [SerializeField]
    float searchingDistance;
    [SerializeField]
    [Range(0,180)]
    float searchingDegree;

    protected UnitInfo targetUnit;


    protected virtual void Start()
    {
        SetState(currentState,true);
    }
    public void SetTargetUnit(UnitInfo unit)
    {
        targetUnit = unit;
    }
    protected void SetState(AIState newState,bool force=false)
    {
        if (currentState != newState||force)
        {
            StopCoroutine(currentState.ToString());
            StartCoroutine(newState.ToString());
            currentState = newState;
        }
    }
    protected abstract IEnumerator idle();
    protected abstract IEnumerator combat();
    protected abstract IEnumerator die();
    protected UnitInfo[] SearchTarget()
    {
        List<UnitInfo> infos=new List<UnitInfo>();
        return infos.ToArray();
    }
}
