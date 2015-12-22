using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
[CustomPropertyDrawer(typeof(EventSubjectValue))]
public class EventSubjectValueDrawer : PropertyDrawer
{

    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        var typeRect = new Rect(position.x, position.y, 100, position.height);
        var subjectRect = new Rect(position.x + 100, position.y, 200, position.height);
        //
        SerializedProperty subjectType = property.FindPropertyRelative("type");
        subjectType.enumValueIndex = (int)(EventSubjectValue.Type)EditorGUI.EnumPopup(typeRect, (EventSubjectValue.Type)subjectType.enumValueIndex);
        if (subjectType.enumValueIndex != (int)EventSubjectValue.Type.CUSTOM)
            GUI.enabled = false;
        EditorGUI.PropertyField(subjectRect, property.FindPropertyRelative("manager"),GUIContent.none);
        GUI.enabled = true;
    }
}
#endif


[System.Serializable]
public class EventSubjectValue
{
    public enum Type { SELF, GLOBAL, CUSTOM };
    public Type type;
    public EventManager manager;
}


public class EventSubject : MonoBehaviour
{
    public EventName type = EventName.GAME_OBJECT_DESTROYED;
    public EventSubjectValue subject;

    void Awake()
    {
        switch (subject.type)
        {
            case EventSubjectValue.Type.SELF:
                subject.manager = GetComponent<EventManager>();
                break;
            case EventSubjectValue.Type.GLOBAL:
                subject.manager = EventManager.GetGlobal();
                break;
        }
    }

}
