using Cinematics;
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
        if (addedObject == gameObject)
            Destroy(gameObject);
    }

    public override void OnInteractStart()
    {
        if (DialogBoxController.IsDialogRunning?.Invoke() == true)
            return;

        Player.Inventory.Inventory.PickUpFromWorld?.Invoke(gameObject);
    }

    private void OnDestroy()
    {
        Player.Inventory.Inventory.OnItemAdded -= OnItemAddedToInventory;
    }
}
