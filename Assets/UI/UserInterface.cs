using Cinematics;
using Player.Inventory;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Player.UI
{
    public class UserInterface : MonoBehaviour
    {
        [SerializeField] private UIDocument UI;
        [SerializeField] private Inventory.Inventory inventory;

        //Shop
        [Space(5)]
        [Header("Shop")]
        [SerializeField] UIDocument shopUI;
        [SerializeField] ShopData shopData;

        [SerializeField] Dialog notSelectedItem;
        [SerializeField] Dialog notSellableItem;
        [SerializeField] Dialog notEnoughtMoneyDialog;

        [Space(2)]
        [SerializeField] Question sellConfirm;

        //Inventory Params
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
            InitShopUI();
        }

        #region Shop

        void InitShopUI() {

            int SHOP_MAX_COLUMNS = 2;
            int SHOP_MAX_ROWS = 3;

            for(int x = 0; x < SHOP_MAX_COLUMNS; x++) {
                for(int y = 0; y < SHOP_MAX_ROWS; y++) {

                    int index = y + (x * SHOP_MAX_ROWS);
                    VisualElement e = shopUI.rootVisualElement.Q($"C{x}S{y}");

                    if(index < shopData.slots.Length) {

                        Label c = e.Q("Cost") as Label;

                        c.text = $"${shopData.slots[index].price}";
                        c.style.display = DisplayStyle.Flex;

                        e.Q("Image").style.backgroundImage = shopData.slots[index].obj.Portrait;

                        e.RegisterCallback<ClickEvent>(evt =>
                        {
                            OnBuyShopItem(e, index);
                        });
                    } else {
                        e.style.display = DisplayStyle.None;
                    }
                }
            }

            shopUI.rootVisualElement.Q("Close").RegisterCallback<ClickEvent>(evt => {
                DisplayShop(false);
            });

            UI.rootVisualElement.Q("StoreButton").RegisterCallback<ClickEvent>(evt => {
                DisplayShop();
            });

            shopUI.rootVisualElement.Q("Sell").RegisterCallback<ClickEvent>(evt => {
                SellItem();
            });
        }

        void SellItem() {
            if(InteractionController.Instance.ItemSelected == null) { DialogBoxController.PlayDialog?.Invoke(notSelectedItem); return; }

            if(InteractionController.Instance.ItemSelected is SellableObject) {
                Debug.Log("Objeto vendible");

                SellableObject so = InteractionController.Instance.ItemSelected as SellableObject;
                sellConfirm.question = new Dialog($"I can give you ${so.sellingPrice} for that!", null, 2);

                DialogBoxController.PlayQuestion?.Invoke(sellConfirm);
                DialogBoxController.OnQuestionEnds += OnSellItem;

                void OnSellItem(byte r) {
                    if(r == 0) {
                        Currency.AddMoney?.Invoke(so.sellingPrice);
                        DialogBoxController.OnQuestionEnds -= OnSellItem;
                    }
                }
            } else {
                DialogBoxController.PlayDialog?.Invoke(notSellableItem);
            }
        }

        void OnBuyShopItem(VisualElement e, int index) {
            int? money = Currency.GetMoney?.Invoke();
            money = money == null ? 0 : money.Value;

            int price = shopData.slots[index].price;

            if(price > money) { DialogBoxController.PlayDialog?.Invoke(notEnoughtMoneyDialog); return; }

            e.AddToClassList("ShopItem_Sold");
            e.Q("Cost").style.display = DisplayStyle.None;

            Inventory.Inventory.PickUpObject?.Invoke(shopData.slots[index].obj);
            Currency.AddMoney?.Invoke(-price);
        }

        void DisplayShop() {
            VisualElement e = shopUI.rootVisualElement.Q("Background");
            e.style.visibility = e.style.visibility == Visibility.Hidden ? Visibility.Visible : Visibility.Hidden; 
        }
        void DisplayShop(bool t) { shopUI.rootVisualElement.Q("Background").style.visibility = t ? Visibility.Visible : Visibility.Hidden; }

        #endregion

        private void UpdateUI()
        {
            UpdateWallet();
            UpdateInventory();
        }

        private void UpdateWallet()
        {
            var root = UI.rootVisualElement;
            var walletLabel = root.Q("Store").Q<Label>("Wallet");

            int? money = Currency.GetMoney?.Invoke();
            if(money == null) money = 0;

            walletLabel.text = $"${money}";
        }

        private void UpdateInventory()
        {
            var root = UI.rootVisualElement;
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
