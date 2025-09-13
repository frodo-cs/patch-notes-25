using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Player.Inventory
{
    public class InventoryUI
    {
        private UIDocument uiDocument;
        private Inventory inventory;

        private const float ROW_ORIGINAL_WIDTH = 960f;
        private const float ROW_ORIGINAL_HEIGHT = 125f;
        private const float SLOT_GAP = 0.01f;
        private const float SLOT_SCALE = 0.72f;
        private int selectedIndex = -1;

        public InventoryUI(UIDocument ui, Inventory inventory)
        {
            uiDocument = ui;
            this.inventory = inventory;

            InitializeSlots();
        }

        private void InitializeSlots()
        {
            var root = uiDocument.rootVisualElement;
            var row = root.Q("H1");

            row.RegisterCallback<GeometryChangedEvent>(evt =>
            {
                float width = row.resolvedStyle.width;
                float aspect = ROW_ORIGINAL_HEIGHT / ROW_ORIGINAL_WIDTH;
                row.style.height = width * aspect;
                AdjustSlotSizes(row);
            });

            for (int x = 1; x <= 9; x++)
            {
                int index = x - 1;
                var slotContainer = root.Q($"H1S{x}");
                if (slotContainer == null)
                    continue;

                var icon = slotContainer.Q("Icon");

                slotContainer.RegisterCallback<ClickEvent>(evt =>
                {
                    if (index < inventory.GetItems().Count)
                    {
                        var data = inventory.GetItems()[index];
                        if (data.obj.PortraitHover != null)
                            icon.style.backgroundImage = data.obj.PortraitHover;
                        selectedIndex = index;
                        inventory.SetSelectedIndex(index);
                    }
                    
                });

                slotContainer.RegisterCallback<MouseEnterEvent>(evt =>
                {
                    if (index < inventory.GetItems().Count)
                    {
                        var data = inventory.GetItems()[index];
                        if (data.obj.PortraitHover != null)
                            icon.style.backgroundImage = data.obj.PortraitHover;
                    }
                });

                slotContainer.RegisterCallback<MouseLeaveEvent>(evt =>
                {
                    if (index < inventory.GetItems().Count && index != selectedIndex)
                    {
                        var data = inventory.GetItems()[index];
                        icon.style.backgroundImage = data.obj.Portrait;
                    }
                });
            }

            UpdateUI();
        }

        public void UpdateUI()
        {
            var root = uiDocument.rootVisualElement;
            var items = inventory.GetItems();
            int selectedIndex = inventory.GetSelectedIndex();

            for (int index = 0; index < 9; index++)
            {
                var slotContainer = root.Q($"H1S{index + 1}");
                if (slotContainer == null)
                    continue;

                var icon = slotContainer.Q("Icon");
                var label = slotContainer.Q<Label>("Name");

                if (index < items.Count)
                {
                    var data = items[index];
                    icon.style.backgroundImage = index == selectedIndex ? data.obj.PortraitHover : data.obj.Portrait;
                    label.text = data.obj.highlightText;
                } else
                {
                    icon.style.backgroundImage = null;
                    label.text = "";
                }

                var slot = slotContainer.Q("Slot");
                if (index == selectedIndex)
                    slot.AddToClassList("selected");
                else
                    slot.RemoveFromClassList("selected");
            }
        }

        private void AdjustSlotSizes(VisualElement row)
        {
            float rowHeight = row.resolvedStyle.height;

            for (int x = 1; x <= 9; x++)
            {
                var slotContainer = row.Q($"H1S{x}");
                if (slotContainer == null)
                    continue;

                var slot = slotContainer.Q("Slot");
                if (slot == null)
                    continue;

                float slotSize = rowHeight * SLOT_SCALE;
                slot.style.width = slotSize;
                slot.style.height = slotSize;

                slotContainer.style.marginLeft = (x > 1) ? row.resolvedStyle.width * SLOT_GAP : 0;
            }
        }
    }
}
