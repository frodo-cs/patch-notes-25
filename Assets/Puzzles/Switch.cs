using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Player.Puzzles
{
    public class SwitchBoardPuzzle : MonoBehaviour
    {
        [SerializeField] private UIDocument UI;
        [SerializeField] private int[] valueToMatch = new int[8];

        private List<ToggleButtonGroup> groups = new();
        public static Action OnPasswordCorrect;
        public static Action OnExitClicked;
        private Button exitButton;


        private void OnEnable()
        {
            var root = UI.rootVisualElement;
            groups = root.Query<ToggleButtonGroup>().ToList();
            exitButton = root.Q<Button>("Exit");

            exitButton.clicked += ExitPuzzleInvoke;

            foreach (var group in groups)
            {
                group.RegisterValueChangedCallback(OnGroupValueChanged);
            }

        }

        private void ExitPuzzleInvoke()
        {
            OnExitClicked?.Invoke();
        }

        private void OnDisable()
        {
            foreach (var group in groups)
            {
                group.UnregisterValueChangedCallback(OnGroupValueChanged);
            }
        }



        private void OnGroupValueChanged(ChangeEvent<ToggleButtonGroupState> evt)
        {
            CheckPassword();
        }

        private int GetSelectedValue(ToggleButtonGroup group)
        {
            var options = group.value.GetActiveOptions(stackalloc int[group.value.length]);
            return options.Length > 0 ? options[0] : -1;
        }

        private void CheckPassword()
        {
            for (int i = 0; i < groups.Count; i++)
            {
                int currentValue = GetSelectedValue(groups[i]);
                if (currentValue != valueToMatch[i])
                {
                    UI.rootVisualElement.Q<VisualElement>("Light").style.backgroundColor = Color.red;
                    return;

                }
            }
            UI.rootVisualElement.Q<VisualElement>("Light").style.backgroundColor = Color.green;
            OnPasswordCorrect?.Invoke();
        }

    }
}
