using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ActionComponent : MonoBehaviour {
    

    [SerializeField]
    List<ActionBehaviour> actionList=new List<ActionBehaviour>();
    int usingActionIndex;
    UnitInfo unitInfo;
    [SerializeField]
    bool stiffOnHit=true;
    public List<ActionBehaviour> GetActionList() { return actionList; }
    [SerializeField]
    Weapon weapon;
    [SerializeField]
    Transform weaponSlot;
    [SerializeField]
    Transform weaponBone;

    void Start () {
        usingActionIndex = -1;
        unitInfo = GetComponent<UnitInfo>();
       /* if(stiffOnHit)
            unitInfo.AddDamageEvent(DamageEventFunc);// 테스트용으로 넣음 맞을시 경직*/
        if(actionList.Count==0)
        {
            for(int i=0;i<transform.childCount;i++)
            {
                if(transform.GetChild(i).name=="ActionList")
                {
                    ActionBehaviour[] aList = transform.GetChild(i).GetComponentsInChildren<ActionBehaviour>();
                    foreach (ActionBehaviour a in aList)
                        actionList.Add(a);
                }
            }
        }
    }

    public Weapon currentWeapon
    {
        get { return weapon; }
    }

    public bool IsUsableWeapon(string _name)
    {
        return unitInfo.currentWeapon.weaponName==_name;
    }

    public void DamageEventFunc(UnitInfo _causer, float _damage)
    {// 테스트용으로 넣음 맞을시 경직
        if (_damage >= 10)
        {
            StopAction();
            unitInfo.AddCrowdControl(ConditionData.UnitCondition.stiffen, 0.5f);
        }
    }
    public bool IsUsingAction()
    {
        return usingActionIndex != -1;
    }
    public bool IsUsingAction(string _type)
    {
        if (usingActionIndex == -1)
            return false;
        return GetUsingAction().GetActionName()==_type;
    }
    public bool UseSkill(ActionBehaviour _behaviour, bool forced = false)
    {
        if (forced == false && usingActionIndex != -1)
            return false;
        int index = actionList.FindIndex(get => get == _behaviour);
        if (index < 0)
            return false;
        if (forced && actionList[usingActionIndex]!=_behaviour)
            StopAction();
        if (actionList[index].PlayAction())
        {
            usingActionIndex = index;
            return true;
        }
        return false;
    }
    public ActionBehaviour GetBehaviour(string _name)
    {
        int index = actionList.FindIndex(get => get.GetActionName() == _name);
        if (index > 0)
            return actionList[index];
        return null;
    }
    public bool UseSkill(string _name, bool forced = false)
    {
        int index = actionList.FindIndex(get => get.GetActionName() == _name);
        if (index < 0)
            return false;
        return UseSkill(index, forced);
    }
    public bool UseSkill(int index,bool forced=false)
    {
        if (forced == false&&usingActionIndex != -1)
            return false;
        if (forced && usingActionIndex != index)
            StopAction();
        if(actionList[index].PlayAction())
        {
            usingActionIndex = index;
            return true;
        }
        return false;
    }
    public ActionBehaviour GetUsingAction()
    {
        if (usingActionIndex == -1)
            return null;
        return actionList[usingActionIndex];
    }
    public int behaviourState
    {
        get
        {
            return GetUsingAction().GetBehaviourState();
        }
    }
    public void RemoveAction(ActionBehaviour _behaviour)
    {
        int index = actionList.FindIndex(b => b == _behaviour);
        if (usingActionIndex != -1)
        {
            if (usingActionIndex == index)
            {
                StopAction();
                actionList.RemoveAt(index);
            }
        }
    }
    void Update()
    {
        if (usingActionIndex != -1)
            if (!actionList[usingActionIndex].IsPlaying())
                usingActionIndex = -1;
    }
    void LateUpdate()//삭제필
    {
        if (weapon != null)
            if (weapon.gameObject.activeSelf)
            {
                weaponSlot.position = weaponBone.position;
                weaponSlot.eulerAngles = weaponBone.eulerAngles;
            }
    }
    public void SendBehaviourMessage(string _msg)
    {
        if (usingActionIndex != -1)
        {
            if (actionList[usingActionIndex].IsPlaying())
            {
                GetUsingAction().SendBeahaviourMessage(_msg);
            }
        }
    }
    public void IncreaseBehaviourState(string actionName) // 스킬Behaviour가 스테이트별로 동작 다르게 하기위함.( 애니메이션 이벤트 용도 )
    {
        if (usingActionIndex != -1)
        {
            if (actionList[usingActionIndex].IsPlaying())
            {
                if(actionList[usingActionIndex].GetActionName()==actionName)
                    actionList[usingActionIndex].IncreaseBehaviourState();
            }
        }
    }

    public void ToggleBehaviourEvent(string actionName)
    {
        if (usingActionIndex != -1)
        {
            if (actionList[usingActionIndex].IsPlaying())
            {
                if (actionList[usingActionIndex].GetActionName() == actionName)
                {
                    actionList[usingActionIndex].ToggleBehaviourEvent();
                }
            }
        }
    }
    public void ResetBehaviourEvent(string actionName)
    {
        if (usingActionIndex != -1)
        {
            if (actionList[usingActionIndex].IsPlaying())
            {
                if (actionList[usingActionIndex].GetActionName() == actionName)
                {
                    actionList[usingActionIndex].ResetBehaviourEvent();
                }
            }
        }
    }

    public void StopAction()
    {
        if (usingActionIndex != -1)
            actionList[usingActionIndex].StopAction();
        usingActionIndex = -1;
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.transform.tag=="EventObject")
        {
            Debug.Log(col.transform);
            col.transform.GetComponent<EventObject>().ExecuteEvent(transform);
        }
    }
}
