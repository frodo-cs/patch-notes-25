using Cinematics;
using UnityEngine;

namespace Player.Gameplay.ClickableItems
{
    public class Car : ItemDroppables
    {
        [Space(5)]
        [SerializeField] protected Texture2D openCar;
        [Space(5)]
        [SerializeField] SpriteRenderer spriteRenderer;

        protected override void Start()
        {
            base.Start();
if (touched)
            {
                spriteRenderer.sprite = Utilities.ToSprite(openCar);
            }
        }

        public override void OnInteractStart()
        {
            if (DialogBoxController.IsDialogRunning?.Invoke() == true)
                return;

            if (!touched || droppables.Length > 0)
            {
                var selected = InteractionController.Instance.ItemSelected;
                if (!HasItemNeeded(selected))
                {
                    OpenDialog();
                    return;
                }


                if (!CanAddItems())
                {
                    dialog.text = "Your inventory doesn't have enough space";
                    OpenDialog();
                    return;
                }

                if (droppables.Length > 0 && HasItemNeeded(selected))
                {
                    spriteRenderer.sprite = Utilities.ToSprite(openCar);
                    dialog.text = "You found some items in the glove box";
                    OpenDialog();
                    DialogBoxController.OnDialogEnds += AddItems;
                }
            }
        }

        protected override void AddItems()
        {
            var selected = InteractionController.Instance.ItemSelected;
            InteractionController.Instance.ClearSelection();
            Inventory.Inventory.RemoveItem?.Invoke(selected);
            Inventory.Inventory.AddItems?.Invoke(droppables);
            droppables = new Inventory.Object[0];
            touched = true;
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
