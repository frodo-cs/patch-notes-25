using Cinematics;
using UnityEngine;

namespace Player.Gameplay.ClickableItems
{
    public class Trash : ItemDroppables
    {
        protected override void Start()
        {
            base.Start();
            if (touched)
            {
                gameObject.GetComponent<Collider2D>().enabled = false;
            }
        }

        public override void OnInteractStart()
        {
            if (DialogBoxController.IsDialogRunning?.Invoke() == true)
                return;

            if (!touched && droppables.Length > 0)
            {
                if (!CanAddItems())
                {
                    dialog.text = "No room to haul more crap around";
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
            SoundTable.PlaySound?.Invoke(onPickSound);

            touched = true;
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