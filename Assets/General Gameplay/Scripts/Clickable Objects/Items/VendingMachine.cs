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
                dialog.text = "It says it accepts token only, huh? What was the problem with the good old greenback? I guess i should keep a eye out for these token things";
                OpenDialog();
                return;
            }

            if (touched && droppables.Length == 0)
            {
                dialog.text = "This old thing seems empty";
                OpenDialog();
                return;
            }

            if (!CanAddItems())
            {
                dialog.text = "It seems I ran out of space";
                OpenDialog();
                return;
            }

            dialog.text = "These tokens seemed to do the trick";
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