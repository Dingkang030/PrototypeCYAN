using UnityEngine;
using System.Collections;

[System.Serializable]
public class CamInfo
{
    public string name="no name";
    public float springDistance = 1.92f;
    //public float closeOffset_Z = 0.0f;
    //public float destOffset_Z = 0.2f;
    //public float startOffset_Z = 0.2f;
    public float rotatorOffset_Y = 1.43f;
    public float mainCamOffset_X = 0.0f;
    public float springSpeed = 3.0f;
    public float changingSpeed = 2.0f;
    public float movementSpeed = 0.1f;
    public bool lockRotationX;
    public bool lockRotationY;
    public bool lockRotationZ;
    public float lockSpeed = 3.0f;
    public bool fixToTarget = false;
    public LayerMask blockingLayer = 0;
}

public class CameraSpring : Singleton<CameraSpring> {

    //public enum CAM_STATE { NORMAL, AIM, FOCUS };
    [SerializeField]
    CamInfo[] camInfos;
    [SerializeField]
    int currentCamIndex;
    float originX;
    new Rigidbody rigidbody;
    //Transform rotator;
    Transform shaker;
    //Transform cam;
    RayHitComparer comparer=new RayHitComparer();
    public bool blocked;
    [HideInInspector]
    public Transform rotator;
    [HideInInspector]
    public Camera cam;

    [Header("movement")]
    [SerializeField]
    float offSet_Y;
    [SerializeField]
    public Transform owner;
    [SerializeField]
    Vector2 mouseSensitive = Vector2.one;

    Transform fixedTarget;

    public enum MOVING_TYPE
    {
        Linear, Lerp, Radial
    }
    public enum UPDATE_TYPE
    {
        Update, LateUpdate, FixedUpdate
    }
    public UPDATE_TYPE updateType;
    public MOVING_TYPE movingType;

    [SerializeField]
    bool lockXRotation;
    [SerializeField]
    bool lockYRotation;
    [SerializeField]
    bool lockZRotation;


    void Start () {
        rigidbody = GetComponent<Rigidbody>();
        //cam = GetComponentInChildren<Camera>().transform;
        cam = GetComponentInChildren<Camera>();
        shaker = cam.transform.parent;
        rotator = shaker.parent;
        originX = cam.transform.localPosition.x;
	}
    CamInfo currentCam { get { return camInfos[currentCamIndex]; } }
    public void ChangeCamInfo(int index)
    {
        if (index >= camInfos.Length)
            return;
        currentCamIndex = index;
    }
    public void SetFixTarget(Transform _target)
    {
        fixedTarget = _target;
    }
    public void ChangeCamInfo(string _name)
    {
        for(int i=0;i<camInfos.Length;i++)
        {
            if(camInfos[i].name==_name)
            {
                currentCamIndex = i;
                break;
            }
        }
    }
    
    public void SetOwner(Transform _owner)
    {
        owner = _owner;
    }
    void LateUpdate()
    {
        if (updateType == UPDATE_TYPE.LateUpdate)
        {
            MovementUpdate();
            SpringUpdate();
        }
    }
    void FixedUpdate()
    {
        if (updateType == UPDATE_TYPE.FixedUpdate)
        {
            MovementUpdate();
            SpringUpdate();
        }
    }
    void Update()
    {
        if (updateType == UPDATE_TYPE.Update)
        {
            MovementUpdate();
            SpringUpdate();
        }
    }

    void MovementUpdate()
    {
        if (owner == null)
            return;
        Quaternion bkRot = rotator.rotation;
        Quaternion newRot = Quaternion.RotateTowards(transform.rotation, owner.rotation, 20.0f);
        Vector3 newEuler = newRot.eulerAngles;
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, owner.rotation, 20.0f);
        transform.eulerAngles = newEuler;
        rotator.rotation = bkRot;
        float axisX = Input.GetAxis("Mouse X") * mouseSensitive.x;
        float axisY = Input.GetAxis("Mouse Y") * mouseSensitive.y;

        float upLimit = 88;
        float downLimit = 270;

        float x = rotator.localEulerAngles.x;
        if (x < 180 && x - axisY >= upLimit)
            axisY = x - upLimit;
        else if (x >= 180 && x - axisY <= downLimit)
            axisY = x - downLimit;

