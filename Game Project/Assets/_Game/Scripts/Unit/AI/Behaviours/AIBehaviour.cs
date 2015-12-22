using UnityEngine;
using System.Collections;
using System.Collections.Generic;







[System.Serializable]
public abstract class AIBehaviour : ScriptableObject
{
    public int index;
    public AIGraphicNode graphic;
    public List<AITransition> transitions = new List<AITransition>();

    [System.NonSerialized]
    public AIController controller;

    private string shortName = "";
    public string Name
    {
        get
        {
            if (shortName == "")
            {
                shortName = this.GetType().Name;
                shortName = shortName.Replace("Behaviour", "");
            }
            return shortName;
        }
    }

    public void OnEnable()
    {
        if (transitions == null)
        {
            transitions = new List<AITransition>();
            SetGraphicNode(new Vector2(50, 50));
        }
    }

    public abstract void Initialize();

    // Links holder chain for object instances
    // NOTE: Maybe the binary cloner preserves references. Test.
    public void Link(AIController _controller)
    {
        controller = _controller;
        foreach (AITransition transition in transitions)
            transition.Link(this);
    }

    public void SetGraphicNode(Vector2 pos)
    {
        graphic = ScriptableObject.CreateInstance<AIGraphicNode>();
        graphic.Area = new Rect(pos, new Vector2(100, 100));
        graphic.index = index;
    }

    public abstract void Update();

    public void AddTransition(AIBehaviour end)
    {
        AITransition transition = ScriptableObject.CreateInstance<AITransition>();
        transition.behaviour = this;
        transition.startIndex = index;
        transition.endIndex = end.index;
        transitions.Add(transition);

        // TODO: implement manual adition of condition
        SensorCondition sensorCondition = ScriptableObject.CreateInstance<SensorCondition>();
        transition.conditions.Add(sensorCondition);
    }


    // If a transition has triggered, the index to the end state is returned. Other wise this state's index is returned.
    public int CheckTransitions()
    {
        foreach (AITransition transition in transitions)
        {
            if (transition.CheckConditions())
                return transition.endIndex;
        }
        return index;
    }



    public void DrawNodeWindow()
    {
        GUILayout.Label(Name);
        GUILayout.Label("Index: " + index);

    }
}
