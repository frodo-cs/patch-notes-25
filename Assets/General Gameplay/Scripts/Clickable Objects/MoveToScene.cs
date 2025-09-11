using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Gameplay {
    public class MoveToScene : MouseReaction {
        [SerializeField] int sceneToMove = 0;

        public override void OnInteractStart() {
            var result = ChangeScene.LoadScene?.Invoke(sceneToMove);

            if(result == null || !result.Value) OnFail();
        }

        void OnFail() {
            Debug.LogWarning("Change Scene Singleton doesn't exist or there is dialog on screen!");

            //Feedback animation
        }
    }

}