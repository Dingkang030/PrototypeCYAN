using System.Collections;
using System.Collections.Generic;
using System.Threading;

using UnityEngine;
public class SnowDeformer : MonoBehaviour {
    [SerializeField]
    [Range(0.1f,1f)]
    float delay;
    Vector3 lastPos;
    [SerializeField]
    Material snowDecal;
    [SerializeField]
    Vector3 eulerOffset;
    [SerializeField]
    LayerMask detectLayer;
    [SerializeField]
    float decalScale = 1;
    CharacterMovement movement;
    [SerializeField]
    float detectDistance = 1.0f;
    [SerializeField]
    bool autoDeform = false;
	void Start () {
        movement = GetComponent<CharacterMovement>();
    }
    void CreateDeformedSnow(Vector3 pos)
    {
        RaycastHit[] hits =
        Physics.RaycastAll(pos + Vector3.up * 0.1f, Vector3.up * -1, detectDistance, detectLayer);
        if (hits.Length > 0)
        {
            foreach (RaycastHit hit in hits)
            {
                SnowDeformable d = hit.transform.GetComponent<SnowDeformable>();
                if (d != null)
                {
                    d.DrawAt(hit.textureCoord, snowDecal, decalScale, 1, transform.eulerAngles.y);
                }
            }
        }
    }
    void Update()
    {
        if (!autoDeform)
            return;
        if (lastPos == transform.position)
            return;
        else
        {
            CreateDeformedSnow(transform.position);
        }
    }
    void LateUpdate()
    {
        if (!autoDeform)
            return;
        lastPos = transform.position;
    }
}
