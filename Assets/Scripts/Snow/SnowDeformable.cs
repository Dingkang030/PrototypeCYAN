using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowDeformable : MonoBehaviour {

    
    public int renderTextureWidth = 512;
    public int renderTextureHeight = 512;
    RenderTexture targetTexture;
    public float scaleSet = 1.0f;

    void Awake()
    {
        targetTexture = new RenderTexture(renderTextureWidth, renderTextureHeight, 32);
        GetComponent<Renderer>().material.SetTexture("_Disp", targetTexture);
        GetComponent<Renderer>().material.SetFloat("_ScaleY", transform.localScale.y);
    }
    Texture2D rotateTexture(Texture2D tex, float angle)
    {
        //Debug.Log("rotating");
        Texture2D rotImage = new Texture2D(tex.width, tex.height);
        int x, y;
        float x1, y1, x2, y2;

        int w = tex.width;
        int h = tex.height;
        float x0 = rot_x(angle, -w / 2.0f, -h / 2.0f) + w / 2.0f;
        float y0 = rot_y(angle, -w / 2.0f, -h / 2.0f) + h / 2.0f;

        float dx_x = rot_x(angle, 1.0f, 0.0f);
        float dx_y = rot_y(angle, 1.0f, 0.0f);
        float dy_x = rot_x(angle, 0.0f, 1.0f);
        float dy_y = rot_y(angle, 0.0f, 1.0f);


        x1 = x0;
        y1 = y0;

        for (x = 0; x < tex.width; x++)
        {
            x2 = x1;
            y2 = y1;
            for (y = 0; y < tex.height; y++)
            {
                //rotImage.SetPixel (x1, y1, Color.clear);          

                x2 += dx_x;//rot_x(angle, x1, y1);
                y2 += dx_y;//rot_y(angle, x1, y1);
                rotImage.SetPixel((int)Mathf.Floor(x), (int)Mathf.Floor(y), getPixel(tex, x2, y2));
            }

            x1 += dy_x;
            y1 += dy_y;

        }

        rotImage.Apply();
        return rotImage;
    }

    private Color getPixel(Texture2D tex, float x, float y)
    {
        Color pix;
        int x1 = (int)Mathf.Floor(x);
        int y1 = (int)Mathf.Floor(y);

        if (x1 > tex.width || x1 < 0 ||
           y1 > tex.height || y1 < 0)
        {
            pix = Color.clear;
        }
        else
        {
            pix = tex.GetPixel(x1, y1);
        }

        return pix;
    }

    private float rot_x(float angle, float x, float y)
    {
        float cos = Mathf.Cos(angle / 180.0f * Mathf.PI);
        float sin = Mathf.Sin(angle / 180.0f * Mathf.PI);
        return (x * cos + y * (-sin));
    }
    private float rot_y(float angle, float x, float y)
    {
        float cos = Mathf.Cos(angle / 180.0f * Mathf.PI);
        float sin = Mathf.Sin(angle / 180.0f * Mathf.PI);
        return (x * sin + y * cos);
    }
    public void DrawAt(Vector2 uvCoordinate, Material decalMaterial,float decalScale=1.0f,float alpha =1.0f,float eulerY=90.0f)
    {
        uvCoordinate.x = uvCoordinate.x * targetTexture.width;
        uvCoordinate.y = uvCoordinate.y * targetTexture.height;
        RenderTexture.active = targetTexture;
        Texture decal = decalMaterial.mainTexture;
        decal = rotateTexture((Texture2D)decalMaterial.mainTexture, eulerY);
        GL.PushMatrix();
        GL.LoadPixelMatrix(0, targetTexture.width, targetTexture.height, 0);
        
        Rect screenRect = new Rect();
        Vector2 scaledDecal = new Vector2(decal.width*(decalScale* scaleSet )/ transform.localScale.x,decal.height* (decalScale * scaleSet) / transform.localScale.z);
        screenRect.x = uvCoordinate.x - scaledDecal.x * 0.5f;
        screenRect.y = (targetTexture.height - uvCoordinate.y) - scaledDecal.y * 0.5f;
        screenRect.width = scaledDecal.x;
        screenRect.height = scaledDecal.y;
        Graphics.DrawTexture(screenRect, decal, decalMaterial);

        GL.PopMatrix();
        RenderTexture.active = null;
    }


}
