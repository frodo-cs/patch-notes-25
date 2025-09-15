using Cinematics;
using Player.Gameplay;
using UnityEngine;

namespace Player.Inventory {
    public class DonavanInteraction : TalkNpc {
        [SerializeField] Conversation beckyDialog;

        public override void OnInteractStart() {
            var p = PersistentData.LoadGeneralData?.Invoke("BeckyWithYou");

            if(p != null && (bool)p.getData()) {
                beckyDialog.StartDialog();
            } else {
                conversation.StartDialog();
            }
        }
    }

}