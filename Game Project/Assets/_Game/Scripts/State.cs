using UnityEngine;
using System.Collections;


public class State : MonoBehaviour {
	public enum Type { MOVE, DASH, FALL  };

	public Type state = Type.FALL;
    private Type prevState = Type.FALL;
    private bool toggled;

	private Renderer rend;
	void Start() {
		rend = GetComponentInChildren<Renderer> ();

	}

	void Update(){

        toggled = (prevState != state);
        prevState = state;

		if(state == Type.MOVE)
			rend.material.color = Color.green;
		if(state == Type.FALL)
			rend.material.color = Color.red;
		if(state == Type.DASH)
			rend.material.color = Color.blue;
	}

    public bool Is(Type type)
    {
        return state == type;
    }
    public bool Toggled(Type type)
    {
        return state == type && toggled;
    }
}
