using UnityEngine;

namespace Player.Gameplay.ClickableItems {
    public class Trapdoor : Door {
        protected override void Start() {
            var powerRestored = PersistentData.LoadGeneralData?.Invoke("PowerRestored");
            isDoorOpen = powerRestored != null && (bool)powerRestored.getData();

            Debug.Log(powerRestored);

            base.Start();
        }
    }

}