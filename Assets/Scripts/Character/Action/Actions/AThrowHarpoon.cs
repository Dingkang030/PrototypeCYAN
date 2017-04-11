using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AThrowHarpoon : ActionBehaviour {
    [SerializeField]
    PHarpoon harpoonProjectile;
    public CameraTargetFinder targetFinder;
    public float defaultDistance=10.0f;
    public float speed;
    protected override void Start()
    {
        base.Start();
        targetFinder = Camera.main.transform.parent.parent.parent.GetComponent<CameraTargetFinder>();
    }
    public override string GetActionName()
    {

        return "ThrowHarpoon";
    }
    PHarpoon CreateHarpoonProjectile()
    {
        PHarpoon h = Instantiate<PHarpoon>(Resources.Load<GameObject>("Projectiles/PF_Pt_Harpoon").GetComponent<PHarpoon>());
        
        return h;
    }
    protected override IEnumerator BehaviourCoroutine()
    {
        SetStopActionEvent(
            delegate ()
            {
                unitInfo.ChangeState(UnitInfo.UnitState.idle);
                animator.SetLayerWeight(1, 0.0f);
                animator.Play("base", 1);
            }
        );
        float charge = 0.0f;
        animator.SetLayerWeight(1, 1.0f);
        animator.CrossFade("ThrowHarpoon",0.25f, 1);
        yield return new WaitForMessageReceiving(this, "Throw_Ready");
        //yield return new WaitForStateChange(this, 2);
        animator.CrossFade("ThrowHarpoon_aim", 0.25f, 1);
        bool fire = false;
        while (Input.GetKey(KeyCode.Mouse1))
        {
            charge += Time.deltaTime;
            charge = Mathf.Clamp(charge, 0, 3.0f);
            if (Input.GetKey(KeyCode.Mouse0))
            {
                fire = true;
                break;
            }
            yield return null;
        }
        if (fire)
        {
            animator.CrossFade("ThrowHarpoon", 0.055f);
            animator.SetLayerWeight(1, 0.0f);
            unitInfo.ChangeState(UnitInfo.UnitState.action);
            float distancePerCharge = 5.0f;
            Vector3 targetPos = Camera.main.transform.position + Camera.main.transform.forward * (defaultDistance + distancePerCharge * charge);
            Vector3 newRotateForward = targetPos - unitInfo.transform.position;
            newRotateForward.y = 0;
            unitInfo.GetMovement().RotateTo(newRotateForward, 30);

            yield return new WaitForMessageReceiving(this, "Throw_Fire");
            //yield return new WaitForStateChange(this, 3);
            if (harpoonProjectile == null)
                harpoonProjectile = CreateHarpoonProjectile();
            //unitInfo.ChangeWeapon(false);

            targetPos = Camera.main.transform.position + Camera.main.transform.forward * (defaultDistance + distancePerCharge * charge);
            newRotateForward = targetPos - unitInfo.transform.position;
            newRotateForward.y = 0;
            unitInfo.GetMovement().RotateTo(newRotateForward, 30);

            Vector3 spawnPos = Camera.main.transform.position + Camera.main.transform.forward * (-Camera.main.transform.localPosition.z + 1.2f);
            harpoonProjectile.SetOwner(unitInfo.gameObject);
            harpoonProjectile.Shot(spawnPos, targetPos, Quaternion.LookRotation(targetPos - spawnPos).eulerAngles, speed, speed, speed);

            harpoonProjectile.chainDrawer.gameObject.SetActive(true);
            unitInfo.EquipWeapon(0);
            harpoonProjectile.chainDrawer.isDrawable = true;
            harpoonProjectile.gameObject.SetActive(true);

            yield return new WaitForMessageReceiving(this, "Throw_End");
            //yield return new WaitForStateChange(this, 4);
        }
        yield return null;
        StopAction();
    }
}
