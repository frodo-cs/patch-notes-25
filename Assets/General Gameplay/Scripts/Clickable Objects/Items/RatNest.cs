using Cinematics;

namespace Player.Gameplay.ClickableItems
{
    public class RatNest : ItemDroppables
    {
        public override void OnInteractStart()
        {
            if (DialogBoxController.IsDialogRunning?.Invoke() == true)
                return;

            var selected = InteractionController.Instance.ItemSelected;

            if (!HasItemNeeded(selected))
            {
                OpenDialog();
                return;
            }

            if (touched && droppables.Length == 0)
            {
                dialog.text = "Looks like the rats are already busy chowing down";
                OpenDialog();
                return;
            }

            if (!CanAddItems())
            {
                dialog.text = "No room to haul more junk. Gotta clear some space";
                OpenDialog();
                return;
            }

            dialog.text = "Those little demons are too busy with the grub to notice me";
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
            InteractionController.Instance.ClearSelection();
            Inventory.Inventory.RemoveItem?.Invoke(selected);
            Inventory.Inventory.AddItems?.Invoke(droppables);
            droppables = new Inventory.Object[0];
            SaveDroppables();
            SaveTouched();
        }

        private void OnDestroy()
        {
            DialogBoxController.OnDialogEnds -= AddItems;
        }

    }
}
