using UnityEngine;
using System.Collections;

public class LevelHint : MonoBehaviour {

    GameObject text;
    GameObject effect;
    void Start ()
    {
        text = transform.Find("Text").gameObject;
        effect = transform.Find("Effect").gameObject;
        text.SetActive(false);
        effect.SetActive(true);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.transform.parent.tag == "Player")
        {
            text.SetActive(true);
            effect.SetActive(false);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
		if (other.transform.parent.tag == "Player")
        {
            text.SetActive(false);
            effect.SetActive(true);
        }
    }

}
