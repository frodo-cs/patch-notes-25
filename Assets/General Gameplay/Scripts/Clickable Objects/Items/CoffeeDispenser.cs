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
                dialog.text = "The coffee dispenser is empty";
                OpenDialog();
                return;
            }

            if (!HasItemNeeded(selected))
            {
                OpenDialog();
                return;
            }

            dialog.text = "You received x amount of money";
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