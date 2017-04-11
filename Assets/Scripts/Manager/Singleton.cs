using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour {


    static T instance;
    
    virtual protected void Awake()
    {
        instance = GetComponent<T>();
        DontDestroyOnLoad(this);
    }

    public static T Instance
    {
        get { return instance; }
    }
    

}
