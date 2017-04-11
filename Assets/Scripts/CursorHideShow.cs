using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorHideShow : MonoBehaviour {

    bool islocked;
	void Start () {
        SetCursorLock(true);

    }
	
    void SetCursorLock(bool _isLocked)
    {
        islocked = _isLocked;
        Cursor.lockState = (_isLocked==true)?CursorLockMode.Locked:CursorLockMode.Confined;
        Cursor.visible = !islocked;
    }

	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
            SetCursorLock(false);
	}
}
