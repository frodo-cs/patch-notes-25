using Cinematics;
using System;

namespace Player.Gameplay.ClickableItems
{
    public class RatNest : ItemDroppables
    {
        public override void OnInteractStart()
        {
            var selected = InteractionController.Instance.ItemSelected;

            if (!HasItemNeeded(selected))
            {
                OpenDialog();
                return;
            }

            if (touched && droppables.Length == 0)
            {
                dialog.text = "The rats are already distracted";
                OpenDialog();
                return;
            }

            if (!CanAddItems())
            {
                dialog.text = "Your inventory doesn't have enough space";
                OpenDialog();
                return;
            }

            dialog.text = "Distracted by the meal";
            OpenDialog();
            DialogBoxController.OnDialogEnds += AddItems;
        }

        protected override void AddItems()
        {
            var selected = InteractionController.Instance.ItemSelected;
            Inventory.Inventory.RemoveItem?.Invoke(selected);
            Inventory.Inventory.AddItems?.Invoke(droppables);
            droppables = new Inventory.Object[0];
            SetTouched();
        }

        private void OnDestroy()
        {
            DialogBoxController.OnDialogEnds -= AddItems;
        }

    }
}
