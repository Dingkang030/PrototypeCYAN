using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTargetFinder : MonoBehaviour {

    public Vector3 targetPosition;
    public LayerMask targetLayer;
    public Transform owner;
    public Transform targetObject;
    public Transform mainCam;
	void Start () {
        if(mainCam==null)
            mainCam = transform.GetChild(0).GetChild(0);
        targetObject = mainCam.GetChild(0);
	}
	

	void Update () {
        RaycastHit[] hits = Physics.RaycastAll(mainCam.position, mainCam.forward, Mathf.Infinity, targetLayer);
        float distance = 9999;
        Transform newTarget = null;
        Vector3 hitPos= mainCam.position+ mainCam.forward*distance;
        if (hits.Length>0)
        {
            foreach(RaycastHit hit in hits)
            {
                if (hit.transform == owner)
                    continue;
                float d = Vector3.Distance(mainCam.position, hit.point);
                if (d<distance)
                {
                    newTarget = hit.transform;
                    distance = d;
                }
            }
        }
        targetPosition = mainCam.position + mainCam.forward * distance;
        /*if (newTarget != null)
            targetObject = newTarget;
        else
        {*/
            //targetObject = transform.GetChild(0);
            targetObject.position = targetPosition;
        //}
    }
}
