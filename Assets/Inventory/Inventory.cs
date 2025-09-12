using Cinematics;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;


namespace Player.Inventory {
    public class Inventory : MonoBehaviour {
        [SerializeField] UIDocument UI;
        [SerializeField] List<InventoryData> objects;
        public static Action<GameObject> PickUpItem;

        private void Start() {
            UpdateUI();

            DialogBoxController.OnDialogStars += HideUI;
            DialogBoxController.OnDialogEnds += ShowUI;
            DialogBoxController.OnQuestionEnds += ShowUI;
            PickUpItem += OnPickUpItem;
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

                    if(index < objects.Count) {
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

        private void OnPickUpItem(GameObject gameObject)
        {
            if (gameObject.TryGetComponent(out InventoryItem obj))
            {
                InventoryData objectExists = objects.FirstOrDefault(o => o.obj.name == obj.obj.name);
                if (objectExists.obj != null)
                {
                    if (objectExists.amount < objectExists.obj.maxAmount)
                    {
                        objectExists.amount++;
                        int index = objects.IndexOf(objectExists);
                        objects[index] = objectExists;
                    }
                }
                else
                {
                    if (objects.Count < 6)
                    {   
                        InventoryData newObj = new InventoryData { obj = obj.obj, amount = 1 };
                        objects.Add(newObj);
                        Destroy(gameObject);
                    }
                }
                UpdateUI();
            }
        }

        void HideUI() { DisplayUI(false); }
        void ShowUI() { DisplayUI(true); }
        void ShowUI(byte a) { DisplayUI(true); }

        void DisplayUI(bool t) { UI.rootVisualElement.Q("Background").style.display = t ? DisplayStyle.Flex : DisplayStyle.None; }
    }

} 