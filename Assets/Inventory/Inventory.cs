using UnityEngine;
using UnityEngine.UIElements;


namespace Player.Inventory {
    public class Inventory : MonoBehaviour {
        [SerializeField] UIDocument UI;
        [SerializeField] InventoryData[] objects;

        private void Start() {
            UpdateUI();
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
    }

} 