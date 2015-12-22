using UnityEngine;
using System.Collections;
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;

// Used to make speed and height variables calculate each other automatically
[CustomEditor(typeof(Charge))]
public class ChargeEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Charge charge = (Charge)target;

        for (int i = 0; i < charge.results.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            Charge.ChargeResult result = charge.results[i];
            EditorGUILayout.LabelField("Attack ", GUILayout.MaxWidth(50));
            result.attack = (GameObject)EditorGUILayout.ObjectField(result.attack, typeof(GameObject), false);
            EditorGUILayout.LabelField("Duration", GUILayout.MaxWidth(60));
            result.time = EditorGUILayout.FloatField(result.time);
            EditorGUILayout.EndHorizontal();
            charge.results[i] = result;
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("-"))
            charge.results.RemoveAt(charge.results.Count - 1);
        if (GUILayout.Button("+"))
            charge.results.Add(new Charge.ChargeResult());

        EditorGUILayout.EndHorizontal();



    }
}
#endif


public class Charge : MonoBehaviour {

	// The reulting attack is chosen according to the carge duration.
	[System.Serializable]
	public class ChargeResult
	{
		public GameObject attack;
		public float time;
	}

	// The reulting attack is chosen according to the carge duration.
	[HideInInspector]
	public List<ChargeResult> results = new List<ChargeResult>();

	public bool releaseOnEnd;

	private Command command;
	private float timer;
	private int currentAttack;

	private Attacker attacker;
	// Use this for initialization
	void Start () {
		command = transform.parent.GetComponentInChildren<Command>();
		attacker = transform.parent.GetComponentInChildren<Attacker>();
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;

		int nextAttack = currentAttack + 1;
		bool onFinal = results.Count == nextAttack;
		if (!onFinal && results [nextAttack].time < timer)
			currentAttack = nextAttack;

		if (onFinal && releaseOnEnd) {
			command.release = true;
		}

		if (command.release) {
			command.release = false;
			attacker.BeginAttack(results[currentAttack].attack);
			GameObject.Destroy(gameObject);
		}

	}
}










