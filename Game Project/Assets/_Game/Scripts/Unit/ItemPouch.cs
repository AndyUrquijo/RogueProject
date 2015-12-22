using UnityEngine;
using System.Collections;

public class ItemPouch : MonoBehaviour {

    public int[] items;

    public GameObject slash;

	void Start () {
	    items = new int[(int)ItemType.COUNT];
	}
	
    
    


    public void StoreItem(ItemType type)
    {
        switch (type)
        {
            case ItemType.SWORD:
                GetComponent<Attacker>().attacks[0] = slash;
                break;
        }

        items[(int)type]++;
    }

}
