using Cinematics;
using UnityEngine;
using UnityEngine.UIElements;


namespace Player.Inventory {
    public class Inventory : MonoBehaviour {
        [SerializeField] UIDocument UI;
        [SerializeField] InventoryData[] objects;

        private void Start() {
            UpdateUI();

            DialogBoxController.OnDialogStars += HideUI;

            DialogBoxController.OnDialogEnds += ShowUI;
            DialogBoxController.OnQuestionEnds += ShowUI;
        }

        [System.Serializable]
        struct InventoryData {
            public Object obj;
            public int amount;
        }

        void UpdateUI() {
            for(int y = 1; y < 2; y++) {
                for(int x = 1; x <= 6; x++) {
                    VisualElement e = UI.rootVisualElement.Q($"H{y}S{x}");
                    int index = (x - 1) + ((y - 1) * 6);

                    Texture2D portrait = null;
                    string description = "";

                    if(index < objects.Length) {
                        InventoryData obj = objects[index];

                        description = $"{obj.amount}/{obj.obj.maxAmount}";
                        portrait = obj.obj.Portrait;
                    }

                    e.Q("Icon").style.backgroundImage = portrait;
                    Label t = e.Q("Amount") as Label;

                    t.text = description;

                }
            }
        }

        void HideUI() { DisplayUI(false); }
        void ShowUI() { DisplayUI(true); }
        void ShowUI(byte a) { DisplayUI(true); }

        void DisplayUI(bool t) { UI.rootVisualElement.Q("Background").style.display = t ? DisplayStyle.Flex : DisplayStyle.None; }
    }

} 