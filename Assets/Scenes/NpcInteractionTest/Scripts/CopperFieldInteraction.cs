using UnityEngine;

namespace Player.Gameplay {
    public class CopperFieldInteraction : Conversation
    {
        protected override void OnLoad() {
            Debug.Log("a");

            var loadDialog = PersistentData.GetData?.Invoke(transform.name, "currentDialog");
            var loadConversation = PersistentData.GetData?.Invoke(transform.name, "currentConversation");

            if(loadDialog != null && loadConversation != null) {
                Destroy(gameObject);
            }
        }
    }

}