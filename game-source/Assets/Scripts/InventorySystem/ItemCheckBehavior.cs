using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using InventorySystem; 

namespace InventorySystem
{
    /* call checkItem to see if inspector-assigned item is in inventory;
     * if it is checkItem fires an editor-assigned callback
     * 
     * */
    public class ItemCheckBehavior : MonoBehaviour
    {
        public Item itemToCheck; 
        public UnityEvent positiveCallback; // if the check returns true
        public UnityEvent negativeCallback; // if the check returns false

        public void CheckItem()
        {
            if (InventoryManager.instance.inventory.CheckForItem(itemToCheck))
            {
                positiveCallback.Invoke();
                Debug.Log("item in inventory"); 
            }
            else
            {
                if (negativeCallback != null)
                {
                    negativeCallback.Invoke(); 
                }
                Debug.Log("item not in inventory");
            }
        }
    }
}