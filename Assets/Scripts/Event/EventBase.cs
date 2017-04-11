using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EventBase : MonoBehaviour {

    public bool isPlaying;
    protected UnitInfo eventPlayer;
    void OnTriggerEnter(Collider col)
    {
        if (isPlaying)
            return;
        if(col.tag=="Player")
        {
            PlayEvent(col.GetComponent<UnitInfo>());
        }
    }

    void PlayEvent(UnitInfo _player=null)
    {
        if (isPlaying)
            return;
        isPlaying = true;
        eventPlayer = _player;
        StartCoroutine("EventCoroutine");
    }

    void StopEvent()
    {
        if (!isPlaying)
            return;
        isPlaying = false;
        StopCoroutine("EventCoroutine");
    }

    protected abstract IEnumerator EventCoroutine();
    
}
