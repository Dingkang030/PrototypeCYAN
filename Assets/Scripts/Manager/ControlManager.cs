using UnityEngine;
using System.Collections;

public class ControlManager : MonoBehaviour {
    
    UnitInfo possedUnit;
    UnitInfo lastPosedUnit;
    public CharacterInput currentInput;
    [SerializeField]
    bool autoPose;
    public GameObject mainCameraObject;
    

    static ControlManager instance;

	void Awake () {
        if(mainCameraObject==null)
            mainCameraObject = GameObject.Find("MainCameraObject");
        if (instance == null)
            instance = this;
        AutoPose();
	}
	
    void AutoPose()
    {
        if(autoPose)
        {
            UnitInfo[] infos = GameObject.FindObjectsOfType<UnitInfo>();
            GameObject unitObject=null;
            float distance = 100000;
            foreach(UnitInfo unit in infos)
            {
                if (unit.gameObject.layer != LayerMask.NameToLayer("Unit"))
                    continue;
                float dis = Vector3.Distance(transform.position, unit.transform.position);
                if (dis < distance)
                {
                    unitObject = unit.gameObject;
                    distance = dis;
                }
            }
            PoseTo(unitObject);
        }
    }
    public UnitInfo GetCurrentUnit()
    {
        return possedUnit;
    }
    static public ControlManager Instance
    {
        get
        {
            return instance;
        }
    }
    public CameraSpring GetCameraMovement()
    {
        return mainCameraObject.GetComponent<CameraSpring>();
    }
    public CameraTargetFinder GetCameraTargetFinder()
    {
        return mainCameraObject.GetComponent<CameraTargetFinder>();
    }
    public void PoseTo(GameObject _newPosedObject) // 새 유닛에 빙의함
    {
        if (_newPosedObject == null|| _newPosedObject.layer!=LayerMask.NameToLayer("Unit"))
        {
            Debug.Log(_newPosedObject);
            Debug.Log("Cant pose to this object");
            return;
        }
        UnitInfo newPosedUnit = _newPosedObject.GetComponent<UnitInfo>();
        if (newPosedUnit==null)
        {
            Debug.Log("Cant pose to this object");
            return;
        }
        SetCamTarget(_newPosedObject); // 카메라타겟을 옮겨줌
        currentInput=ToggleInput(_newPosedObject, true);// 빙의할캐릭터의 인풋을 켜줌
        if(possedUnit!=null)
            ToggleInput(possedUnit.gameObject, false); // 빙의한 캐릭터의 인풋을 꺼줌
        lastPosedUnit = possedUnit; // 이전빙의유닛 할당
        possedUnit = newPosedUnit; // 빙의유닛 할당
    }
    void SetCamTarget(GameObject _poseTo)
    {
        CameraSpring camMovement = mainCameraObject.GetComponent<CameraSpring>();
        CameraTargetFinder camTargetFinder = GetCameraTargetFinder();
        if (camMovement)
            camMovement.SetOwner(_poseTo.transform);  // 카메라 타겟을 바꿔줌
        if (camTargetFinder)
            camTargetFinder.owner = _poseTo.transform;
    }
    CharacterInput ToggleInput(GameObject _object, bool _toggle = true)
    {
        if (_object == null)
            return null;
        CharacterInput input = _object.GetComponent<CharacterInput>();
        if (input)
        {
            if (input.enabled != false || _toggle != false)
                input.enabled = _toggle; // 인풋컴포넌트를 비활/활 성화
        }
        return input;
    }
    
}
