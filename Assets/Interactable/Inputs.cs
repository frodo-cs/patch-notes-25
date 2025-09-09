using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player {
    public class Inputs : MonoBehaviour {
        Vector2 cursorPos;
        [SerializeField] PlayerInput input;

        Camera cam;

        public delegate Vector2 getCursorPosition();
        public static getCursorPosition GetCursorPosition;

        public static Action<InputAction.CallbackContext> onClickPerformed;
        public static Action<InputAction.CallbackContext> onClickCancel;

        private void Awake() {
            GetCursorPosition = OnGetCursorPosition;
            input.currentActionMap["Interact"].performed += onClickPerformedCallback;
            input.currentActionMap["Interact"].canceled += onClickCancelCallback;

            cam = Camera.main;
        }

        private void Update() {

            Vector2 minPos = cam.ViewportToWorldPoint(Vector2.zero);
            Vector2 maxPos = cam.ViewportToWorldPoint(Vector2.one);

            Debug.Log($"{minPos} - {maxPos}");

            switch(input.currentControlScheme) {
                case "Keyboard":
                    cursorPos = cam.ScreenToWorldPoint(Input.mousePosition);
                    break;

                case "Controller":
                    cursorPos += input.currentActionMap["Cursor"].ReadValue<Vector2>();
                    break;


                default:
                    break;
            }

            cursorPos = cursorPos.ClampVector(minPos, maxPos);
        }

        Vector2 OnGetCursorPosition() { return cursorPos; }
        void onClickPerformedCallback(InputAction.CallbackContext context) { onClickPerformed?.Invoke(context); }
        void onClickCancelCallback(InputAction.CallbackContext context) { onClickCancel?.Invoke(context); }
    }

}