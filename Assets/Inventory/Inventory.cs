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
        private int? selectedIndex = null;

        public InventoryData? SelectedItem
        {
            get
            {
                if (selectedIndex.HasValue && selectedIndex.Value >= 0 && selectedIndex.Value < objects.Count)
                {
                    return objects[selectedIndex.Value];
                }
                return null;
            }
        }

        private const int COLUMNS = 9;


        private void Start() {
            for (int x = 1; x <= COLUMNS; x++)
            {
                int index = (x - 1);
                VisualElement e = UI.rootVisualElement.Q($"H1S{x}");

                if (e == null)
                    continue;

                e.RegisterCallback<ClickEvent>(evt =>
                {
                    if (index < objects.Count)
                    {
                        selectedIndex = index;
                        Debug.Log($"Selected item: {SelectedItem.Value.obj.name}");
                        UpdateUI();
                    }
                });
            }

            UpdateUI();

            DialogBoxController.OnDialogStars += HideUI;
            DialogBoxController.OnDialogEnds += ShowUI;
            DialogBoxController.OnQuestionEnds += ShowUI;

            PickUpItem += OnPickUpItem;
        }

        [System.Serializable]
        public struct InventoryData {
            public Object obj;
            public int amount;
        }

        void UpdateUI() {
            for (int index = 0; index <= COLUMNS; index++)
            {
                VisualElement e = UI.rootVisualElement.Q($"H1S{index + 1}");

                if (e == null)
                    continue;
                Texture2D portrait = null;
                string description = "";

                if (index < objects.Count)
                {
                    InventoryData obj = objects[index];
                    description = $"{obj.amount}/{obj.obj.maxAmount}";
                    portrait = obj.obj.Portrait;
                }

                e.Q("Icon").style.backgroundImage = portrait;

                if (index == selectedIndex)
                    e.Q("Slot").AddToClassList("selected");
                else
                    e.Q("Slot").RemoveFromClassList("selected");
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

        private void OnDestroy() {
            DialogBoxController.OnDialogStars -= HideUI;

            DialogBoxController.OnDialogEnds -= ShowUI;
            DialogBoxController.OnQuestionEnds -= ShowUI;
        }
    }

} 