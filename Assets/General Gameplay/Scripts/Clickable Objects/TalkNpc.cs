using Player;
using UnityEngine;

namespace Player.Gameplay {
    public class TalkNpc : MouseReaction {
        [SerializeField] Conversation conversation;

        public override void OnInteractStart() {
            conversation.StartDialog();
        }
    }

}