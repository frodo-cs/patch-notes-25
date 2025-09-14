using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Player.Puzzles
{
    public class NumberInput : MonoBehaviour
    {
        private enum PasswordType { Safe, Door }

        [SerializeField] private UIDocument UI;
        [SerializeField] private string valueToMatch;
        [SerializeField] private PasswordType passwordType;

        public static Action OnPasswordCorrect;
        private readonly Dictionary<Button, Action> digitButtonHandlers = new();

        private VisualElement container;
        private TextField inputField;
        private Button submitButton;
        private Button resetButton;

        private void OnEnable()
        {
            if (UI == null || UI.rootVisualElement == null)
                return;

            UI.rootVisualElement.RegisterCallback<GeometryChangedEvent>(evt =>
            {
                SetupUI();
            });
        }

        private void SetupUI()
        {
            container = UI.rootVisualElement.Q<VisualElement>("Container");
            inputField = container.Q<TextField>("Password");
            submitButton = container.Q<Button>("Submit");
            resetButton = container.Q<Button>("Reset");
            inputField.isReadOnly = true;
            SetPlaceHolderText();
            var keypad = container.Q<VisualElement>("Keypad");
            var rows = keypad?.Query<VisualElement>("Row").ToList() ?? new List<VisualElement>();

            foreach (var row in rows)
            {
                foreach (var button in row.Children().OfType<Button>())
                {
                    string label = button.text;

                    if (label != "Clear" && label != "Enter")
                    {
                        Action handler = () => AppendDigit(label);
                        button.clicked += handler;
                        digitButtonHandlers[button] = handler;
                    }
                }
            }

            submitButton.clicked += SubmitPassword;
            resetButton.clicked += ClearPassword;

            inputField.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                submitButton.SetEnabled(evt.newValue.Length > 0);
                evt.StopPropagation();
            });
            submitButton.SetEnabled(inputField.value.Length > 0);
        }

        private void AppendDigit(string digit)
        {
            if (inputField.value.Length >= valueToMatch.Length)
                return;
            inputField.value += digit;
            ResetClass();
        }

        private void ClearPassword()
        {
            inputField.value = string.Empty;
            SetPlaceHolderText();
        }

        private void SubmitPassword()
        {
            if (inputField.value == valueToMatch)
            {
                Debug.Log("Password correct");
                OnPasswordCorrect?.Invoke();
                AddClass("Correct");
            } else
            {
                Debug.Log("Incorrect password");
                AddClass("Incorrect");
            }
        }

        private void SetPlaceHolderText()
        {
            inputField.textEdition.placeholder = new string('*', valueToMatch.Length);
            ResetClass();
        }

        private void AddClass(string className)
        {
            SetPlaceHolderText();
            inputField.AddToClassList(className);
            Invoke(nameof(Clear), 1f);
        }

        private void Clear()
        {
            ResetClass();
            inputField.value = string.Empty;
        }

        private void ResetClass()
        {
            inputField.RemoveFromClassList("Correct");
            inputField.RemoveFromClassList("Incorrect");
            inputField.textEdition.placeholder = new string('*', valueToMatch.Length);
        }

        private void OnDisable()
        {
            if (submitButton != null)
                submitButton.clicked -= SubmitPassword;
            if (resetButton != null)
                resetButton.clicked -= ClearPassword;

            foreach (var buttonHandler in digitButtonHandlers)
            {
                buttonHandler.Key.clicked -= buttonHandler.Value;
            }
            digitButtonHandlers.Clear();
        }
    }
}
