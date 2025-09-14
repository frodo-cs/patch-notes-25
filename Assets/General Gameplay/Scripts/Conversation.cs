using Cinematics;
using UnityEngine;

public class Conversation : MonoBehaviour
{
    [SerializeField] conversationDialogs[] conversations;
    int currentConversation = 0;
    int currentDialog = 0;

    private void Start() {
        StartDialog();
    }

    public void StartDialog() {

        currentDialog = 0;

        if(currentConversation < conversations.Length) {
            DialogBoxController.PlayDialog?.Invoke(conversations[currentConversation].dialogs[currentDialog]);
            DialogBoxController.OnDialogEnds += NextDialog;
            currentDialog++;
        }
    }

    void NextDialog() {
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
    }
}
