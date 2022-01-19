using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{
    [CreateAssetMenu(fileName = "Item", menuName = "Inventory/Item", order = 1)]
    public class Item : ScriptableObject
    {

        [SerializeField]
        private string itemName = "Item";
        [SerializeField]
        private Sprite sprite;
        [SerializeField]
        private bool isUnique = false; 


        public string GetName()
        {
            return itemName;
        }

        public Sprite GetSprite()
        {
            return sprite;
        }

        public bool GetIsUnique()
        {
            return isUnique; 
        }

        public bool Equals(Item other)
        {
            if (other == null) return false;
            return (this.itemName.Equals(other.GetName()));
        }
    }
}
