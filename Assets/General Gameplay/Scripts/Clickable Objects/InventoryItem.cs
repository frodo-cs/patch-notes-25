using Cinematics;
using UnityEngine;
using Player;

public class InventoryItem : MouseReaction
{
    public Player.Inventory.Object obj;

    public override void OnInteractStart()
    {
        Player.Inventory.Inventory.PickUpItem?.Invoke(gameObject);
    }

}
