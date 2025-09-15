using Cinematics;
using Player.Puzzles;
using UnityEngine;
using static PersistentData;

namespace Player.Gameplay.ClickableItems
{
    public class AccessDoor : Door
    {
        [SerializeField] GameObject puzzlePrefab;
        [SerializeField] int password;
        private GameObject puzzleInstance;

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
                InstantiatePuzzle();
            }
        }

        private void InstantiatePuzzle()
        {
            gameObject.GetComponent<Collider2D>().enabled = false;
            DialogBoxController.OnDialogEnds -= InstantiatePuzzle;

            Interactable.ChangeState?.Invoke(Interactable.CurrentState.Paused);

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

            Interactable.ChangeState?.Invoke(Interactable.CurrentState.Normal);

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
            dialog.text = "Nice";
            OpenDialog();
            DialogBoxController.OnDialogEnds += GoToScene;
        }

        protected override void GoToScene()
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

        private void OnDestroy()
        {
            DialogBoxController.OnDialogEnds -= InstantiatePuzzle;
            DialogBoxController.OnDialogEnds -= GoToScene;
            NumberInput.OnPasswordCorrect -= OnPasswordCorrect;
            NumberInput.OnExitPuzzle -= OnDestroyPuzzle;
        }
    }
}
