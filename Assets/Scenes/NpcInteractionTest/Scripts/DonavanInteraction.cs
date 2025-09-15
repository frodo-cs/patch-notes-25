using Cinematics;
using Player.Gameplay;
using UnityEngine;

namespace Player.Inventory {
    public class DonavanInteraction : TalkNpc {
        [SerializeField] Conversation beckyDialog;
        [SerializeField] Conversation postBeckyDialog;
        [SerializeField] Object key;

        public override void OnInteractStart() {
            if(DialogBoxController.IsDialogRunning.Invoke()) return;

            var p = PersistentData.LoadGeneralData?.Invoke("BeckyWithYou");
            var b = PersistentData.LoadGeneralData?.Invoke("ReceivedBecky");

            if(b != null && (bool)b.getData()) {
                postBeckyDialog.StartDialog();

                return;
            }

            if(p != null && (bool)p.getData()) {
                beckyDialog.StartDialog();

                bool? r = Inventory.PickUpObject?.Invoke(key);
                if(r != null && r.Value) {
                    PersistentData.SaveGeneralData?.Invoke(new PersistentData.BoolData("ReceivedBecky", true));
                }
            } else {
                conversation.StartDialog();
            }
        }
    }

}