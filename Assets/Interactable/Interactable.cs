using Cinematics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class Interactable : MonoBehaviour
    {
        [SerializeField] LayerMask interactableMask;
        [SerializeField] Vector2 cursorPos;
        [SerializeField] Texture2D cursor;

        MouseReaction interactableObj;

        CurrentState currentState;
        public enum CurrentState
        {
            Normal,
            Paused
        }

        public static Action<CurrentState> ChangeState;

        private void Awake()
        {
            Inputs.onClickPerformed += OnClickStarts;
            Inputs.onClickCancel += OnClickEnds;
            ChangeState = OnChangeState;
        }

        // Update is called once per frame
        void Update()
        {
            Vector2? _cursorPos = Inputs.GetCursorPosition?.Invoke();
            if (_cursorPos == null)
            { Debug.LogWarning("Cursor position not found! Check for Input script"); } else
            { cursorPos = _cursorPos.Value; }

            bool? isDialogRunning = DialogBoxController.IsDialogRunning?.Invoke();

            if (currentState == CurrentState.Paused)
                return;

            RaycastHit2D hit = Physics2D.CircleCast(cursorPos, 0.2f, Vector2.zero, 0.1f, interactableMask);

            if (hit.transform != null && hit.transform.TryGetComponent(out MouseReaction reaction))
            {
                interactableObj = reaction;
                if (cursor != null)
                {
                    Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);
                }
            } else if (isDialogRunning != null && !isDialogRunning.Value)
            {
                interactableObj = null;
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            }
        }

        void OnClickStarts(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;

            if (interactableObj != null && currentState == CurrentState.Normal)
                interactableObj.OnInteractStart();
            DialogBoxController.InteractOnDialog?.Invoke();
        }

        void OnClickEnds(InputAction.CallbackContext context)
        {
            if (!context.canceled)
                return;

            if (interactableObj != null && currentState == CurrentState.Normal)
                interactableObj.OnInteractEnd();
        }

        void OnChangeState(CurrentState newState)
        {
            if (newState == CurrentState.Paused)
            {
                if (interactableObj != null)
                    interactableObj.OnInteractEnd();
                interactableObj = null;
            }

            currentState = newState;
        }

        private void OnDestroy()
        {
            Inputs.onClickPerformed -= OnClickStarts;
            Inputs.onClickCancel -= OnClickEnds;
        }
    }

}
