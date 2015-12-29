using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif 

[ExecuteInEditMode]
public class FollowScene : MonoBehaviour {

    void Update() { }

#if UNITY_EDITOR
	void OnRenderObject()
    {
        SceneView scene = SceneView.lastActiveSceneView;
        if (scene)
        {
            transform.position = scene.camera.transform.position;
            transform.rotation = scene.camera.transform.rotation;
        }
    }
#endif
}
