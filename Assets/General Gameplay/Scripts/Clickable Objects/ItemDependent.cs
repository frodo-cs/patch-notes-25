using Cinematics;
using Player;
using Player.Inventory;
using System.Collections.Generic;
using UnityEngine;

public class ItemDependent : DescriptionObject
{
    [SerializeField] private Player.Inventory.Object[] neededObjects;
    HashSet<Player.Inventory.Object> neededSet;

    private void Awake()
    {
        neededSet = new HashSet<Player.Inventory.Object>(neededObjects);
    }

    public override void OnInteractStart()
    {
        var selected = InteractionController.Instance.ItemSelected;
        if (selected && neededSet.Contains(selected))
        {
            // Open door
        } else
        {

            var a = DialogBoxController.IsDialogRunning?.Invoke();
            if (a != null && !a.Value)
                DialogBoxController.PlayDialog?.Invoke(dialog);
        }
    }
}
