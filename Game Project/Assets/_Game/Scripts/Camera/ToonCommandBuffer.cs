using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

/*
#if UNITY_EDITOR

// Used to make speed and height variables calculate each other automatically
[CustomEditor(typeof(ToonCommandBuffer))]
public class ToonCommandBufferEditor : Editor
{
	
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		
		ToonCommandBuffer cBuffer = (ToonCommandBuffer)target;
		
		if( GUILayout.Button("ToggleScene") )
		{
			Shader shader = cBuffer.SceneOn ? cBuffer.shader : null;
			SceneView current = SceneView.lastActiveSceneView;
			if( current != null )
			{
				Debug.Log("Changed");
				current.SetSceneViewShaderReplace(shader, null);
				cBuffer.SceneOn = !cBuffer.SceneOn;
			}
			else
				Debug.Log("NOT Changed");
				
		}
	}
}
#endif
*/
[ExecuteInEditMode]
public class ToonCommandBuffer : MonoBehaviour 
{


	public Shader shader;
	Material mat;

	[HideInInspector]
	public bool SceneOn;

	void OnRenderImage (RenderTexture src, RenderTexture dst)
	{


		if( mat == null )
			mat = new Material(shader);
		
		Graphics.Blit(src, dst, mat);
		
	}
	
}