        if (currentCam.lockRotationX)
            axisY = 0;
        if (currentCam.lockRotationY)
            axisX = 0;
        Vector3 localEuler = (Quaternion.Euler(0, axisX, 0) * rotator.localRotation * Quaternion.Euler(-axisY, 0, 0)).eulerAngles;
        //localEuler.z = 0;
        if (currentCam.fixToTarget)
        {
            rotator.rotation = Quaternion.LookRotation(fixedTarget.position - rotator.position);
        }
        else
        {
            if (currentCam.lockRotationX)
                localEuler.x = Mathf.Lerp(localEuler.x, (localEuler.x > 180) ? 360 : 0, Time.deltaTime * currentCam.lockSpeed);
            if (currentCam.lockRotationY)
                localEuler.y = Mathf.Lerp(localEuler.y, (localEuler.y > 180) ? 360 : 0, Time.deltaTime * currentCam.lockSpeed);
            if (currentCam.lockRotationZ)
                localEuler.z = Mathf.Lerp(localEuler.z, (localEuler.z > 180) ? 360 : 0, Time.deltaTime * currentCam.lockSpeed);
            rotator.localRotation = Quaternion.Euler(localEuler);
        }

        switch (movingType)
        {
            case MOVING_TYPE.Linear:
                transform.position = Vector3.MoveTowards(transform.position, owner.position, currentCam.movementSpeed);
                break;
            case MOVING_TYPE.Lerp:
                transform.position = Vector3.Lerp(transform.position, owner.position, currentCam.movementSpeed);
                break;
            case MOVING_TYPE.Radial:
                break;
            default:
                break;
        }
    }
    void SpringUpdate () {
        RaycastHit hitInfo;
        RaycastHit hitInfo2;
        RaycastHit hitInfoRight;
        Vector3 dir = cam.transform.forward;
        Vector3 pos1 = cam.transform.position;
        Vector3 pos2 = rotator.position+rotator.right*originX;
        float distance = currentCam.springDistance;
        bool hit = false;
        bool hit2 = false;
        if (Physics.SphereCast(pos1, 0.2f,dir, out hitInfo, distance, currentCam.blockingLayer))
            hit = true;
        if (Physics.SphereCast(pos2, 0.2f, -dir, out hitInfo2, distance, currentCam.blockingLayer))
            hit2 = true;
        //if (Physics.Raycast(pos1, dir, out hitInfo, distance, currentCam.blockingLayer))
        //    hit = true;
        //if (Physics.Raycast(pos2, -dir, out hitInfo2, distance, currentCam.blockingLayer))
        //    hit2 = true;
        if (hit == true)
        {

//            Debug.DrawRay(pos1, hitInfo.point - pos1, Color.cyan);
            if (Physics.Linecast(rotator.position, pos2, out hitInfoRight, currentCam.blockingLayer))
            {
                blocked = true;
                Debug.DrawRay(rotator.position, hitInfoRight.point - rotator.position, Color.blue);
                float rightDiff = Vector3.Distance(hitInfoRight.point, pos2);
                Debug.DrawRay(hitInfoRight.point, (hitInfo.point+cam.transform.right*-rightDiff) - hitInfoRight.point, Color.blue);
            }
        } 
        if( hit2 == true)
        {
            Debug.DrawRay(pos2, hitInfo2.point - pos2, Color.red);
            Vector3 localPos = cam.transform.localPosition;
            float newZ = -hitInfo2.distance;
            localPos.z = newZ;
            //cam.position = hitInfo2.point;
            cam.transform.localPosition = localPos;
            blocked = true;
        }
        if (!hit&&!hit2)
        {
            /*Vector3 localPos = cam.localPosition;
            localPos.z = -currentCam.springDistance;
            //localPos.x = originX;
            cam.localPosition = localPos;*/
            Debug.DrawRay(pos1, dir * distance, Color.yellow);
            blocked = false;
        }

        Vector3 mainCamPos = cam.transform.localPosition;
        mainCamPos.x = currentCam.mainCamOffset_X;
        if (!blocked)
            mainCamPos.z = -currentCam.springDistance;
        cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition,mainCamPos,Time.unscaledDeltaTime* currentCam.changingSpeed);
        Vector3 rotatorPos = rotator.localPosition;
        rotatorPos.y = currentCam.rotatorOffset_Y;
        rotator.localPosition = Vector3.Lerp(rotator.localPosition,rotatorPos, Time.unscaledDeltaTime * currentCam.changingSpeed);
    }
}
class RayHitComparer : IComparer
{
    public int Compare(object x, object y)
    {
        return ((RaycastHit)x).distance.CompareTo(((RaycastHit)y).distance);
    }
}
