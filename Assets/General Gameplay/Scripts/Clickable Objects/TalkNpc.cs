using Player;
using UnityEngine;

namespace Player.Gameplay {
    public class TalkNpc : MouseReaction {
        [SerializeField] protected Conversation conversation;

        public override void OnInteractStart() {
            conversation.StartDialog();
        }
    }

}