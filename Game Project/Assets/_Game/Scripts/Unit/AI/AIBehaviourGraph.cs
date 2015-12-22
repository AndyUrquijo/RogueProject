using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

/// <summary>
/// This class is used and saved as an asset by the AIEditor window. 
/// AI Controller loads its behaviours from this asset.
/// </summary>
public class AIBehaviourGraph : ScriptableObject
{
    public List<AIBehaviour> behaviours = null;
    public string testString;


    [SerializeField] int behaviourCount;
    
    // NOTE: Add new types here
    //public  List<StartBehaviour> serializerStartBehaviour;
    //public  List<PatrolBehaviour> serializerPatrolBehaviour;
    //public  List<ShootingBehaviour> serializerShootingBehaviour;

    public void OnEnable()
    {
        testString = "OnEnable";
        if (behaviours == null)
        {
            behaviours = new List<AIBehaviour>();
            testString = "Initialize";
        }
        hideFlags = HideFlags.None;
    }

    public void Clear()
    {
        testString = "Clear";
        behaviours.Clear(); // Use to clear invalid data
        Object[] subAssets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(this));
        foreach (Object subAsset in subAssets)
            DestroyImmediate(subAsset);
    }

    public void OnDestroy()
    {
       //Debug.Log("Graph destroyed...");
    }

    public void LoadBehaviours( ref List<AIBehaviour> _behaviours )
    {
        _behaviours = new List<AIBehaviour>();
        Object[] subAssets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(this));
        foreach (Object subAsset in subAssets)
        {
            AIBehaviour behaviour = subAsset as AIBehaviour;
            if(behaviour != null)
                _behaviours.Add(ScriptableObject.Instantiate(behaviour));
        }
        /*
        for (int i = 0; i < this.behaviours.Count; i++)
        {
            AIBehaviour behaviour = this.behaviours[i];
            //_behaviours.Add(ObjectCopier.Clone(this.behaviours[i]));
            _behaviours.Add(ScriptableObject.Instantiate(behaviour));
        } 
        */
    }

    public void AddBehaviour( AIBehaviour behaviour )
    {
        behaviour.index = behaviours.Count;
        behaviours.Add(behaviour);
        behaviour.SetGraphicNode(new Vector2(50, 50));
        behaviour.name = behaviour.Name;
        //AssetDatabase.CreateAsset(behaviour, AssetDatabase.GetAssetPath(this) + "/" + behaviour.name);
        //AssetDatabase.ImportAsset(this);
        AssetDatabase.AddObjectToAsset(behaviour, this);
        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(behaviour));
    }
}
