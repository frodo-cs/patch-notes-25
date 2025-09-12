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
            cam = Camera.main;
        }

        private void Update() {

            Vector2 minPos = cam.ViewportToWorldPoint(Vector2.zero);
            Vector2 maxPos = cam.ViewportToWorldPoint(Vector2.one);

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
        public void onClickPerformedCallback(InputAction.CallbackContext context) { 

            if(context.performed) onClickPerformed?.Invoke(context); 
            else if(context.canceled) onClickCancel?.Invoke(context);
        }
    }

}