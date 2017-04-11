using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Rigidbody))]
public class ProjectileBase : MonoBehaviour {

    public delegate void OnHitEvent(Collision _col);
    public delegate void OnTriggerHitEvent(Collider _col);

    protected OnHitEvent onHitEvent;
    protected OnTriggerHitEvent onTriggerHitEvent;

    [SerializeField]
    protected GameObject owner;

    protected Vector3 targetPosition;

    protected ObjectMovement objectMovement;
    
    [SerializeField]
    public bool isToggle=false;

    public float speed;

    protected void Awake()
    {
        objectMovement = GetComponent<ObjectMovement>();
    }

    public static ProjectileBase CreateProjectile(GameObject prefab,Vector3 _spawnPos,Vector3 _euler,Vector3 _targetPos,GameObject _owner)
    {
        ProjectileBase instance = Instantiate<GameObject>(prefab).GetComponent<ProjectileBase>();
        instance.transform.position=_spawnPos;
        instance.transform.eulerAngles = _euler;
        float s = instance.speed;
        instance.SetTargetPosition(_targetPos, s,s,s);
        instance.SetOwner(_owner);
        return instance;
    }
    public void AddOnHitEvent(OnHitEvent _event)
    {
        onHitEvent += _event;
    }
    public void AddOnTriggerHitEvent(OnTriggerHitEvent _event)
    {
        onTriggerHitEvent += _event;
    }

    public void RemoveOnHitEvent(OnHitEvent _event)
    {
        onHitEvent -= _event;
    }
    public void RemoveOnTriggerHitEvent(OnTriggerHitEvent _event)
    {
        onTriggerHitEvent -= _event;
    }
    void OnCollisionEnter(Collision _col)
    {
        if (!isToggle)
            return;
        if(onHitEvent!=null)
            onHitEvent(_col);
    }
    void OnTriggerEnter(Collider _col)
    {
        if (!isToggle)
            return;
        if (onTriggerHitEvent != null)
            onTriggerHitEvent(_col);
    }
    public void SetOwner(GameObject _ownerObject)
    {
        owner = _ownerObject;
    }
    public GameObject GetOwner()
    {
        return owner;
    }
    public virtual void Shot(Vector3 _start,Vector3 _dest, Vector3 _euler,float _maxSpeed = 3.0f, float _accelator = 0.1f, float _sSpeed = 3.0f)
    {
        transform.position = _start;
        transform.eulerAngles = _euler;
        SetTargetPosition(_dest, _maxSpeed, _accelator, _sSpeed);
    }
    public virtual void SetTargetPosition(Vector3 _pos, float _maxSpeed = 3.0f, float _accelator = 0.1f, float speed = 3.0f)
    {
        isToggle = true;
        targetPosition = _pos;
        objectMovement.MoveTo(_pos, _maxSpeed, _accelator, speed);
    }
    public void ToggleProjectile(bool _toggle)
    {
        isToggle = _toggle;
        if(!isToggle)
            objectMovement.Stop();
    }
    public virtual void SetReachedEvent()
    {
        objectMovement.AddReachedEvent(delegate() { GameObject.Destroy(gameObject); });
    }
}
