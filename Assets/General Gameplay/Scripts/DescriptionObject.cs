using Cinematics;
using Player;
using UnityEngine;

public class DescriptionObject : MouseReaction
{
    [SerializeField] Dialog dialog;

    public override void OnInteractStart() {

        var a = DialogBoxController.IsDialogRunning?.Invoke();
        if(a != null && !a.Value)
            DialogBoxController.PlayDialog?.Invoke(dialog);
    }
}
