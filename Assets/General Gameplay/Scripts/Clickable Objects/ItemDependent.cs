using Cinematics;
using Player;
using Player.Inventory;
using UnityEngine;

public class ItemDependent : DescriptionObject 
{
    [SerializeField] private Player.Inventory.Object neededObject;

    public override void OnInteractStart()
    {
        var a = DialogBoxController.IsDialogRunning?.Invoke();
        if (a != null && !a.Value)
            DialogBoxController.PlayDialog?.Invoke(dialog);
    }
}
