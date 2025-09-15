using Cinematics;
using Player.Inventory;
using UnityEngine;

namespace Player.Gameplay {
    public class BeckyInteraction : TalkNpc {

        [SerializeField] Inventory.Object treats;
        [SerializeField] Dialog treatDialog;

        public override void OnInteractStart() {
            if(InteractionController.Instance.ItemSelected) {
                DialogBoxController.PlayDialog?.Invoke(treatDialog);
            } else {
                conversation.StartDialog();
            }
        }
    }
}