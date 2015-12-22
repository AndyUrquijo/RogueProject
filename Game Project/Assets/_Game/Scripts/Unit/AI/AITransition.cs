using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AITransition : AIGraphicElement
{
    // If any one of the conditions is true, the transition happens
    public List<AICondition> conditions;

    public int startIndex;
    public int endIndex;

    [System.NonSerialized]
    public AIBehaviour behaviour;

    public void OnEnable()
    {
        if (conditions == null)
            conditions = new List<AICondition>();

    }

    // Links holder chain for object instances
    public void Link( AIBehaviour _behaviour )
    {
        behaviour = _behaviour;
        foreach (AICondition condition in conditions)
            condition.transition = this;
    }

    public bool CheckConditions()
    {

        foreach (AICondition condition in conditions)
        {
            if (condition.CheckCondition() == true)
                return true;
        }
        return false;

    }

}