using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ConditionData
{
    public enum UnitCondition { none, slowed, stiffen, stuned, knockBack }
    public UnitCondition condition;
    public float conditionTIme;
    public delegate void OnConditionChangeEvent();
    public OnConditionChangeEvent onConditionChangeEvent;
}
[System.Serializable]
public class UnitInfo : MonoBehaviour
{
    public enum UnitState { idle, action, die }
    UnitState unitState;
    List<ConditionData> unitConditionList = new List<ConditionData>();
    [SerializeField]
    float currentHP;
    float lastHP;
    [SerializeField]
    float maxHP = 100;
    [SerializeField]
    float defaultInvincibleTime = 0.5f;
    float invincibleTime;
    Animator animator;
    ActionComponent action;
    CharacterMovement movement;

    public bool isTargetingAble = true;
    [SerializeField]
    public Sprite unitSplashArt;
    [SerializeField]
    public string unitName;
    List<BuffBehaviour.BuffType> buffTypeList = new List<BuffBehaviour.BuffType>();
    List<BuffBehaviour> buffList = new List<BuffBehaviour>();
    [SerializeField]
    List<Weapon> weaponList = new List<Weapon>();
    [SerializeField]
    int currentWeaponIndex = 0;


    public delegate void OnTakeDamageEvent(UnitInfo _causer, float _damage);
    public delegate void OnStateChangeEvent(UnitState _newState, UnitState _oldState);
    OnTakeDamageEvent onTakeDamageEvent;
    OnStateChangeEvent onStateChangeEvent;
    void Awake()
    {
    }
    void Start()
    {
        movement = GetComponent<CharacterMovement>();
        animator = GetComponent<Animator>();
        currentHP = maxHP;
        action = GetComponent<ActionComponent>();
        UnitManager.Instance.AddUnit(this);
    }

    #region STATE FUNCTION
    public void ChangeState(UnitState _newState)
    {
        UnitState _oldState = unitState;
        unitState = _newState;
        OnStateChange(_newState, _oldState);
        if (animator)
            animator.SetInteger("state", (int)_newState);
    }
    public UnitState GetState()
    {
        return unitState;
    }
    protected virtual void OnStateChange(UnitState _newState, UnitState _oldState)
    {
        if (_newState == UnitState.idle)
        {
            if (GetHP() <= 0)
            { ChangeState(UnitState.die); return; }
        }
        if (onStateChangeEvent != null)
            onStateChangeEvent(_newState, _oldState);
    }
    #endregion
    #region GETTER FUNCTION
    public ActionComponent GetAction()
    {
        return action;
    }
    public Animator GetAnimator()
    {
        return animator;
    }
    public CharacterMovement GetMovement()
    {
        return movement;
    }
    #endregion
    #region HP FUNCTION
    public void SetHP(float _hp)
    {
        lastHP = currentHP;
        currentHP = _hp;
        if (currentHP > maxHP)
            currentHP = maxHP;
        else if (currentHP < 0)
            currentHP = 0;
    }
    public float GetHP()
    {
        return currentHP;
    }
    public float GetLastHP()
    {
        return lastHP;
    }
    public float GetMaxHP()
    {
        return maxHP;
    }
    public void AddDamageEvent(OnTakeDamageEvent _event)
    {
        onTakeDamageEvent += _event;
    }
    public void AddStateChangeEvent(OnStateChangeEvent _event)
    {
        onStateChangeEvent += _event;
    }
    public void RemoveDamageEvent(OnTakeDamageEvent _event)
    {
        onTakeDamageEvent -= _event;
    }
    public bool GiveDamage(UnitInfo _target, float _damage, Vector3 _damagePoint,float _invincibleTime=-1)
    {
        return _target.OnTakeDamage(this, _damage, _damagePoint,_invincibleTime);
    }
    public bool OnTakeDamage(UnitInfo _causer, float _damage, Vector3 _damagePoint, float _invincibleTime = -1)
    {
        if (invincibleTime!=-999&&invincibleTime <= 0)
        {
            SetHP(currentHP - _damage);
            invincibleTime = (_invincibleTime == -1) ? defaultInvincibleTime : _invincibleTime;
            if (onTakeDamageEvent!=null)
                onTakeDamageEvent(_causer, _damage);
            if (currentHP <= 0)
                ChangeState(UnitState.die);
            return true;
        }
        return false;
    }

    public bool IsInvincibleTIme()
    {
        return (invincibleTime > 0);
    }
    public void SetInvincibleTime(float _time)
    {
        if (invincibleTime == -999)
            return;
        invincibleTime = _time;
    }
    public void ToggleInvincible(bool toggle)
    {
        invincibleTime = (toggle) ? -999 : 0;
    }

