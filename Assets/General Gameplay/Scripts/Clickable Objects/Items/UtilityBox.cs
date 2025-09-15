using Cinematics;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static PersistentData;

namespace Player.Gameplay.ClickableItems
{
    public class UtilityBox : ItemDependent
    {
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

        public override void OnInteractStart()
        {
            if (DialogBoxController.IsDialogRunning?.Invoke() == true)
                return;

            if (!lightsOn)
            {
                var selected = InteractionController.Instance.ItemSelected;

                if (!touched && !HasItemsNeeded())
                {
                    dialog.text = "Looks like it's strapped shut like Fort Knox. I could try prying it with a crowbar and hope it breaks loose";
                    OpenDialog();
                    return;
                }

                dialog.text = "Bet one of these little switches powers up something";
                OpenDialog();
                DialogBoxController.OnDialogEnds += InstantiatePuzzle;
            } else
            {
                dialog.text = "Lights are already on, why do I keep playing with this?";
                OpenDialog();
            }
        }

        private void OpenUtilityBox()
        {
            touched = true;
            SaveTouched();
            DialogBoxController.OnDialogEnds -= OpenUtilityBox;
        }

        private void InstantiatePuzzle()
        {
            touched = true;
            SaveTouched();
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

            Puzzles.SwitchBoardPuzzle.OnPasswordCorrect -= OnPasswordCorrect;
            Puzzles.SwitchBoardPuzzle.OnExitClicked -= OnDestroyPuzzle;
            Save(itemId, new BoolData("LightsOn", true));

            if (puzzleInstance != null)
                Destroy(puzzleInstance);

            dialog.text = "Turn on lights";
            OpenDialog();
        }

        private void OnDestroy()
        {
            DialogBoxController.OnDialogEnds -= OpenUtilityBox;
            DialogBoxController.OnDialogEnds -= InstantiatePuzzle;

            Puzzles.SwitchBoardPuzzle.OnPasswordCorrect -= OnPasswordCorrect;
            Puzzles.SwitchBoardPuzzle.OnExitClicked -= OnDestroyPuzzle;
        }
    }
}
