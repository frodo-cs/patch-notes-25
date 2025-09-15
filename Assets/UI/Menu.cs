using UnityEngine;
using UnityEngine.UIElements;

namespace Player.UI
{
    public class Menu : MonoBehaviour
    {
        [SerializeField] private UIDocument UI;

        private VisualElement root;
        private VisualElement container;
        private Button startButton;

        private void OnEnable()
        {
            root = UI.rootVisualElement;
            container = root.Q<VisualElement>("Default");
            startButton = root.Q<Button>("Start");

            startButton.RegisterCallback<MouseEnterEvent>(OnStartHover);
            startButton.RegisterCallback<MouseLeaveEvent>(OnStartLeave);
            startButton.RegisterCallback<MouseUpEvent>(OnMouseClick);
        }

        private void OnMouseClick(MouseUpEvent e)
        {
            Debug.Log("START");
            var result = ChangeScene.LoadScene?.Invoke(0);
        }

        private void OnDisable()
        {
            startButton.UnregisterCallback<MouseEnterEvent>(OnStartHover);
            startButton.UnregisterCallback<MouseLeaveEvent>(OnStartLeave);
        }

        private void OnStartHover(MouseEnterEvent evt)
        {
            container.RemoveFromClassList("menu-default");
            container.AddToClassList("menu-hover");
        }

        private void OnStartLeave(MouseLeaveEvent evt)
        {
            container.RemoveFromClassList("menu-hover");
            container.AddToClassList("menu-default");
        }
    }
}
