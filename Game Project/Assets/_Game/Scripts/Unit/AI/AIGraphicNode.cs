using UnityEngine;
using System.Collections;

/// <summary>
/// A node in the AIEditor window graph.
/// Each node represents a behaviour in the AI Controller.
/// This class takes care of the graphical representation of such behaviours
/// </summary>
[System.Serializable]
public class AIGraphicNode : AIGraphicElement
{

    private SerializableRect area = new SerializableRect();
    public Rect Area
    {
        get { return area.rect;  }
        set { area.rect = value; }
    }

    [HideInInspector]
    public int index;

    public void OnGUI()
    {

    }


}