using Cinematics;
using UnityEngine;

namespace Player.Gameplay.ClickableItems
{
    public class Trash : ItemDroppables
    {
        public override void OnInteractStart()
        {
            if (DialogBoxController.IsDialogRunning?.Invoke() == true)
                return;

            if (!touched || droppables.Length > 0)
            {
                if (!CanAddItems())
                {
                    dialog.text = "No room to haul more crap arounde";
                    OpenDialog();
                    return;
                }

                dialog.text = "Just some trash... figures. Might still come in handy";
                OpenDialog();
                DialogBoxController.OnDialogEnds += AddItems;
            }
        }

        protected override void AddItems()
        {
            Inventory.Inventory.AddItems?.Invoke(droppables);
            droppables = new Inventory.Object[0];
            gameObject.GetComponent<Collider2D>().enabled = false;
            SaveDroppables();
            SaveTouched();
        }

        private void OnDestroy()
        {
            DialogBoxController.OnDialogEnds -= AddItems;
        }
    }
}