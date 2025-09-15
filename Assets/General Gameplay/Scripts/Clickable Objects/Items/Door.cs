using Cinematics;
using Player.Puzzles;
using UnityEngine;
using static PersistentData;

namespace Player.Gameplay.ClickableItems
{
    public class Door : DescriptionObject
    {
        [SerializeField] protected int sceneToMove;
        protected bool isDoorOpen = false;

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
                OpenDialog();
            }
        }

        protected virtual void GoToScene()
        {
            var result = ChangeScene.LoadScene?.Invoke(sceneToMove);
            if (result == null || !result.Value)
                Debug.LogWarning("Failed to change scene");
        }

        protected void LoadDoorOpened()
        {
            var data = GetData(itemId, "DoorOpened") as BoolData;
            if (data != null)
            {
                isDoorOpen = data.value;
            }
        }
    }
}
