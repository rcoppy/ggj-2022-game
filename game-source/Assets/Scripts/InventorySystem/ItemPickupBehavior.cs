using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{
    [RequireComponent(typeof(Interactable))]
    public class ItemPickupBehavior : MonoBehaviour
    {
        public Item item;
        public bool destroyOnPickup = true; 

        // this is a callback-- call from 'Interactable' component
        public void PickupItem()
        {
            InventoryManager.instance.inventory.AddItem(item); 

            if (destroyOnPickup)
            {
                Destroy(gameObject); 
            }

            Interactable.EndInteraction(); // release the interaction
        }
    }
}