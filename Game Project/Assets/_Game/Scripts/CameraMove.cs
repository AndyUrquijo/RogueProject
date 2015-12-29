using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(CameraMove))]
public class CameraMoveEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CameraMove cameraMove = (CameraMove)target;
        if( GUILayout.Button("Update") )
        {
	        GameObject player = GameObject.FindGameObjectWithTag("Player");
            cameraMove.transform.position = player.transform.position + cameraMove.offset;
            cameraMove.transform.rotation = Quaternion.Euler(cameraMove.rotation);
        }
    }
}
#endif

public class CameraMove : MonoBehaviour {


	public float speed;
	public float range;
    public Vector3 rotation;
    public Vector3 offset;
    public float preemption = 10;
    private Vector3 velocity;

	Rigidbody2D player;


	public void FixedUpdate () {
        
		if( player == null )
			player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Rigidbody2D>();

		Vector3 destination = player.transform.position + offset;
        Vector3 preemptionDistance = new Vector3(player.velocity.x, player.velocity.y, 0) * preemption;
        destination += preemptionDistance;
		Vector3 displace = destination - transform.position;
		displace = Vector3.ClampMagnitude (displace, range);

		transform.position += displace * (speed / range) * Time.fixedDeltaTime;
        transform.rotation = Quaternion.Euler(rotation);
	}

}
