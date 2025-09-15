using Cinematics;
using Player;
using Player.Gameplay;
using Player.Inventory;
using Player.Puzzles;
using UnityEngine;
using static PersistentData;

public class SimonButton : ItemDependent
{
    [SerializeField] private Simon simon;
    private bool active = true;
    [HideInInspector]
    public bool Active
    {
        get { return active; }
        set { active = value; }
    }
    private bool gameEnded = false;

    protected override void Start()
    {
        LoadGameStatus();
        if (gameEnded)
        {
            Active = false;
        }
        Simon.OnSimonComplete += EndDialog;
    }

    private void EndDialog()
    {
        gameEnded = true;
        SaveGameStatus();
        RemoveItems();
        dialog.text = "A door has opened";
        OpenDialog();
    }

    private void RemoveItems()
    {
        var selectedIndexes = InteractionController.Instance.SelectedIndexes;

        if (selectedIndexes.Count == 0)
            return;

        int index = selectedIndexes[0];
        var item = Inventory.Instance.GetItems()[index];
        int amount = item.amount;

        for (int i = 0; i < amount; i++)
        {
            Inventory.RemoveItem?.Invoke(item.obj);
        }
    }

    protected void SaveGameStatus()
    {
        Save(itemId, new BoolData("DoorOpened", gameEnded));
    }

    protected void LoadGameStatus()
    {
        var data = GetData(itemId, "DoorOpened") as BoolData;
        if (data != null)
        {
            gameEnded = data.value;
        }
    }

    private void OnMouseDown()
    {
        if (DialogBoxController.IsDialogRunning?.Invoke() == true)
            return;
        bool hasItems = HasItemsNeeded();
        if (Active && hasItems)
        {
            simon.Play();
        } else if (Active && !hasItems)
        {
            dialog.text = "You don't have the required items";
            OpenDialog();
            return;
        }
    }

    private void OnDestroy()
    {
        Simon.OnSimonComplete -= EndDialog;
    }
}
