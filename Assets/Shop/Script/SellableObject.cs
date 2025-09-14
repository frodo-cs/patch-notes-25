using UnityEngine;


namespace Player.Inventory {
    [CreateAssetMenu(fileName = "New Sellable Object", menuName = "General Gameplay/Inventory Sellable Object")]
    public class SellableObject : Object {
        public int sellingPrice = 20;
    }

}