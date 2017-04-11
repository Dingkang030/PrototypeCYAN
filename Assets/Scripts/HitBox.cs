using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HitBox
{
    static bool debug = true;
    public static Collider[] LaunchHitBox(Vector3 _position,Vector3 _extends,Quaternion _rotation)
    {
        Collider[] cols = Physics.OverlapBox(_position, _extends, _rotation);
        if(debug)
        {
            Vector3[] point = new Vector3[8];
            point[0] = new Vector3(-_extends.x, -_extends.y, _extends.z);
            point[1] = new Vector3(_extends.x, -_extends.y, _extends.z);
            point[2] = new Vector3(_extends.x, -_extends.y, -_extends.z);
            point[3] = new Vector3(-_extends.x, -_extends.y, -_extends.z);

            point[4] = new Vector3(-_extends.x, _extends.y, _extends.z);
            point[5] = new Vector3(_extends.x, _extends.y, _extends.z);
            point[6] = new Vector3(_extends.x, _extends.y, -_extends.z);
            point[7] = new Vector3(-_extends.x, _extends.y, -_extends.z);

            for (int i = 0; i < 8; i++)
                point[i] = _rotation * point[i];
            Debug.DrawLine(point[0], point[1], Color.yellow, 2.0f);
            Debug.DrawLine(point[0], point[2], Color.yellow, 2.0f);
            Debug.DrawLine(point[1], point[2], Color.yellow, 2.0f);
            Debug.DrawLine(point[2], point[3], Color.yellow, 2.0f);

            Debug.DrawLine(point[4], point[5], Color.yellow, 2.0f);
            Debug.DrawLine(point[4], point[6], Color.yellow, 2.0f);
            Debug.DrawLine(point[5], point[6], Color.yellow, 2.0f);
            Debug.DrawLine(point[6], point[7], Color.yellow, 2.0f);
        }
        if(cols.Length>0)
        {
            return cols;
        }
        return null;
    }
    public static Collider[] LaunchHitBox(Vector3 _position, Vector3 _extends, Quaternion _rotation, LayerMask _mask)
    {

        if (debug)
        {
            Vector3[] point = new Vector3[8];
            point[0] = new Vector3(-_extends.x, -_extends.y, _extends.z);
            point[1] = new Vector3(_extends.x, -_extends.y, _extends.z);
            point[2] = new Vector3(_extends.x, -_extends.y, -_extends.z);
            point[3] = new Vector3(-_extends.x, -_extends.y, -_extends.z);

            point[4] = new Vector3(-_extends.x, _extends.y, _extends.z);
            point[5] = new Vector3(_extends.x, _extends.y, _extends.z);
            point[6] = new Vector3(_extends.x, _extends.y, -_extends.z);
            point[7] = new Vector3(-_extends.x, _extends.y, -_extends.z);

            for (int i = 0; i < 8; i++)
                point[i] = _rotation * point[i]+ _position;
            Debug.DrawLine(point[0], point[1], Color.yellow, 2.0f);
            Debug.DrawLine(point[0], point[3], Color.yellow, 2.0f);
            Debug.DrawLine(point[1], point[2], Color.yellow, 2.0f);
            Debug.DrawLine(point[2], point[3], Color.yellow, 2.0f);

            Debug.DrawLine(point[4], point[5], Color.yellow, 2.0f);
            Debug.DrawLine(point[4], point[7], Color.yellow, 2.0f);
            Debug.DrawLine(point[5], point[6], Color.yellow, 2.0f);
            Debug.DrawLine(point[6], point[7], Color.yellow, 2.0f);

            Debug.DrawLine(point[1], point[5], Color.yellow, 2.0f);
            Debug.DrawLine(point[2], point[6], Color.yellow, 2.0f);
            Debug.DrawLine(point[0], point[4], Color.yellow, 2.0f);
            Debug.DrawLine(point[3], point[7], Color.yellow, 2.0f);


        }
        Collider[] cols = Physics.OverlapBox(_position, _extends, _rotation, _mask.value);
        if (cols.Length > 0)
        {
            return cols;
        }
        return null;
    }
    public static Collider[] LaunchHitSphere(Vector3 _position, float _radius)
    {
        
        Collider[] cols = Physics.OverlapSphere(_position, _radius);
        if (cols.Length > 0)
        {
            return cols;
        }
        return null;
    }
    public static Collider[] LaunchHitSphere(Vector3 _position, float _radius,params string[] tags)
    {
        Collider[] cols = Physics.OverlapSphere(_position, _radius);
        if (cols.Length > 0)
        {
            return cols;
        }
        return null;
    }
    public static Collider[] LaunchHitSphere(Vector3 _position, float _radius, LayerMask _mask)
    {
        LayerMask ignoreLayers = new LayerMask();
        ignoreLayers.value = ~_mask;
        Collider[] cols = Physics.OverlapSphere(_position, _radius, _mask.value);
        if (cols.Length > 0)
        {
            return cols;
        }
        return null;
    }
}
