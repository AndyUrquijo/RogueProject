using UnityEngine;
using System.Collections;



// This class will be used as a base for unit behaviours
// It allows the use of the script assembler class
public abstract class AbstractBehaviour : MonoBehaviour {

    [HideInInspector]
    public Command command;
    [HideInInspector]
    public Body body;
    [HideInInspector]
    public State state;



    void Awake()
    {
        Initialize();
    }

    // This function is used by ScriptAssembler 
    public virtual void Initialize()
    {
        command = GetComponent<Command>();
        body = GetComponent<Body>();
        state = GetComponent<State>();
    }
}
