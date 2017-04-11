using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {
    [HideInInspector]
    public Transform rotator;
    [HideInInspector]
    public Camera cam;
    [SerializeField]
    float offSet_Y;
    [SerializeField]
    public Transform owner;
    [SerializeField]
    float camSpeed=3.0f;
    [SerializeField]
    Vector2 mouseSensitive = Vector2.one;
    public enum MOVING_TYPE
    {
        Linear, Lerp, Radial
    }
    public enum UPDATE_TYPE
    {
        Update,LateUpdate,FixedUpdate
    }
    public UPDATE_TYPE updateType;
    public MOVING_TYPE movingType;

    [SerializeField]
    bool lockXRotation;
    [SerializeField]
    bool lockYRotation;
    [SerializeField]
    bool lockZRotation;

    void Start()
    {
        rotator = transform.GetChild(0);
        cam = GetComponentInChildren<Camera>();
    }
    void Updating()
    {
        if (owner == null)
            return;
        Quaternion bkRot = rotator.rotation;
        Quaternion newRot = Quaternion.RotateTowards(transform.rotation, owner.rotation, 20.0f);
        Vector3 newEuler = newRot.eulerAngles;
        if (lockXRotation)
            newEuler.x = transform.rotation.eulerAngles.x;
        if (lockXRotation)
            newEuler.y = transform.rotation.eulerAngles.y;
        if (lockXRotation)
            newEuler.z = transform.rotation.eulerAngles.z;
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, owner.rotation, 20.0f);
        transform.eulerAngles = newEuler;
        rotator.rotation = bkRot;
        float axisX = Input.GetAxis("Mouse X")*mouseSensitive.x;
        float axisY = Input.GetAxis("Mouse Y")*mouseSensitive.y;

        float upLimit = 88;
        float downLimit = 270;

        float x = rotator.localEulerAngles.x;
        if (x < 180 && x - axisY >= upLimit)
            axisY = x - upLimit;
        else if (x >= 180 && x - axisY <= downLimit)
            axisY = x - downLimit;

        Vector3 localEuler = (Quaternion.Euler(0, axisX, 0) * rotator.localRotation * Quaternion.Euler(-axisY, 0, 0)).eulerAngles;
        localEuler.z = 0;
        rotator.localRotation = Quaternion.Euler(localEuler);
        switch (movingType)
        {
            case MOVING_TYPE.Linear:
                transform.position = Vector3.MoveTowards(transform.position, owner.position, camSpeed);
                break;
            case MOVING_TYPE.Lerp:
                transform.position = Vector3.Lerp(transform.position, owner.position, camSpeed);
                break;
            case MOVING_TYPE.Radial:
                break;
            default:
                break;
        }
    }
    public void SetOwner(Transform _owner)
    {
        owner = _owner;
    }
    void LateUpdate()
    {
        if (updateType == UPDATE_TYPE.LateUpdate)
            Updating();
    }
    void FixedUpdate()
    {
        if (updateType == UPDATE_TYPE.FixedUpdate)
            Updating();
    }
    void Update()
    {
        if (updateType == UPDATE_TYPE.Update)
            Updating();
    }
    
}
