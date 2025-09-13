using Cinematics;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Player.UI
{
    public class UserInterface : MonoBehaviour
    {
        [SerializeField] private UIDocument UI;
        [SerializeField] private Inventory.Inventory inventory;

        private const float ROW_MAX_WIDTH = 960f;
        private const float ROW_MAX_HEIGHT = 125f;
        private const float STORE_BUTTON_MAX_WIDTH = 134f;
        private const float STORE_BUTTON_MAX_HEIGHT = 22f;
        private const float SLOT_GAP = 0.01f;
        private const float SLOT_SCALE = 0.72f;
        private int selectedIndex = -1;

        public static Action OnInventoryUpdated;
        public static Action OnWalletUpdated;

        private void Start()
        {
            DialogBoxController.OnDialogStars += HideUI;
            DialogBoxController.OnDialogEnds += ShowUI;
            DialogBoxController.OnQuestionEnds += ShowUI;
            OnInventoryUpdated += UpdateInventory;
            OnWalletUpdated += UpdateWallet;
            InitializeUI();
        }

        private void InitializeUI()
        {
            var root = UI.rootVisualElement;
            var row = root.Q("H1");
            var store = root.Q("Store");

            row.RegisterCallback<GeometryChangedEvent>(evt =>
            {
                SetContainerDimensions(root, row, store);
                AdjustSlotSizes(row);
            });

            store.Q("StoreButton").RegisterCallback<ClickEvent>(evt =>
            {
                // Open store interface
                Debug.Log("Store button clicked");
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

        private void UpdateUI()
        {
            UpdateWallet();
            UpdateInventory();
        }

        private void UpdateWallet()
        {
            var root = UI.rootVisualElement;
            var walletLabel = root.Q("Store").Q<Label>("Wallet");
            walletLabel.text = $"${10000}";
        }

        private void UpdateInventory()
        {
            var root = UI.rootVisualElement;
            var description = root.Q<Label>("Description");
            var items = inventory.GetItems();
            int selectedIndex = inventory.GetSelectedIndex();

            for (int index = 0; index < 9; index++)
            {
                var slotContainer = root.Q($"H1S{index + 1}");
                if (slotContainer == null)
                    continue;

                var icon = slotContainer.Q("Icon");
                var name = slotContainer.Q<Label>("Name");
                var amount = slotContainer.Q<Label>("Amount");


                if (index < items.Count)
                {
                    var data = items[index];
                    icon.style.backgroundImage = index == selectedIndex ? data.obj.PortraitHover : data.obj.Portrait;
                    name.text = data.obj.objectName;
                    amount.text = data.amount > 1 ? data.amount.ToString() : "";
                    if (index == selectedIndex)
                        description.text = data.obj.highlightText;
                } else
                {
                    icon.style.backgroundImage = null;
                    name.text = "";
                    amount.text = "";
                }

                var slot = slotContainer.Q("Slot");
                if (index == selectedIndex)
                    slot.AddToClassList("selected");
                else
                    slot.RemoveFromClassList("selected");
            }


            if (selectedIndex >= 0)
            {
                description.text = items[selectedIndex].obj.highlightText;
            } else
            {
                description.text = "";
            }

        }

        private void SetContainerDimensions(VisualElement root, VisualElement row, VisualElement store)
        {
            // Interface row sizing
            float rowWidth = Mathf.Min(row.resolvedStyle.width, ROW_MAX_WIDTH);
            float rowHeight = rowWidth * (ROW_MAX_HEIGHT / ROW_MAX_WIDTH);
            row.style.height = rowHeight;
            row.style.width = rowWidth;

            // Store info and button sizing
            store.style.width = rowWidth;
            store.style.height = Mathf.Min(STORE_BUTTON_MAX_HEIGHT, rowHeight * (STORE_BUTTON_MAX_HEIGHT / rowHeight));
            store.style.paddingRight = rowWidth * 0.1f;

            var newMax = 125f;
            var storeButton = root.Q("StoreButton");
            storeButton.style.width = Mathf.Min(newMax, rowWidth * (newMax / ROW_MAX_WIDTH));
            storeButton.style.height = storeButton.resolvedStyle.width / (STORE_BUTTON_MAX_WIDTH / STORE_BUTTON_MAX_HEIGHT);
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

                var label = slotContainer.Q<Label>("Name");
                label.style.fontSize = rowHeight * 0.15f;

                float slotSize = rowHeight * SLOT_SCALE;
                slot.style.width = slotSize;
                slot.style.height = slotSize;
                label.style.width = slotSize;

                slotContainer.style.marginLeft = (x > 1) ? row.resolvedStyle.width * SLOT_GAP : 0;
            }
        }

        void HideUI() { DisplayUI(false); }
        void ShowUI() { DisplayUI(true); }
        void ShowUI(byte a) { DisplayUI(true); }

        void DisplayUI(bool t)
        {
            UI.rootVisualElement.Q("Background").style.visibility = t ? Visibility.Visible : Visibility.Hidden;
        }

        private void OnDestroy()
        {
            OnInventoryUpdated -= UpdateUI;
            OnWalletUpdated -= UpdateWallet;
            DialogBoxController.OnDialogStars -= HideUI;
            DialogBoxController.OnDialogEnds -= ShowUI;
            DialogBoxController.OnQuestionEnds -= ShowUI;
        }
    }
}
