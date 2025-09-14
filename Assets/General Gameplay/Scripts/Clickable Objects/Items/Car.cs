using Cinematics;

namespace Player.Gameplay.ClickableItems
{
    public class Car : ItemDroppables
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
                dialog.text = "There' nothing else here";
                OpenDialog();
                return;
            }

            if (!CanAddItems())
            {
                dialog.text = "Your inventory doesn't have enough space";
                OpenDialog();
                return;
            }

            dialog.text = "You found some items in the glove box";
            OpenDialog();
            DialogBoxController.OnDialogEnds += AddItems;
        }

        protected override void AddItems()
        {
            var selected = InteractionController.Instance.ItemSelected;
            InteractionController.Instance.ClearSelection();
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
