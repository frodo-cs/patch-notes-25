using Cinematics;
using Player;
using Player.Inventory;
using UnityEngine;
using UnityEngine.Events;

public class ItemDependent : DescriptionObject 
{
    Dialog lockedDialog;

    [Space(5)]
    [SerializeField] private Player.Inventory.Object neededObject;

    [Space(5)]
    [SerializeField] UnityEvent onUnlock;

    public override void OnInteractStart()
    {
        var a = DialogBoxController.IsDialogRunning?.Invoke();
        if (a != null && !a.Value)
            DialogBoxController.PlayDialog?.Invoke(dialog);
    }
}
