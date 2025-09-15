using Cinematics;
using Player.Puzzles;
using UnityEngine;
using static PersistentData;

namespace Player.Gameplay.ClickableItems
{
    public class AccessPoint : DescriptionObject
    {
        [SerializeField] int sceneToMove;

        public override void OnInteractStart()
        {
            if (DialogBoxController.IsDialogRunning?.Invoke() == true)
                return;

            GoToScene();
        }

        private void GoToScene() {
            SoundTable.PlaySound?.Invoke($"movement_door_{Random.Range(0, 3)}");

            var result = ChangeScene.LoadScene?.Invoke(sceneToMove);
            if(result == null || !result.Value) {
                Debug.LogWarning("Failed to change scene");
                return;
            }
        }
    }
}
