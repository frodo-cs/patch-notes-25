using Cinematics;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Player.Gameplay.ClickableItems
{
    public class UtilityBox : ItemDependent
    {
        [SerializeField] SpriteRenderer spriteRenderer;
        [SerializeField] Collider2D col;

        private List<Inventory.Inventory.InventoryData> GetItemsNeed()
        {
            var selectedIndexes = InteractionController.Instance.SelectedIndexes;
            var inventoryItems = Inventory.Inventory.Instance.GetItems();
            var selectedItems = new List<Inventory.Inventory.InventoryData>();

            foreach (var index in selectedIndexes)
            {
                if (index >= 0 && index < inventoryItems.Count)
                {
                    selectedItems.Add(inventoryItems[index]);
                }
            }

            return selectedItems;
        }

        private bool HasItemsNeeded()
        {
            var selectedItems = GetItemsNeed();

            foreach (var needed in neededObjects)
            {
                if (!selectedItems.Any(item => item.obj == needed && item.amount > 0))
                    return false;
            }

            return true;
        }

        public override void OnInteractStart()
        {
            if (DialogBoxController.IsDialogRunning?.Invoke() == true)
                return;

            var selected = InteractionController.Instance.ItemSelected;

            if (!HasItemsNeeded())
            {
                dialog.text = "You don't have the required items";
                OpenDialog();
                return;
            }

            dialog.text = "You opened the utility box";
            OpenDialog();
            DialogBoxController.OnDialogEnds += OpenUtilityBox;
        }

        protected void OpenUtilityBox()
        {
            spriteRenderer.color = new Color(170f / 255f, 90f / 255f, 90f / 255f, 0.4f);
            col.enabled = false;
        }

        private void OnDestroy()
        {
            DialogBoxController.OnDialogEnds -= OpenUtilityBox;
        }

    }
}
