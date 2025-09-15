using Cinematics;
using Player.Puzzles;
using System;
using UnityEngine;

namespace Player.Gameplay.ClickableItems
{
    public class Safe : ItemDroppables
    {
        [SerializeField] GameObject puzzleObject;
        [SerializeField] string password;
        private GameObject puzzleInstance;

        public override void OnInteractStart()
        {
            if (DialogBoxController.IsDialogRunning?.Invoke() == true)
                return;
            var selected = InteractionController.Instance.ItemSelected;

            if (!HasItemNeeded(selected))
            {
                InstantiatePuzzle();
            } else if (HasItemNeeded(selected))
            {
                OpenSafe();
            }
        }

        private void InstantiatePuzzle()
        {
            gameObject.GetComponent<Collider2D>().enabled = false;
            DialogBoxController.OnDialogEnds -= InstantiatePuzzle;

            puzzleInstance = Instantiate(
                puzzleObject,
                Vector3.zero,
                Quaternion.identity,
                transform
            );

            var input = puzzleInstance.GetComponent<NumberInput>();
            input.SetValueToMatch(password.ToString());

            NumberInput.OnPasswordCorrect += OpenSafe;
            NumberInput.OnExitPuzzle += OnExitClicked;
        }

        private void OnExitClicked()
        {
            NumberInput.OnPasswordCorrect -= OpenSafe;
            NumberInput.OnExitPuzzle -= OnExitClicked;

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
            puzzleInstance.SetActive(false);
            dialog.text = "You opened the safe";
            OpenDialog();

            DialogBoxController.OnDialogEnds += AddItems;

            if (puzzleInstance != null)
                Destroy(puzzleInstance);

            NumberInput.OnPasswordCorrect -= OpenSafe;
            NumberInput.OnExitPuzzle -= OnExitClicked;
        }

        private void OnDestroy()
        {
            DialogBoxController.OnDialogEnds -= AddItems;
            NumberInput.OnPasswordCorrect -= OpenSafe;
            NumberInput.OnExitPuzzle -= OnExitClicked;
        }
    }
}
