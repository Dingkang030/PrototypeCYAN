using UnityEngine;
using System.Collections;

public abstract class ActionBehaviour : MonoBehaviour
{
    public enum ActionType
    {
        none=-1,basicAttack,throwBoomerang,roll
    }
    
    [SerializeField]
    protected float coolDown;
    protected float coolDownCheck;
    [SerializeField]
    protected float damage;
    protected Animator animator;
    protected int behaviourState;
    protected bool eventToggle;

    protected UnitInfo unitInfo;
    bool isPlaying;

    protected Transform targetObject;

    protected ActionComponent actionComponent;
    

    protected delegate void OnStopAction();
    OnStopAction onStopActionEvent;

    public abstract string GetActionName();

    protected string behaviourMessage;

    protected virtual void Start()
    {
        unitInfo = transform.GetComponentInParent<UnitInfo>();
        actionComponent = unitInfo.transform.GetComponent<ActionComponent>();
        animator = unitInfo.transform.GetComponent<Animator>();
    }
    
    public void SetTarget(Transform _target)
    {
        targetObject = _target;
    }

    public virtual bool PlayAction()
    {
        if (!isPlaying&& coolDownCheck <= 0)
        {
            SetBehaviourState(1);
            StartCoroutine("BehaviourCoroutine");
            StartCoroutine("CoolDownCoroutine");
            isPlaying = true;
            return true;
        }
        return false;
    }

    public bool IsPlaying()
    {
        return isPlaying;
    }

    public virtual void StopAction()
    {
        if (isPlaying)
        {
            StopCoroutine("BehaviourCoroutine");
            SetBehaviourState(0);
            eventToggle = false;
            isPlaying = false;
            unitInfo.ChangeState(UnitInfo.UnitState.idle);
            behaviourMessage = "";
            if (onStopActionEvent != null)
                onStopActionEvent();
        }
    }
    
    protected abstract IEnumerator BehaviourCoroutine();
    public void SetBehaviourState(int _state) { behaviourState = _state; } // behaviour에서 state별로 동작을 나누기위함
    public void IncreaseBehaviourState() { behaviourState++; } // behaviour에서 state별로 동작을 나누기위함
    public void ToggleBehaviourEvent() { eventToggle = true; }
    public void SendBeahaviourMessage(string _msg) { behaviourMessage = _msg; }
    public void ResetBehaviourEvent() { eventToggle = false; }
    public int GetBehaviourState() { return behaviourState; }
    protected class WaitForStateChange : CustomYieldInstruction
    {
        int toState;
        ActionBehaviour behaviour;
        public WaitForStateChange(ActionBehaviour _behaviour, int _toState)
        {
            behaviour = _behaviour;
            toState = _toState;
        }
        public override bool keepWaiting
        {
            get
            {
                return (behaviour.GetBehaviourState() < toState);
            }
        }
    }
    protected class WaitForMessageReceiving : CustomYieldInstruction
    {
        string message;
        ActionBehaviour behaviour;
        public WaitForMessageReceiving(ActionBehaviour _behaviour, string _msg)
        {
            behaviour = _behaviour;
            message = _msg;
        }
        public override bool keepWaiting
        {
            get
            {
                return (message!=behaviour.behaviourMessage);
            }
        }
    }
    public bool IsCoolDown() { return coolDownCheck > 0; }
    protected IEnumerator CoolDownCoroutine()
    {
        coolDownCheck = coolDown;
        while (coolDownCheck > 0)
        {
            coolDownCheck -= Time.deltaTime;
            yield return null;
        }
        coolDownCheck = 0;
    }
    protected void SetStopActionEvent(OnStopAction _event)
    {
        onStopActionEvent = _event;
    }
}
