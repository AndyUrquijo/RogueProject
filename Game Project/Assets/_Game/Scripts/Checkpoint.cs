using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour
{
    private CheckpointManager checkPointManager;
    private Renderer rend;
    void Start()
    {
        checkPointManager = GameObject.Find("CheckpointManager").GetComponent<CheckpointManager>();
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        if (ReferenceEquals(checkPointManager.currentCheckPoint, this))
            rend.material.color = Color.yellow;
        else
            rend.material.color = Color.gray;

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
            checkPointManager.currentCheckPoint = this;

        
    }
}
