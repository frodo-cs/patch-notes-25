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
            Debug.Log(evt.target);
            CheckPassword(evt.target as ToggleButtonGroup);  
        }

        private int? GetSelectedValue(ToggleButtonGroup group)
        {
            var options = group.value.GetActiveOptions(stackalloc int[group.value.length]);
            if (options.Length == 0)
                return null;
            return options[0];
        }

        private void CheckPassword(ToggleButtonGroup lastGroup)
        {
            var light = UI.rootVisualElement.Q<VisualElement>("Light");
            for (int i = 0; i < groups.Count; i++)
            {
                int? currentValue = GetSelectedValue(groups[i]);
                if(groups[i] == lastGroup) { //Last switch

                    if((valueToMatch[i] == 0 && currentValue != null) || (valueToMatch[i] == 1 && currentValue != 0)) {
                        SoundTable.PlaySound?.Invoke("interact_switch");
                    } else {
                        SoundTable.PlaySound?.Invoke("interact_switch_correct");
                    }
                }
            }

            for(int i = 0; i < groups.Count; i++) {
                int? currentValue = GetSelectedValue(groups[i]);

                if((valueToMatch[i] == 0 && currentValue != null) || (valueToMatch[i] == 1 && currentValue != 0)) {
                    light.RemoveFromClassList("light-on");
                    light.AddToClassList("light-off");
                    return;
                }
            }

            light.RemoveFromClassList("light-off");
            light.AddToClassList("light-on");
            OnPasswordCorrect?.Invoke();
        }



    }
}
