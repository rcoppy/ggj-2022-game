using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InventorySystem;

[RequireComponent(typeof(Interactable))]
public class ItemRemoveBehavior : MonoBehaviour {

    public Item itemToRemove; 

    public void RemoveItem()
    {
        InventoryManager.instance.inventory.RemoveItem(itemToRemove); 
    }
}
