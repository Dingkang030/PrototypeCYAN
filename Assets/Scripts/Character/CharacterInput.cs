using UnityEngine;
using System.Collections;
[RequireComponent(typeof(CharacterMovement))]
public class CharacterInput : MonoBehaviour
{



    protected CharacterMovement movement;
    public CameraSpring camSpring;
    public GameObject aim;
    public float maxSpeed;
    public float walkSpeed;
    public float accelation;
    public float breakAccel = 0.2f;
    public float turnSpeed = 40;
    public float jumpPower = 15.0f;
    UnitInfo myUnit;
    public PHarpoon harpoonProjectile;
    GenericIK genericIK;
    public bool zoomIn;
    bool isZoomAble = true;

    float dashCheckTime;

    public float dashCheckMaxTime = 3;

    Vector3 lastAxis = new Vector3();
    void Start()
    {
        myUnit = GetComponent<UnitInfo>();
        movement = GetComponent<CharacterMovement>();
        genericIK = GetComponent<GenericIK>();
    }
    public void SetZoomAble(bool _able)
    {
        isZoomAble = _able;
    }
    public static Vector3 GetMovingInput()
    {
        Vector3 axis = new Vector3();
        axis.x = (Input.GetKey(KeyCode.A)) ? -1 : (Input.GetKey(KeyCode.D)) ? 1 : 0;
        axis.z = (Input.GetKey(KeyCode.W)) ? 1 : (Input.GetKey(KeyCode.S)) ? -1 : 0;
        return axis;
    }
    void MovementInput()
    {
        Vector3 axis = GetMovingInput();
        Vector3 downAxis = new Vector3();
        downAxis.x = (Input.GetKeyDown(KeyCode.A)) ? -1 : (Input.GetKeyDown(KeyCode.D)) ? 1 : 0;
        downAxis.z = (Input.GetKeyDown(KeyCode.W)) ? 1 : (Input.GetKeyDown(KeyCode.S)) ? -1 : 0;
        if (dashCheckTime > 0)
        {
            if (downAxis != Vector3.zero)
            {
                if (downAxis == lastAxis)
                {
                    dashCheckTime = 0;
                    if (!myUnit.GetAction().GetBehaviour("WakeUp").IsPlaying() && movement.isOnGround)
                    {
                        myUnit.GetAction().UseSkill("Roll", true);
                    }
                    return;
                }
                else
                {
                    lastAxis = downAxis;
                    dashCheckTime = dashCheckMaxTime;
                }
            }
            else dashCheckTime -= Time.deltaTime;
        }
        else
        {
            lastAxis = downAxis;
            dashCheckTime = dashCheckMaxTime;
        }
        if (movement.paramaterSet != maxSpeed)
            movement.paramaterSet = maxSpeed;
        bool sprint = !Input.GetKey(KeyCode.LeftShift);
        if (!zoomIn && myUnit.GetState() == UnitInfo.UnitState.idle)
        {
            if (axis != Vector3.zero)
            {
                float speed = (sprint) ? maxSpeed : walkSpeed;
                axis = Camera.main.transform.TransformVector(axis);
                axis.y = 0;
                axis.Normalize();
                //movement.Move(axis, speed, accelation);
                movement.RotateTo(Quaternion.LookRotation(axis), turnSpeed);
                movement.Move(transform.forward, speed, accelation);
            }
            else
            {
                if (movement.IsMoving())
                {
                    //movement.Stop();
                    movement.StopSmooth(breakAccel);
                }
            }
        }
        else if (zoomIn && myUnit.GetState() != UnitInfo.UnitState.die)
        {
            if (axis != Vector3.zero)
            {
                float speed = (sprint) ? maxSpeed : walkSpeed;
                axis = Camera.main.transform.TransformVector(axis);
                axis.y = 0;
                axis.Normalize();
                movement.Move(axis, speed, accelation);
            }
            else if (myUnit.GetState() == UnitInfo.UnitState.idle)
            {
                if (movement.IsMoving())
                {
                    //movement.Stop();
                    movement.StopSmooth(breakAccel);
                }
            }
            Vector3 forward = Camera.main.transform.forward;
            forward.y = 0;
            if (forward == Vector3.zero)
                movement.RotateTo(forward, 90);
            else movement.RotateTo(Quaternion.LookRotation(forward), 90);
        }
    }
    void KeyInput()
    {
        if (myUnit.GetCondition() != ConditionData.UnitCondition.none)
        {
            movement.Stop();
            return;
        }

        // Zoom IN
        if (myUnit.GetAction().IsUsingAction("ThrowHarpoon"))
        {
            if (myUnit.GetAction().behaviourState < 3)
            {
                if ((!Input.GetKey(KeyCode.Mouse1) || myUnit.currentWeapon.weaponName != "Harpoon"))
                {
                    /*camSpring.ChangeCamInfo("Player");
                    zoomIn = false;*/
                    if (zoomIn)
                    {
                        camSpring.ChangeCamInfo("Player");
                        zoomIn = false;
                    }
                    myUnit.GetAction().StopAction();
                }
            }
            /*if (zoomIn && !movement.isOnGround)
            {
                Time.timeScale = 0.3f;
                Time.fixedDeltaTime = 0.02F * Time.timeScale;
            }
            else
            {
                Time.timeScale = 1f;
                Time.fixedDeltaTime = 0.02F * Time.timeScale;
            }*/
        }
        else
        {
            if (myUnit.currentWeapon.weaponName == "Harpoon")
            {
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    if (myUnit.GetAction().UseSkill("ThrowHarpoon"))
                    {
                        camSpring.ChangeCamInfo("Player_Aim");
                        zoomIn = true;
                    }
                }
            }
            else if (harpoonProjectile.harpoonState == PHarpoon.HARPOON_STATE.FIXED)
            {
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    myUnit.GetAction().UseSkill("PullHarpoon");
                }
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (harpoonProjectile.harpoonState == PHarpoon.HARPOON_STATE.FIXED)
            {
                myUnit.GetAction().UseSkill("HookShot", false);
            }
            else if (!zoomIn && harpoonProjectile.harpoonState == PHarpoon.HARPOON_STATE.NONE)
            {
                myUnit.GetAction().UseSkill("SwingWeapon");
            }
            else if (zoomIn && harpoonProjectile.harpoonState == PHarpoon.HARPOON_STATE.NONE)
            {
                myUnit.GetAction().UseSkill("ThrowHarpoon");
            }
        }
        MovementInput();
        if (Input.GetKey(KeyCode.E) && harpoonProjectile.harpoonState == PHarpoon.HARPOON_STATE.FIXED)
        {
            if (harpoonProjectile.fixedTarget.tag == "Monster")
            {
                SoulInfo soulInfo = harpoonProjectile.fixedTarget.GetComponent<SoulInfo>();
                if (soulInfo && soulInfo.IsAnEvil())
                {
                    myUnit.GetAction().UseSkill("HealingSoul", false);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Space) && !myUnit.GetAction().IsUsingAction())
        {
            if (!movement.isOnGround)
            {
                if (myUnit.IsHaveBuff(BuffBehaviour.BuffType.Fly))
                {
                    myUnit.GetAction().UseSkill("Flying");
                }
            }
            else movement.Jump(jumpPower);
        }
    }
    void Update()
    {
        KeyInput();

    }
}
