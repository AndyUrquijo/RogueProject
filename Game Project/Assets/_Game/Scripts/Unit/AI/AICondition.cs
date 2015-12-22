using UnityEngine;
using System.Collections;

[System.Serializable]
public abstract class AICondition : ScriptableObject
{
    [System.NonSerialized]
    public AITransition transition;

    abstract public bool CheckCondition();
}