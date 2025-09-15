using Cinematics;
using UnityEngine;
using UnityEngine.Events;

public class Conversation : MonoBehaviour
{
    [SerializeField] conversationDialogs[] conversations;
    int currentConversation = 0;
    int currentDialog = 0;

    private void Start() {
        for(int i = 0; i < conversations.Length; i++) {
            for(int j = 0; j < conversations[i].dialogs.Length; j++) {
                conversations[i].dialogs[j].hideOnDialogEnds = j >= conversations[i].dialogs.Length - 1;
            }
        }

        //StartDialog();
    }

    public void StartDialog() {
        var a = DialogBoxController.IsDialogRunning?.Invoke();
        if(a == null || a.Value) return;

        currentDialog = 0;

        if(currentConversation < conversations.Length) {
            DialogBoxController.PlayDialog?.Invoke(conversations[currentConversation].dialogs[currentDialog]);
            DialogBoxController.OnDialogEnds += NextDialog;

            currentDialog++;
        }
    }

    void NextDialog() {

        for(int i = 0; i < conversations[currentConversation].events.Length; i++) {
            if(conversations[currentConversation].events[i].index + 1 == currentDialog) {

                conversations[currentConversation].events[i].trigger?.Invoke();
                break;
            }
        }

        if(currentDialog < conversations[currentConversation].dialogs.Length) {
            DialogBoxController.PlayDialog?.Invoke(conversations[currentConversation].dialogs[currentDialog]);
            currentDialog++;
        } else {
            currentConversation++;
            DialogBoxController.OnDialogEnds -= NextDialog;
        }
    }

    [System.Serializable]
    public struct conversationDialogs {
        public Dialog[] dialogs;
        public conversationEvents[] events;

        [System.Serializable]
        public struct conversationEvents {
            public int index;
            public UnityEvent trigger;
        }
    }
}
