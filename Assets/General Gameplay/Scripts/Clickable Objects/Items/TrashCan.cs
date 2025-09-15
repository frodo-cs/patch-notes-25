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
                dialog.text = "There's only trash in this trash can";
                OpenDialog();
                return;
            }

            if (!CanAddItems())
            {
                dialog.text = "I have no room for more trash";
                OpenDialog();
                return;
            }

            dialog.text = "Bet these could come handy";
            OpenDialog();
            DialogBoxController.OnDialogEnds += AddItems;
        }

        protected override void AddItems()
        {
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