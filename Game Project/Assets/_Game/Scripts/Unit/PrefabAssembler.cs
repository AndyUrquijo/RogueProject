using UnityEngine;
using System.Collections;
using System.Collections.Generic;


#if UNITY_EDITOR

using UnityEditor;
// Used to make speed and height variables calculate each other automatically
[CustomEditor(typeof(PrefabAssembler))]
public class PrefabAssemblerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PrefabAssembler prefabAssembler = (PrefabAssembler)target;
        GUILayout.BeginHorizontal();
        if( GUILayout.Button("Update") )
            prefabAssembler.Assemble();
        if (GUILayout.Button("Clear"))
            prefabAssembler.ClearInstances();
        GUILayout.EndHorizontal();

    }
}
#endif


[ExecuteInEditMode]
public class PrefabAssembler : MonoBehaviour {


    public List<GameObject> prefabs;
    [HideInInspector]
    public List<GameObject> instances;


    void Start () {
        Assemble();	
	}
	
    public void ClearInstances()
    {
        if(instances == null)
            instances = new List<GameObject>();

        for (int i = 0; i < instances.Count; i++)
        {
            GameObject.DestroyImmediate(instances[i]);
            instances[i] = null;
        }


        instances.Clear();

        // Find un linked copies in the object
        foreach (GameObject prefab in prefabs)
        {
            while(true)
            {
                Transform child = transform.Find(prefab.name);
                if (child)
                    GameObject.DestroyImmediate(child.gameObject);
                else break;
            } 

        }
    }

    public void Assemble()
    {
        ClearInstances();

        foreach (GameObject prefab in prefabs)
        {
            GameObject instance = GameObject.Instantiate(prefab);
            instance.name = prefab.name;
            instance.transform.position = transform.position;
            instance.transform.parent = transform;
            instances.Add(instance);
        }

    }

    public void Update()
    {
        for (int i = 0; i < instances.Count; i++)
        {
            if(instances[i] == null)
                instances.RemoveAt(i);
        }
    }

}
