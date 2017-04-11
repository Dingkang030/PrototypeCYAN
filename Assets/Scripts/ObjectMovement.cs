using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMovement : MonoBehaviour {

    public float speed;
    public float accelator;
    public float maxSpeed;
    public Vector3 destPosition;
    public bool isMoving = false;


    public bool isRadialMoving;
    public float rotatedAmount;
    float rSpeed;
    public Vector3 circlePoint;
    public Vector3 circleNormal;

    public delegate void OnReachedEvent();
    public OnReachedEvent reachedEvent;

    public new Rigidbody rigidbody;
    
	void Start () {
        rigidbody = GetComponent<Rigidbody>();
	}
	public void AddReachedEvent(OnReachedEvent _reachedEvent)
    {
        reachedEvent += _reachedEvent;
    }
    public void RemoveReachedEvent(OnReachedEvent _reachedEvent)
    {
        reachedEvent -= _reachedEvent;
    }
    public void MoveTo(Vector3 _destination,float _maxSpeed,float _accel,float _speed=-1)
    {
        if (_speed != -1)
            speed = _speed;
        accelator = _accel;
        maxSpeed = _maxSpeed;
        destPosition = _destination;
        isMoving = true;
    }
    public void RadialMoveTo(Vector3 _destination, float _speed)
    {
        isMoving = false;
        destPosition = _destination;
        isRadialMoving = true;
        float distance = Vector3.Distance(transform.position, _destination);
        circlePoint = transform.position + (_destination - transform.position).normalized * (distance * 0.5f);
        rotatedAmount = 0.0f;
        rSpeed = _speed;
        
    }
    void UpdateAccelator()
    {
        if (speed <= maxSpeed)
        {
            speed += accelator;
            if (speed > maxSpeed)
                speed = maxSpeed;
        }
    }
    void UpdateMovement()
    {
        if(isMoving)
        {
            Vector3 newPosition;
            newPosition = Vector3.MoveTowards(transform.position, destPosition, speed * Time.deltaTime);
            if (rigidbody && rigidbody.isKinematic == false)
                rigidbody.position = newPosition;
            else
            {
                transform.position = newPosition;
            }
            if(Vector3.Distance(transform.position, destPosition)<=0.1f)
            {
                Stop();
                OnReached();
            }
        }
    }
    void UpdateRadialMovement()
    {
        if (isRadialMoving)
        {
            float radius = Vector3.Distance(circlePoint, destPosition);
            Vector3 forward = -(destPosition - circlePoint).normalized;
            rotatedAmount += rSpeed * Time.deltaTime;
            if (rSpeed > 0&& rotatedAmount > 180)
                rotatedAmount = 180;
            else if(rSpeed<0 && rotatedAmount < -180)
                rotatedAmount = -180;

            /*Quaternion newRotation = Quaternion.RotateTowards(start, dest, rotatedAmount);
            forward = newRotation * forward;*/
            Vector3 newForward = Vector3.RotateTowards(forward, -forward, rotatedAmount*Mathf.Deg2Rad, 1.0f);
            transform.position = circlePoint + newForward * radius;
            if (rotatedAmount == 180||rotatedAmount==-180)
            {
                Stop();
                OnReached();
            }
        }
    }
    void OnReached()
    {
        if(reachedEvent!=null)
            reachedEvent();
    }
    public void Stop()
    {
        speed = 0;
        isMoving = false;
        isRadialMoving = false;
        rSpeed = 0;
    }
	void Update () {
        UpdateAccelator();
        UpdateMovement();
        UpdateRadialMovement();
    }
}
