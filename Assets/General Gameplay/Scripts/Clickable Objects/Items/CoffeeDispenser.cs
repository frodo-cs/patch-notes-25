using Cinematics;
using System;
using UnityEngine;

namespace Player.Gameplay.ClickableItems
{

    public class CoffeeDispenser : ItemDependent
    {
        public override void OnInteractStart()
        {
            if (DialogBoxController.IsDialogRunning?.Invoke() == true)
                return;

            var selected = InteractionController.Instance.ItemSelected;

            if (touched)
            {
                dialog.text = "Huh, seems that was all";
                OpenDialog();
                return;
            }

            if (!HasItemNeeded(selected))
            {
                OpenDialog();
                return;
            }

            dialog.text = "Nothing like an early payday";
            OpenDialog();
            Inventory.Inventory.RemoveItem?.Invoke(selected);
            DialogBoxController.OnDialogEnds += SaveTouched;
        }

        private void OnDestroy()
        {
            DialogBoxController.OnDialogEnds -= SaveTouched;
        }
    }
}