    #endregion
    #region CC FUNCTION
    public void RemoveCrowdControl(ConditionData.UnitCondition _type)
    {
        ConditionData removeItem=null;
        foreach (ConditionData c in unitConditionList) // 아니라면 foreach를 돌림
            if (c.condition == _type) // 만약 군중제어상태가 같은 것이 있다면
            {
                removeItem = c;
                break;
            }
        if (removeItem != null)
        {
            unitConditionList.Remove(removeItem);
            animator.SetInteger("condition", (int)GetCondition());
        }
    }
    public void RemoveCrowdControlAll()
    {
        unitConditionList.Clear();
        animator.SetInteger("condition", 0);
    }
    public void AddCrowdControl(ConditionData.UnitCondition _type,float _ccTime=1.0f,ConditionData.OnConditionChangeEvent _conditionEvent=null) // 군중제어상태를 추가
    {
        ConditionData condition=new ConditionData(); // 컨디션데이터를만들고
        condition.condition = _type; // 채워줌
        condition.conditionTIme = _ccTime;
        condition.onConditionChangeEvent = _conditionEvent;
        foreach (ConditionData c in unitConditionList) // 아니라면 foreach를 돌림
            if (c.condition == _type) // 만약 군중제어상태가 같은 것이 있다면
                if (c.conditionTIme < _ccTime) // 시간을 체크후
                {
                    c.conditionTIme = _ccTime; // 더 큰시간으로 설정해줌
                    return;
                }
        unitConditionList.Add(condition); // 없다면 새로추가
        ConditionData topCondition = GetTopCondition();
        animator.SetInteger("condition", (int)topCondition.condition);
    }

    void UpdateCondition(float deltaTime)
    { // 군중제어기시간 체크용 업데이트문
        ConditionData.UnitCondition bigCondition=ConditionData.UnitCondition.none;
        List<ConditionData> removes = new List<ConditionData>();
        if (unitConditionList.Count > 0)
        {
            foreach (ConditionData c in unitConditionList)
            {
                if (c.conditionTIme <= deltaTime)
                {
                    removes.Add(c);
                    if(c.onConditionChangeEvent!=null)
                        c.onConditionChangeEvent();
                }
                else
                {
                    c.conditionTIme -= deltaTime;
                    if (bigCondition < c.condition)
                    {
                        bigCondition = c.condition;
                    }
                }
            }
            if (removes.Count > 0)
            {
                foreach (ConditionData c in removes)
                    unitConditionList.Remove(c);
                ConditionData topCondition = GetTopCondition();
                if (topCondition != null)
                    animator.SetInteger("condition", (int)topCondition.condition);
                else
                    animator.SetInteger("condition", 0);
            }
        }
    }
    public ConditionData.UnitCondition GetCondition()
    {
        ConditionData c = GetTopCondition();
        if (c == null)
            return ConditionData.UnitCondition.none;
        return c.condition;
    }
    public ConditionData GetTopCondition()
    {
        /*
         * 가장 우선순위가 높은 군중제어상태를 찾아줌
         */ 
        if (unitConditionList.Count == 0)
            return null;
        ConditionData bigCondition=null;
        foreach (ConditionData c in unitConditionList)
            if (bigCondition==null||c.condition > bigCondition.condition)
                bigCondition = c;
        return bigCondition;
    }
    #endregion
    #region BUFF FUNCTION
    public void RemoveBuff(BuffBehaviour _buffBehaviour)
    {
        buffList.Remove(_buffBehaviour);
        buffTypeList.Remove(_buffBehaviour.buffType);
    }
    public bool IsHaveBuff(BuffBehaviour.BuffType _type)
    {
        if (buffList.Count == 0)
            return false;
        if (buffList.FindIndex(buff => buff.buffType == _type) >= 0)
            return true;
        return false;
    }
    public void AddBuff(BuffBehaviour.BuffType _type, float time = -1)
    {
        AddBuff(BuffBehaviour.GetBuff(_type, time));
    }
    public void AddBuff(BuffBehaviour _buffBehaviour)
    {
        if (_buffBehaviour == null)
            return;
        BuffBehaviour b=null;
        if (buffList.Count > 0)
        {
            int index = buffList.FindIndex(buff => buff.buffType == _buffBehaviour.buffType);
            if(index>=0)
                b = buffList[index];
        }
        if (b==null)
        {
            buffList.Add(_buffBehaviour);
            buffTypeList.Add(_buffBehaviour.buffType);
            //_buffBehaviour.AddBuffAction(this);
            StartCoroutine(_buffBehaviour.BuffCoroutine(this));
        }
    }
    #endregion
    #region WEAPON FUNCTION
    public Weapon currentWeapon { get { if (currentWeaponIndex == -1) return null; else return weaponList[currentWeaponIndex]; } }
    public bool ChangeWeapon(bool right)
    {
        if (weaponList.Count == 0)
            return false;
        currentWeapon.gameObject.SetActive(false);
        int newIndex;
        if (right)
            newIndex = (currentWeaponIndex == weaponList.Count - 1) ? 0 : currentWeaponIndex++;
        else newIndex = (currentWeaponIndex == 0) ? weaponList.Count - 1 : currentWeaponIndex--;
        currentWeaponIndex = newIndex;
        currentWeapon.gameObject.SetActive(true);
        return true;
    }
    public bool EquipWeapon(int weaponIndex)
    {
        if(currentWeapon!=null)
        {
            if (currentWeaponIndex != weaponIndex)
            {
                if (weaponIndex < weaponList.Count)
                {
                    currentWeapon.gameObject.SetActive(false);
                    currentWeaponIndex = weaponIndex;
                    currentWeapon.gameObject.SetActive(true);
                    return true;
                }
            }
        }
        return false;
    }
    #endregion
    #region ETC
    void Update()
    {
        if (invincibleTime > 0 && invincibleTime != -999)
        {
            invincibleTime -= Time.deltaTime;
            if (invincibleTime < 0)
                invincibleTime = 0;
        }
    }
    void FixedUpdate()
    {
        UpdateCondition(Time.fixedDeltaTime);
    }
    #endregion
}
