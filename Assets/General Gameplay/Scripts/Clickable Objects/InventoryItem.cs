using Cinematics;
using Player;
using UnityEngine;
using static PersistentData;

public class InventoryItem : MouseReaction
{
    [SerializeField] string itemId;
    public Player.Inventory.Object obj;
    private bool hasBeenPicked;

    private void Start()
    {
        LoadPicked();
        if(hasBeenPicked)
        {
            Destroy(gameObject);
        }
        Player.Inventory.Inventory.OnItemAdded += OnItemAddedToInventory;
    }


    private void OnItemAddedToInventory(GameObject addedObject)
    {
        hasBeenPicked = true;
        SavePicked();
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

    protected void SavePicked()
    {
        Save(itemId, new BoolData("Picked", hasBeenPicked));
    }

    protected void LoadPicked()
    {
        var data = GetData(itemId, "Picked") as BoolData;
        if (data != null)
        {
            hasBeenPicked = data.value;
        }

    }
}
