using UnityEngine;
using System.Collections;

public class KillBox : MonoBehaviour {

	private Vector3 respawnPos;
    private CheckpointManager checkPointManager;

    // Use this for initialization
    void Start () {
        checkPointManager = GameObject.Find("CheckpointManager").GetComponent<CheckpointManager>();
    }

    void OnTriggerEnter2D(Collider2D other) {
		GameObject unit = other.transform.parent.gameObject;
        if (unit.tag == "Player")
            unit.transform.position = checkPointManager.currentCheckPoint.transform.position;
    }

}
