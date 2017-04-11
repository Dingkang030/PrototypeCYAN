using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffector : MonoBehaviour {

    static HitEffector instance;
    public float timeScale=0.01f;
    float checkedTime = 0;
    void Start ()
    {
        if (instance == null)
            instance = FindObjectOfType<HitEffector>();
    }
    void Awake()
    {
        if(instance==null)
            instance = this;
    }

    static public HitEffector Instance
    {
        get { return instance; }
    }

    public void StopFrameEffect(float _time=0.3f)
    {
        if (checkedTime <= 0)
        {
            Time.timeScale = timeScale;
        }
        checkedTime += _time;
    }
    void Update()
    {
        if (checkedTime>0)
        {
            checkedTime -= Time.unscaledDeltaTime;
            if (checkedTime <= 0)
            {
                Time.timeScale = 1.0f;
                checkedTime = 0;
            }
        }
    }
	/*void Update () {
        if (checkedFrame > 0)
        {
            checkedFrame--;
            if (checkedFrame == 0)
                Time.timeScale = 1.0f;
        }
	}*/
}
