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

        //Shop
        [Space(5)]
        [Header("Shop")]
        [SerializeField] UIDocument shopUI;
        [SerializeField] ShopData shopData;

        [SerializeField] Dialog notSelectedItem;
        [SerializeField] Dialog notSellableItem;
        [SerializeField] Dialog notEnoughtMoneyDialog;
        [SerializeField] Dialog alreadyBuyed;

        [Space(2)]
        [SerializeField] Question sellConfirm;

        //Inventory Params
        public static Action OnInventoryUpdated;
        public static Action OnWalletUpdated;

        private void Awake() {
            DialogBoxController.OnDialogStars += HideUI;
            DialogBoxController.OnDialogEnds += ShowUI;
            DialogBoxController.OnQuestionEnds += ShowUI;
            OnInventoryUpdated += UpdateInventory;
            OnWalletUpdated += UpdateWallet;
            InteractionController.OnSelectionChanged += UpdateInventory;
        }

        private void Start()
        {
            InitializeUI();
        }

        private void InitializeUI()
        {
            var root = UI.rootVisualElement;
            var row = root.Q("H1");
            var store = root.Q("Store");

            for (int x = 1; x <= 9; x++)
            {
                int index = x - 1;
                var slotContainer = root.Q($"H1S{x}");
                if (slotContainer == null)
                    continue;

                var icon = slotContainer.Q("Icon");

                slotContainer.RegisterCallback<PointerDownEvent>(evt => {
                    if (index < Inventory.Inventory.Instance.GetItems().Count)
                    {
                        if (evt.button == (int)MouseButton.LeftMouse)
                        {
                            InteractionController.Instance.OnLeftClick(index);
                        } else if (evt.button == (int)MouseButton.RightMouse)
                        {
                            InteractionController.Instance.OnRightClick(index);
                        }
                    }

                });

                slotContainer.RegisterCallback<MouseEnterEvent>(evt => {
                    if (index < Inventory.Inventory.Instance.GetItems().Count)
                    {
                        UpdateHoverIcon(index, icon, true);
                    }
                });

                slotContainer.RegisterCallback<MouseLeaveEvent>(evt => {
                    if (index < Inventory.Inventory.Instance.GetItems().Count)
                    {
                        UpdateHoverIcon(index, icon, false);
                    }
                });
            }

            UpdateUI();
            InitShopUI();
        }

        #region Shop

        void InitShopUI()
        {

            int SHOP_MAX_COLUMNS = 2;
            int SHOP_MAX_ROWS = 3;

            for (int x = 0; x < SHOP_MAX_COLUMNS; x++)
            {
                for (int y = 0; y < SHOP_MAX_ROWS; y++)
                {

                    int index = y + (x * SHOP_MAX_ROWS);
                    VisualElement e = shopUI.rootVisualElement.Q($"C{x}S{y}");

                    if (index < shopData.slots.Length)
                    {

                        Label c = e.Q("Cost") as Label;

                        c.text = $"${shopData.slots[index].price}";
                        c.style.display = DisplayStyle.Flex;

                        e.Q("Image").style.backgroundImage = shopData.slots[index].obj.Portrait;

                        PersistentData.BoolData isBuyed = PersistentData.LoadGeneralData?.Invoke(e.name) as PersistentData.BoolData;
                        if (isBuyed != null && isBuyed.value)
                        {
                            e.AddToClassList("ShopItem_Sold");
                            c.style.display = DisplayStyle.None;
                        }

                        e.RegisterCallback<ClickEvent>(evt => {
                            OnBuyShopItem(e, index);
                        });
                    } else
                    {
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

            DisplayShop(false);
        }

        void SellItem()
        {
            if (InteractionController.Instance.ItemSelected == null)
            { DialogBoxController.PlayDialog?.Invoke(notSelectedItem); return; }

            if (InteractionController.Instance.ItemSelected is SellableObject)
            {
                Debug.Log("Objeto vendible");

                SellableObject so = InteractionController.Instance.ItemSelected as SellableObject;
                sellConfirm.question = new Dialog($"I can give you ${so.sellingPrice} for that!", sellConfirm.question.characterSpeaking, 2);

                DialogBoxController.PlayQuestion?.Invoke(sellConfirm);
                DialogBoxController.OnQuestionEnds += OnSellItem;

                void OnSellItem(byte r)
                {
                    if (r == 0)
                    {
                        Inventory.Inventory.RemoveItem?.Invoke(so);
                        Currency.AddMoney?.Invoke(so.sellingPrice);
                        DialogBoxController.OnQuestionEnds -= OnSellItem;
                    }
                }
            } else
            {
                DialogBoxController.PlayDialog?.Invoke(notSellableItem);
            }
        }

        void OnBuyShopItem(VisualElement e, int index)
        {
            int? money = Currency.GetMoney?.Invoke();
            money = money == null ? 0 : money.Value;

            if (e.ClassListContains("ShopItem_Sold"))
            {
                DialogBoxController.PlayDialog?.Invoke(alreadyBuyed);
                return;
            }

            int price = shopData.slots[index].price;

            if (price > money)
            { DialogBoxController.PlayDialog?.Invoke(notEnoughtMoneyDialog); return; }

            var result = Inventory.Inventory.PickUpObject?.Invoke(shopData.slots[index].obj);
            if (result == null || !result.Value)
            { return; }

            e.AddToClassList("ShopItem_Sold");
            e.Q("Cost").style.display = DisplayStyle.None;

            Currency.AddMoney?.Invoke(-price);
            PersistentData.SaveGeneralData?.Invoke(new PersistentData.BoolData(e.name, true));
        }

        void DisplayShop()
        {
            VisualElement e = shopUI.rootVisualElement.Q("Background");
            bool t = e.style.visibility == Visibility.Hidden;

            e.style.visibility = t ? Visibility.Visible : Visibility.Hidden;
            Interactable.ChangeState?.Invoke(!t ? Interactable.CurrentState.Normal : Interactable.CurrentState.Paused);
        }
        void DisplayShop(bool t)
        {
            shopUI.rootVisualElement.Q("Background").style.visibility = t ? Visibility.Visible : Visibility.Hidden;
            Interactable.ChangeState?.Invoke(!t ? Interactable.CurrentState.Normal : Interactable.CurrentState.Paused);
        }

        #endregion

        #region Inventory
        private void UpdateUI()
        {
            UpdateWallet();
            UpdateInventory();
        }

        private Background GetIconBackground(int index, bool isHover)
        {
            var items = Inventory.Inventory.Instance.GetItems();
            if (index < 0 || index >= items.Count)
                return null;

            var data = items[index];
            var controller = InteractionController.Instance;

            if ((isHover || controller.SelectedIndexes.Contains(index)) && data.obj.PortraitHover != null)
                return data.obj.PortraitHover;

            return data.obj.Portrait;
        }

        private void UpdateHoverIcon(int index, VisualElement icon, bool isEnter)
        {
            icon.style.backgroundImage = GetIconBackground(index, isEnter);
        }

        private void UpdateWallet()
        {
            var root = UI.rootVisualElement;
            var walletLabel = root.Q("Store").Q<Label>("Wallet");

            int? money = Currency.GetMoney?.Invoke();
            if (money == null)
                money = 0;

            walletLabel.text = $"${money}";
        }

        private void UpdateInventory()
        {
            var root = UI.rootVisualElement;
            var description = root.Q<Label>("Description");
            var items = Inventory.Inventory.Instance.GetItems();
            var controller = InteractionController.Instance;

            for (int index = 0; index < 9; index++)
            {
                var slotContainer = root.Q($"H1S{index + 1}");
                if (slotContainer == null)
                    continue;

                var icon = slotContainer.Q("Icon");
                var name = slotContainer.Q<Label>("Name");
                var amount = slotContainer.Q<Label>("Amount");
                var slot = slotContainer.Q("Slot");

                if (index < items.Count)
                {
                    var data = items[index];

                    icon.style.backgroundImage = GetIconBackground(index, false);
                    name.text = data.obj.objectName;
                    amount.text = data.amount > 1 ? data.amount.ToString() : "";

                    if (controller.SelectedIndexes.Contains(index))
                        slot.AddToClassList("selected");
                    else
                        slot.RemoveFromClassList("selected");
                } else
                {
                    icon.style.backgroundImage = null;
                    name.text = "";
                    amount.text = "";
                    slot.RemoveFromClassList("selected");
                }
            }


            var selectedIndex = controller.GetLastIndexSelected();
            if (selectedIndex >= 0 && selectedIndex < items.Count)
            {
                description.text = items[selectedIndex].obj.highlightText;
            } else
            {
                description.text = "";
            }

        }

        #endregion

        void HideUI() => DisplayUI(false);
        void ShowUI() => DisplayUI(true);
        void ShowUI(byte a) => DisplayUI(true);

        void DisplayUI(bool visible)
        {
            UI.rootVisualElement.Q("Background").style.visibility =
                visible ? Visibility.Visible : Visibility.Hidden;
        }

        private void OnDestroy()
        {
            OnInventoryUpdated -= UpdateInventory;
            OnWalletUpdated -= UpdateWallet;
            InteractionController.OnSelectionChanged -= UpdateInventory;
            DialogBoxController.OnDialogStars -= HideUI;
            DialogBoxController.OnDialogEnds -= ShowUI;
            DialogBoxController.OnQuestionEnds -= ShowUI;
        }

    }
}
