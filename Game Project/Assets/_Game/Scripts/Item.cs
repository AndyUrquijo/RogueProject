using UnityEngine;
using System.Collections;

public enum ItemType
{
    SWORD,
    COUNT
}



public class Item : EventSubject {

    public ItemType itemType;


    void OnTriggerEnter2D( Collider2D other )
    {
		GameObject unit = other.transform.parent.gameObject;
        if ( unit && unit.tag == "Player")
        {
            unit.GetComponentInChildren<ItemPouch>().StoreItem(itemType);
            GameObject.Destroy(gameObject);
            subject.manager.Notify(type);
            Debug.Log("Item Obtained");
        }
    }
}
