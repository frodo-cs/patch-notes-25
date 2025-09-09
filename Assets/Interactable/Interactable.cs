using Cinematics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player {
    public class Interactable : MonoBehaviour {
        [SerializeField] LayerMask interactableMask;
        [SerializeField] Vector2 cursorPos;

        MouseReaction interactableObj;

        // Start is called before the first frame update
        void Start() {
            Inputs.onClickPerformed += OnClickStarts;
            Inputs.onClickCancel += OnClickEnds;
        }

        // Update is called once per frame
        void Update() {
            Vector2? _cursorPos = Inputs.GetCursorPosition?.Invoke();
            if(_cursorPos == null) { Debug.LogWarning("Cursor position not found! Check for Input script"); }
            else { cursorPos = _cursorPos.Value; }

            bool? isDialogRunning = DialogBoxController.IsDialogRunning?.Invoke();
            Debug.Log(isDialogRunning);

            RaycastHit2D hit = Physics2D.CircleCast(cursorPos, 0.2f, Vector2.zero, 0.1f, interactableMask);

            if(hit.transform != null && hit.transform.TryGetComponent(out MouseReaction reaction)) { interactableObj = reaction; }
            else if(isDialogRunning != null && !isDialogRunning.Value) { interactableObj = null; }
        }

        void OnClickStarts(InputAction.CallbackContext context) {
            if(!context.performed) return;

            if(interactableObj != null) interactableObj.OnInteractStart();
            DialogBoxController.InteractOnDialog?.Invoke();
        }

        void OnClickEnds(InputAction.CallbackContext context) {
            if(!context.canceled) return;

            if(interactableObj != null) interactableObj.OnInteractEnd();
        }

        private void OnDestroy() {
            Inputs.onClickPerformed += OnClickStarts;
            Inputs.onClickCancel += OnClickEnds;
        }
    }

}
