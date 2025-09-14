using Cinematics;
using UnityEngine;

namespace Player.Gameplay.ClickableItems
{
    public class Safe : ItemDroppables
    {
        [SerializeField] GameObject puzzleObject;

        public void Start()
        {
            puzzleObject.SetActive(false);
        }

        public override void OnInteractStart()
        {
            if (DialogBoxController.IsDialogRunning?.Invoke() == true)
                return;

            var selected = InteractionController.Instance.ItemSelected;

            if (!HasItemNeeded(selected))
            {
                puzzleObject.SetActive(true);
                Puzzles.NumberInput.OnPasswordCorrect += OpenStorage;
            } else if (HasItemNeeded(selected))
            {
                OpenStorage();
            }
        }

        protected override void AddItems()
        {
            var selected = InteractionController.Instance.ItemSelected;
            if (selected && HasItemNeeded(selected))
            {
                Debug.Log("Used item on safe");
                InteractionController.Instance.ClearSelection();
                Inventory.Inventory.RemoveItem?.Invoke(selected);
            }

            gameObject.GetComponent<Collider2D>().enabled = false;
            puzzleObject.SetActive(false);
            Inventory.Inventory.AddItems?.Invoke(droppables);
            droppables = new Inventory.Object[0];
        }

        private void OpenStorage()
        {
            puzzleObject.SetActive(false);
            dialog.text = "You opened the safe";
            OpenDialog();
            DialogBoxController.OnDialogEnds += AddItems;
        }


        private void OnDestroy()
        {
            DialogBoxController.OnDialogEnds -= AddItems;
            Puzzles.NumberInput.OnPasswordCorrect -= OpenStorage;
        }


    }

}
