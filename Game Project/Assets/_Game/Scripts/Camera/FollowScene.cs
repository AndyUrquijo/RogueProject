using UnityEngine;
using System.Collections;
using UnityEditor;

[ExecuteInEditMode]
public class FollowScene : MonoBehaviour {

    void Update() { }
    void OnRenderObject()
    {
        SceneView scene = SceneView.lastActiveSceneView;
        if (scene)
        {
            transform.position = scene.camera.transform.position;
            transform.rotation = scene.camera.transform.rotation;
        }
    }
}
