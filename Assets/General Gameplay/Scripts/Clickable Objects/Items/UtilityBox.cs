using Cinematics;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static PersistentData;

namespace Player.Gameplay.ClickableItems
{
    public class UtilityBox : ItemDependent
    {
        [SerializeField] SpriteRenderer spriteRenderer;
        [SerializeField] GameObject puzzlePrefab;
        private GameObject puzzleInstance;
        private bool lightsOn = false;

        protected override void Start()
        {
            base.Start();
            var data = GetData(itemId, "LightsOn") as BoolData;
            if (data != null)
            {
                lightsOn = data.value;
            }
        }

        private List<Inventory.Inventory.InventoryData> GetItemsNeed()
        {
            var selectedIndexes = InteractionController.Instance.SelectedIndexes;
            var inventoryItems = Inventory.Inventory.Instance.GetItems();

            return selectedIndexes
                .Where(index => index >= 0 && index < inventoryItems.Count)
                .Select(index => inventoryItems[index])
                .ToList();
        }

        private bool HasItemsNeeded()
        {
            var selectedItems = GetItemsNeed();
            return neededObjects.All(needed =>
                selectedItems.Any(item => item.obj == needed && item.amount > 0));
        }

        public override void OnInteractStart()
        {
            if (DialogBoxController.IsDialogRunning?.Invoke() == true)
                return;

            if (!lightsOn)
            {
                var selected = InteractionController.Instance.ItemSelected;

                if (!touched && !HasItemsNeeded())
                {
                    dialog.text = "You don't have the required items";
                    OpenDialog();
                    return;
                }

                if (!touched)
                {
                    dialog.text = "You opened the utility box";
                    OpenDialog();
                    DialogBoxController.OnDialogEnds += OpenUtilityBox;
                    return;
                }

                dialog.text = "You find a switch board";
                OpenDialog();
                DialogBoxController.OnDialogEnds += InstantiatePuzzle;
            } else
            {
                dialog.text = "You alerady turned the lights on";
                OpenDialog();
            }
        }

        private void OpenUtilityBox()
        {
            spriteRenderer.color = new Color(170f / 255f, 90f / 255f, 90f / 255f, 0.4f);
            SaveTouched();
            DialogBoxController.OnDialogEnds -= OpenUtilityBox;
        }

        private void InstantiatePuzzle()
        {
            gameObject.GetComponent<Collider2D>().enabled = false;
            DialogBoxController.OnDialogEnds -= InstantiatePuzzle;

            puzzleInstance = Instantiate(
                puzzlePrefab,
                Vector3.zero,
                Quaternion.identity,
                transform
            );

            Puzzles.SwitchBoardPuzzle.OnPasswordCorrect += OnPasswordCorrect;
            Puzzles.SwitchBoardPuzzle.OnExitClicked += OnDestroyPuzzle;
        }

        private void OnDestroyPuzzle()
        {
            gameObject.GetComponent<Collider2D>().enabled = true;
            Puzzles.SwitchBoardPuzzle.OnPasswordCorrect -= OnPasswordCorrect;
            Puzzles.SwitchBoardPuzzle.OnExitClicked -= OnDestroyPuzzle;

            if (puzzleInstance != null)
                Destroy(puzzleInstance);
        }

        private void OnPasswordCorrect()
        {
            dialog.text = "Turn on lights";
            OpenDialog();
            DialogBoxController.OnDialogEnds += OnFinishPuzzle;
        }

        private void OnFinishPuzzle()
        {
            DialogBoxController.OnDialogEnds -= OnFinishPuzzle;
            Puzzles.SwitchBoardPuzzle.OnPasswordCorrect -= OnPasswordCorrect;
            Puzzles.SwitchBoardPuzzle.OnExitClicked -= OnDestroyPuzzle;
            Save(itemId, new BoolData("LightsOn", true));

            if (puzzleInstance != null)
                Destroy(puzzleInstance);
        }

        private void OnDestroy()
        {
            DialogBoxController.OnDialogEnds -= OpenUtilityBox;
            DialogBoxController.OnDialogEnds -= InstantiatePuzzle;
            DialogBoxController.OnDialogEnds -= OnFinishPuzzle;

            Puzzles.SwitchBoardPuzzle.OnPasswordCorrect -= OnPasswordCorrect;
            Puzzles.SwitchBoardPuzzle.OnExitClicked -= OnDestroyPuzzle;
        }
    }
}
