using Player;
using UnityEngine;

public class InventoryItem : MouseReaction
{
    public Player.Inventory.Object obj;

    private void Start()
    {
        Player.Inventory.Inventory.OnItemAdded += OnItemAddedToInventory;
    }

    private void OnItemAddedToInventory(GameObject addedObject)
    {
        Destroy(addedObject);
    }

    public override void OnInteractStart()
    {
        Player.Inventory.Inventory.PickUpFromWorld?.Invoke(gameObject);

    }

    private void OnDestroy()
    {
        Player.Inventory.Inventory.OnItemAdded -= OnItemAddedToInventory;
    }
}
