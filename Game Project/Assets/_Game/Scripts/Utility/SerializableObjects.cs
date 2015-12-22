using UnityEngine;
using System.Collections;

[System.Serializable]
public class SerializableRect
{
    public float x;
    public float y;
    public float width;
    public float height;

    public Rect rect
    {
        get { return new Rect(x, y, width, height); }
        set { x = value.x; y = value.y; width = value.width; height = value.height; }
    }


}