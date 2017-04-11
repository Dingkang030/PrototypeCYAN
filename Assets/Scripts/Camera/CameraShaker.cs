using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaker : MonoBehaviour {

    public float shakeAmount;
    public float shakeTimer;
    [Range(0, 1)]
    public float shakeSpeed;
    public bool shaking;
    public Transform shakePivot;
    void Start () {
    }
    void Update()
    {
        if (shakeTimer > 0 && shaking == false)
            ShakeCamera(shakeAmount, shakeTimer,shakeSpeed);
    }
    public void ShakeCamera(float power, float time,float speed=0.5f)
    {
        shakeAmount = power;
        shakeTimer = time;
        shakeSpeed = speed;
        shaking = true;
        StartCoroutine("ShakeCoroutine");
    }
    IEnumerator ShakeCoroutine()
    {
        Vector2 shakePos=Vector2.zero;
        while (shakeTimer>0)
        {
            while (true)
            {
                Vector2 newShakePos = Random.insideUnitCircle.normalized;
                if (Vector2.Angle(shakePos, newShakePos) > 30)
                {
                    shakePos = newShakePos;
                    break;
                }
            }
            shakePos *= shakeAmount;
            Vector3 dest = new Vector3(shakePos.x, shakePos.y, 0);
            while (Vector3.Distance(dest, shakePivot.localPosition) > 0.1f&&shakeTimer>0)
            {
                shakePivot.localPosition = Vector3.MoveTowards(shakePivot.localPosition, dest, shakeSpeed);
                shakeTimer -= Time.deltaTime;
                yield return null;
            }
        }
        shakePivot.localPosition = Vector3.zero;
        shakeTimer = 0;
        shaking = false;
    }
}
