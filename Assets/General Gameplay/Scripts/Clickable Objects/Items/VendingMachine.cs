using Cinematics;
using System.Linq;
using UnityEngine;

namespace Player.Gameplay.ClickableItems
{
    public class VendingMachine : ItemDroppables
    {
        public override void OnInteractStart()
        {
            if (DialogBoxController.IsDialogRunning?.Invoke() == true)
                return;

            var selected = InteractionController.Instance.ItemSelected;

            if (!HasItemNeeded(selected))
            {
                dialog.text = "You need a token to use the vending machine";
                OpenDialog();
                return;
            }

            if (touched && droppables.Length == 0)
            {
                dialog.text = "No more items";
                OpenDialog();
                return;
            }

            if (!CanAddItems())
            {
                dialog.text = "Your inventory doesn't have enough space";
                OpenDialog();
                return;
            }

            dialog.text = "Vending machine dispenses an item";
            OpenDialog();

            DialogBoxController.OnDialogEnds -= AddItems;
            DialogBoxController.OnDialogEnds += AddItems;
        }

        protected override void AddItems()
        {
            DialogBoxController.OnDialogEnds -= AddItems;

            if (droppables.Length == 0)
                return;

            var selected = InteractionController.Instance.ItemSelected;
            Inventory.Inventory.AddItem?.Invoke(droppables[0]);

            droppables = droppables.Skip(1).ToArray();
            SaveDroppables();

            InteractionController.Instance.ClearSelection();
            Inventory.Inventory.RemoveItem?.Invoke(selected);
            SaveTouched();
        }

        protected void OnDestroy()
        {
            DialogBoxController.OnDialogEnds -= AddItems;
        }
    }
}