using Cinematics;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Player.Inventory
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] UIDocument UI;
        [SerializeField] List<InventoryData> objects;

        public static Action<GameObject> PickUpItem;
        public static Action<Object> ItemSelected;

        private int selectedIndex = -1;
        private const int COLUMNS = 9;
        private const float ROW_ORIGINAL_WIDTH = 960f;
        private const float ROW_ORIGINAL_HEIGHT = 125f;
        private const float SLOT_GAP = 0.01f;
        private const float PADDING_TOP = 0.005f;

        private void Start()
        {
            var root = UI.rootVisualElement;
            var row = root.Q("H1");

            row.RegisterCallback<GeometryChangedEvent>(evt =>
            {
                float width = row.resolvedStyle.width;
                float aspect = ROW_ORIGINAL_HEIGHT / ROW_ORIGINAL_WIDTH;
                row.style.height = width * aspect;
                row.style.paddingTop = width * PADDING_TOP;
                AdjustSlotSizes(row);
            });

            for (int x = 1; x <= COLUMNS; x++)
            {
                int index = x - 1;
                VisualElement e = root.Q($"H1S{x}");
                if (e == null)
                    continue;

                e.RegisterCallback<ClickEvent>(evt =>
                {
                    if (index < objects.Count)
                    {
                        selectedIndex = (selectedIndex == index) ? -1 : index;
                        ItemSelected?.Invoke((selectedIndex >= 0) ? objects[selectedIndex].obj : null);
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

        void UpdateUI()
        {
            var root = UI.rootVisualElement;

            for (int index = 0; index < COLUMNS; index++)
            {
                VisualElement e = root.Q($"H1S{index + 1}");
                if (e == null)
                    continue;

                Texture2D portrait = null;
                string description = "";

                if (index < objects.Count)
                {
                    InventoryData obj = objects[index];
                    portrait = obj.obj.Portrait;
                    description = $"{obj.obj.highlightText}";

                }

                e.Q("Icon").style.backgroundImage = portrait;
                Label t = e.Q("Name") as Label;
                t.text = description;

                var slotElement = e.Q("Slot");
                if (index == selectedIndex)
                    slotElement.AddToClassList("selected");
                else
                    slotElement.RemoveFromClassList("selected");
            }
        }

        [System.Serializable]
        public struct InventoryData
        {
            public Object obj;
            public int amount;
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
                        int idx = objects.IndexOf(objectExists);
                        objects[idx] = objectExists;
                    }
                } else
                {
                    if (objects.Count < COLUMNS)
                    {
                        InventoryData newObj = new InventoryData { obj = obj.obj, amount = 1 };
                        objects.Add(newObj);
                        Destroy(gameObject);
                    }
                }

                UpdateUI();
            }
        }

        private void AdjustSlotSizes(VisualElement row)
        {
            float rowHeight = row.resolvedStyle.height;

            for (int x = 1; x <= COLUMNS; x++)
            {
                var slotContainer = row.Q($"H1S{x}");
                if (slotContainer == null)
                    continue;

                var slot = slotContainer.Q("Slot");
                if (slot == null)
                    continue;

                float slotSize = rowHeight * 0.72f;
                slot.style.width = slotSize;
                slot.style.height = slotSize;
                //slot.style.paddingBottom = rowHeight * 0.05f;

                if (x > 1)
                    slotContainer.style.marginLeft = row.resolvedStyle.width * SLOT_GAP;
                else
                    slotContainer.style.marginLeft = 0;
            }
        }

        void HideUI() => DisplayUI(false);
        void ShowUI() => DisplayUI(true);
        void ShowUI(byte a) => DisplayUI(true);

        void DisplayUI(bool t)
        {
            UI.rootVisualElement.Q("Background").style.display =
                t ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void OnDestroy()
        {
            DialogBoxController.OnDialogStars -= HideUI;
            DialogBoxController.OnDialogEnds -= ShowUI;
            DialogBoxController.OnQuestionEnds -= ShowUI;

            PickUpItem -= OnPickUpItem;
        }
    }
}
