using Cinematics;
using UnityEngine;

namespace Player
{
    public class InventoryItem : MouseReaction
    {
        public Inventory.Object obj;

        public override void OnInteractStart()
        {
            Inventory.Inventory.PickUpItem?.Invoke(gameObject);
        }

    }
}