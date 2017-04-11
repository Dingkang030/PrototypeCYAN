using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawBones : MonoBehaviour
{
    public Transform bones;

    void Start()
    {
    }

    void LateUpdate()
    {
        DrawBone(bones,bones);
    }
    void DrawBone(Transform root,Transform B)
    {
        for (int i = 0; i < B.childCount; i++)
            DrawBone(root,B.GetChild(i));
        if(B.parent!=null&&root!=B)
            Debug.DrawLine(B.position, B.parent.position, Color.red);
    }
}
