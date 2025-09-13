using Cinematics;
using System;
using UnityEngine;
using static UnityEditor.Timeline.Actions.MenuPriority;

namespace Player.Gameplay.ClickableItems
{

    public class StorageContainer : ItemDependent
    {

        public override void OnInteractStart()
        {
            var selected = InteractionController.Instance.ItemSelected;

            if (touched)
            {
                dialog.text = "The storage container is already open";
                OpenDialog();
                return;
            }

            if (!HasItemNeeded(selected))
            {
                OpenDialog();
                return;
            }

            transform.localScale = new Vector3(1, 0.2f, 1);
            dialog.text = "You opened the storage container";
            OpenDialog();
            DialogBoxController.OnDialogEnds += SetTouched;
            Inventory.Inventory.RemoveItem?.Invoke(selected);
        }


        private void OnDestroy()
        {
            DialogBoxController.OnDialogEnds -= SetTouched;
        }
    }
}