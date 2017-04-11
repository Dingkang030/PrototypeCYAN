using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventObject : MonoBehaviour {


    public delegate void OnEventCalled(Transform caller);

    OnEventCalled calledEvent;
    public bool isEnable;

    public void SetEnable(bool _enable)
    {
        isEnable = _enable;
    }
    public void ExecuteEvent(Transform caller)
    {
        if (isEnable && calledEvent != null)
            calledEvent(caller);
    }
    void OnTriggerEnter(Collider col)
    {
        if (isEnable&&calledEvent != null)
            calledEvent(col.transform);
    }
	
    public void AddEvent(OnEventCalled _event)
    {
        calledEvent += _event;
    }
    
}
