using Cinematics;

namespace Player.Gameplay.ClickableItems
{
    public class TrashCan : ItemDroppables
    {
        public override void OnInteractStart()
        {
            if (DialogBoxController.IsDialogRunning?.Invoke() == true)
                return;

            if (touched && droppables.Length == 0)
            {
                dialog.text = "The trash can is already empty";
                OpenDialog();
                return;
            }

            if (!CanAddItems())
            {
                dialog.text = "Your inventory doesn't have enough space";
                OpenDialog();
                return;
            }

            dialog.text = "You found some stuff";
            OpenDialog();
            DialogBoxController.OnDialogEnds += AddItems;
        }

        protected override void AddItems()
        {
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