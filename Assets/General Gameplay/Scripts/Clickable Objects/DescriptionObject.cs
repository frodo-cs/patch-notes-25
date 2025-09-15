using Cinematics;
using Player;
using UnityEngine;
using static PersistentData;

public class DescriptionObject : MouseReaction
{
    [Space(5)]
    [SerializeField] protected string itemId;
    [SerializeField] protected Dialog dialog;

    protected bool touched = false;

    protected virtual void Start()
    {
        LoadTouched();
    }

    public override void OnInteractStart() {
        OpenDialog();
    }

    protected void OpenDialog()
    {
        var a = DialogBoxController.IsDialogRunning?.Invoke();
        if (a != null && !a.Value)
            DialogBoxController.PlayDialog?.Invoke(dialog);
    }

    protected virtual void SaveTouched()
    {
        Save(itemId, new BoolData("Touched", touched));
    }

    protected virtual void LoadTouched()
    {
        var data = GetData(itemId, "Touched") as BoolData;
        if (data != null)
        {
            touched = data.value;
        }

    }
}
