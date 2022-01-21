using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{
    public class InventoryManager : MonoBehaviour
    {
        #region singleton pattern
        public static InventoryManager instance { get; set; }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
            }
        }
        #endregion

        public Inventory inventory = new Inventory(); 
    }

    [System.Serializable]
    public class Inventory
    {
        [SerializeField]
        private List<Item> items = new List<Item>();

        // if returns true, add was successful 
        public bool AddItem(Item item)
        {
            if (item.GetIsUnique())
            {
                if (CheckForItem(item))
                {
                    return false; // item is already in inventory; don't add duplicate
                }
            }

            items.Add(item);
            return true; 
        }

        // if returns false, remove was unsuccessful 
        public bool RemoveItem(Item item)
        {
            return items.Remove(item); 
        }
        public bool RemoveItem(string itemName)
        {
            Item i = items.Find(x => x.GetName().Equals(itemName));
            return RemoveItem(i);
        }

        public bool CheckForItem(Item item) 
        {
            return items.Exists(x => x.Equals(item)); 
        }
        public bool CheckForItem(string itemName)
        {
            return items.Exists(x => x.GetName().Equals(itemName));
        }

        
    }
}