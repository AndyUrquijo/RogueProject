using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
[CustomPropertyDrawer(typeof(ToggleCommand))]
public class ToggleCommandDrawer : PropertyDrawer
{

    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        var toggleRect = new Rect(position.x, position.y, 30, position.height);
        var timerRect = new Rect(position.x + 30, position.y, 50, position.height);

        EditorGUI.Toggle(toggleRect, property.FindPropertyRelative("toggle").boolValue);
        EditorGUI.FloatField(timerRect, property.FindPropertyRelative("timeHeld").floatValue);
    }
}
#endif


[System.Serializable]
public struct ToggleCommand
{
    public bool Toggle
    {
        get { return toggle; }
        set
        {
            on = !toggle && value;
            off = toggle && !value;
            toggle = value;
            if (toggle)
                timeHeld += Time.deltaTime;
            else
                timeHeld = 0;
        }
    }
    public bool toggle;
    static public implicit operator bool(ToggleCommand command)
    {
        return command.toggle;
    }


    // True just after the command has been toggled (typically during one frame)
    public bool On
    {
        get { return on; }
    }
    private bool on;

    // True just after the command has been untoggled (typically during one frame)
    public bool Off
    {
        get { return off; }
    }
    private bool off;

    public float TimeHeld
    {
        get { return timeHeld; }
    }
    public float timeHeld;
}


public class Command : AbstractBehaviour
{

	public float move;

    public ToggleCommand jump;
	public int attack;
	public bool dash;
	public bool release;

}
