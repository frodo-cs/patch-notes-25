using Cinematics;
using UnityEngine;

namespace Player.Gameplay.ClickableItems
{

    public class StorageContainer : ItemDependent
    {
        [SerializeField] SpriteRenderer spriteRenderer;
        [SerializeField] Texture2D texture;
        [SerializeField] GameObject safe;
        [SerializeField] private Transform spawnPoint;

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

            dialog.text = "You emptied the storage container";
            OpenDialog();
            DialogBoxController.OnDialogEnds += OpenStorage;

        }

        private void OpenStorage()
        {
            spriteRenderer.sprite = Utilities.ToSprite(texture);
            GetComponent<Collider2D>().enabled = false;

            var selected = InteractionController.Instance.ItemSelected;
            InteractionController.Instance.ClearSelection();
            Inventory.Inventory.RemoveItem?.Invoke(selected);
            DialogBoxController.OnDialogEnds -= OpenStorage;
            GameObject safeInstance = Instantiate(
                safe,
                spawnPoint != null ? spawnPoint.position : transform.position,
                Quaternion.identity
            );

            safeInstance.transform.parent = transform;
        }

        private void OnDestroy()
        {
            DialogBoxController.OnDialogEnds -= OpenStorage;
        }
    }
}