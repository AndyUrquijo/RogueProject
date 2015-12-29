using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(AIBehaviourGraph))]
public class AIBehaviourGraphEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		AIBehaviourGraph graph = (AIBehaviourGraph)target;
	}
}

#endif
/// <summary>
/// This class is used and saved as an asset by the AIEditor window. 
/// AI Controller loads its behaviours from this asset.
/// </summary>
public class AIBehaviourGraph : ScriptableObject
{
    public List<AIBehaviour> behaviours = null;

#if UNITY_EDITOR
    public void OnEnable()
    {
        hideFlags = HideFlags.None;
    }

    public void Clear()
    {
		behaviours = new List<AIBehaviour>();
		Object[] subAssets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(this));
        foreach (Object subAsset in subAssets)
			DestroyImmediate(subAsset as AIBehaviour, true);
        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(this));
    }




    public void AddBehaviour( AIBehaviour behaviour )
    {
        behaviour.index = behaviours.Count;
        behaviours.Add(behaviour);
        behaviour.SetGraphicNode(new Vector2(50, 50));
        behaviour.name = behaviour.Name;
		//behaviours.Add(behaviour);
        AssetDatabase.AddObjectToAsset(behaviour, this);
        //AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(this));

		//RefreshList();
    }

	public void RestoreIndices()
	{
		// TODO: Restore transitions
		for (int i = 0; i < behaviours.Count; i++) 
		{
			behaviours[i].graphic.index = i;
			behaviours[i].index = i;
		}
	}


#endif

	public void LoadBehaviours( ref List<AIBehaviour> _behaviours )
	{
		_behaviours = new List<AIBehaviour>();
		foreach (AIBehaviour behaviour in behaviours)
			_behaviours.Add(ScriptableObject.Instantiate(behaviour));
	}

}
