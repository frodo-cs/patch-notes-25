using UnityEngine;
using UnityEngine.UIElements;
using System;
using Player.Gameplay;

namespace Player.UI
{
    public class Movement : MonoBehaviour
    {
        [SerializeField] private UIDocument UI;

        [Serializable]
        public struct FieldData
        {
            public enum Alignment
            {
                Left,
                Right,
                Center
            }

            public Alignment alignment;
            public int index;
            public string text;
        }

        [SerializeField] private FieldData[] fields = new FieldData[3];

        void Start()
        {
            InitializeUIElements();
        }

        private void InitializeUIElements()
        {
            var root = UI.rootVisualElement;

            var left = root.Q("Left").Q<Button>();
            var right = root.Q("Right").Q<Button>();
            var center = root.Q("Down").Q<Button>();

            foreach (var field in fields)
            {
                Button button = null;
                switch (field.alignment)
                {
                    case FieldData.Alignment.Left:
                        button = left;
                        break;
                    case FieldData.Alignment.Right:
                        button = right;
                        break;
                    case FieldData.Alignment.Center:
                        button = center;
                        break;
                }

                if (button == null)
                    continue;

                button.text = field.text;
                int sceneToMove = field.index;
                button.RegisterCallback<ClickEvent>(evt =>
                {
                    MoveToScene(sceneToMove);
                });
            }
        }

        private void MoveToScene(int sceneToMove)
        {
            var result = ChangeScene.LoadScene?.Invoke(sceneToMove);

            if (result == null || !result.Value)
                OnFail();
        }

        private void OnFail()
        {
            Debug.LogWarning("Change Scene Singleton doesn't exist or there is a dialog on screen!");
        }

    }
}
