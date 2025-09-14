using Cinematics;
using System;
using UnityEngine;

namespace Player.Gameplay.ClickableItems
{
    public class Safe : ItemDroppables
    {
        [SerializeField] GameObject puzzleObject;
        private GameObject puzzleInstance;
        private bool isOpen = false;

        public override void OnInteractStart()
        {
            if (DialogBoxController.IsDialogRunning?.Invoke() == true)
                return;

            var selected = InteractionController.Instance.ItemSelected;

            if (!HasItemNeeded(selected) && !isOpen)
            {
                puzzleInstance = Instantiate(
                    puzzleObject,
                    new Vector3(0, 0, 0),
                    Quaternion.identity
                );
                puzzleInstance.transform.parent = transform;

                Puzzles.NumberInput.OnPasswordCorrect += OpenSafe;
                Puzzles.NumberInput.OnExitPuzzle += OnExitClicked;
            } else if (HasItemNeeded(selected) && !isOpen)
            {
                OpenSafe();
            } else if (isOpen)
            {
                dialog.text = "The safe is already open and empty";
                OpenDialog();
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
        }

        private void OpenSafe()
        {
            isOpen = true;

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
