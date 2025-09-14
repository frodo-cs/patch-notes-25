using Cinematics;
using System;
using UnityEngine;

namespace Player.Gameplay.ClickableItems
{
    public class Safe : ItemDroppables
    {
        [SerializeField] GameObject puzzleObject;
        private GameObject puzzleInstance;

        public override void OnInteractStart()
        {
            if (DialogBoxController.IsDialogRunning?.Invoke() == true)
                return;

            var selected = InteractionController.Instance.ItemSelected;

            if (!HasItemNeeded(selected))
            {
                puzzleObject.SetActive(true);
                Puzzles.NumberInput.OnPasswordCorrect += OpenSafe;
            } else if (HasItemNeeded(selected))
            {
                OpenSafe();
            }
        }

        private void OnExitClicked()
        {
            Puzzles.NumberInput.OnPasswordCorrect -= OpenSafe;
            Puzzles.NumberInput.OnExitPuzzle -= OnExitClicked;

            if (puzzleInstance != null)
                Destroy(puzzleInstance);
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

            Inventory.Inventory.AddItems?.Invoke(droppables);
            droppables = new Inventory.Object[0];
            SaveDroppables();
            SaveTouched();
        }

        private void OpenSafe()
        {
            puzzleObject.SetActive(false);
            dialog.text = "You opened the safe";
            OpenDialog();

            DialogBoxController.OnDialogEnds += AddItems;

            if (puzzleInstance != null)
                Destroy(puzzleInstance);

            Puzzles.NumberInput.OnPasswordCorrect -= OpenSafe;
            Puzzles.NumberInput.OnExitPuzzle -= OnExitClicked;
        }

        private void OnDestroy()
        {
            DialogBoxController.OnDialogEnds -= AddItems;
            Puzzles.NumberInput.OnPasswordCorrect -= OpenSafe;
            Puzzles.NumberInput.OnExitPuzzle -= OnExitClicked;
        }
    }
}
