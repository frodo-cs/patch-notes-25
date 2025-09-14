using Cinematics;
using Player.Puzzles;
using UnityEngine;
using static PersistentData;

namespace Player.Gameplay.ClickableItems
{
    public class AccessDoor : DescriptionObject
    {
        [SerializeField] GameObject puzzlePrefab;
        [SerializeField] int sceneToMove;
        [SerializeField] int password;
        private GameObject puzzleInstance;
        private bool isDoorOpen = false;

        protected override void Start()
        {
            base.Start();
            LoadDoorOpened();
        }

        public override void OnInteractStart()
        {
            if (DialogBoxController.IsDialogRunning?.Invoke() == true)
                return;

            if (isDoorOpen)
            {
                GoToScene();
            } else
            {
                dialog.text = "There's a keypad";
                OpenDialog();
                DialogBoxController.OnDialogEnds += InstantiatePuzzle;
            }
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

            var input = puzzleInstance.GetComponent<NumberInput>();
            input.SetValueToMatch(password.ToString());

            NumberInput.OnPasswordCorrect += OnPasswordCorrect;
            NumberInput.OnExitPuzzle += OnDestroyPuzzle;
        }

        private void OnDestroyPuzzle()
        {
            gameObject.GetComponent<Collider2D>().enabled = true;
            NumberInput.OnPasswordCorrect -= OnPasswordCorrect;
            NumberInput.OnExitPuzzle -= OnDestroyPuzzle;

            if (puzzleInstance != null)
                Destroy(puzzleInstance);
        }

        private void OnPasswordCorrect()
        {
            isDoorOpen = true;
            SaveDoorOpened();
            dialog.text = "Correct password";
            OpenDialog();
            DialogBoxController.OnDialogEnds += GoToScene;
        }

        private void GoToScene()
        {
            OnDestroyPuzzle();
            var result = ChangeScene.LoadScene?.Invoke(sceneToMove);
            if (result == null || !result.Value)
                Debug.LogWarning("Failed to change scene");
        }

        private void SaveDoorOpened()
        {
            Save(itemId, new BoolData("DoorOpened", isDoorOpen));
        }

        protected void LoadDoorOpened()
        {
            var data = GetData(itemId, "DoorOpened") as BoolData;
            if (data != null)
            {
                isDoorOpen = data.value;
            }
        }

        private void OnDestroy()
        {
            DialogBoxController.OnDialogEnds -= InstantiatePuzzle;
            DialogBoxController.OnDialogEnds -= GoToScene;
            NumberInput.OnPasswordCorrect -= OnPasswordCorrect;
            NumberInput.OnExitPuzzle -= OnDestroyPuzzle;
        }
    }
}
