using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class AutoTile : MonoBehaviour {

    public Vector2 TileSize;

    private Material material = null;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        Vector2 scale;
        scale.x = transform.lossyScale.x / TileSize.x;
        scale.y = transform.lossyScale.y / TileSize.y;
        Renderer rend = GetComponent<Renderer>();
        if(!material)
            material = new Material(rend.sharedMaterial);
        material.mainTextureScale = scale;
        rend.sharedMaterial = material;
	}
}
