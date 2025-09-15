using Cinematics;
using System;
using System.Collections.Generic;
using UnityEngine;
using static PersistentData;
using static UnityEditor.Progress;

namespace Player.Inventory
{
    public class Inventory : MonoBehaviour
    {

        public static Inventory Instance { get; private set; }

        [SerializeField] private List<InventoryData> objects;
        [SerializeField] Dialog dialog;

        public delegate bool ObjectOperation(Object obj);
        public static ObjectOperation PickUpObject;

        public static Action<GameObject> PickUpFromWorld;
        public static Action<Object> RemoveItem;
        public static Action<GameObject> OnItemAdded;
        public static Action<Object[]> AddItems;
        public static Action<Object> AddItem;
        public static int SpaceLeft => COLUMNS - Instance.objects.Count;

        private const int COLUMNS = 9;

        [Serializable]
        public struct InventoryData
        {
            public Object obj;
            public int amount;
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            PickUpFromWorld += OnPickUpItem;
            PickUpObject += OnAddOneItem;
            RemoveItem += OnRemoveItem;
            AddItems += OnAddItems;
            AddItem += OnAddItem;

            Debug.Log("Loading Inventory Objects!");
            LoadObjects();
        }


        public List<InventoryData> GetItems() => objects;

        private void SaveObjects()
        {
            SaveGeneralData?.Invoke(new ListInventoryData("Inventory", objects));
        }

        private void LoadObjects()
        {
            ListInventoryData data = LoadGeneralData?.Invoke("Inventory") as ListInventoryData;
            if (data != null)
            {
                objects = data.value;
            }
        }

        private void OnAddItems(Object[] obj)
        {
            foreach (var item in obj)
            {
                TryAddItem(item);
            }
            SaveObjects();
        }

        private void OnAddItem(Object obj)
        {
            if(obj == null) return;

            TryAddItem(obj);
            SaveObjects();
        }


        private bool OnAddOneItem(Object obj)
        {
            bool added = TryAddItem(obj);
            SaveObjects();
            return added;
        }

        private void OnPickUpItem(GameObject obj)
        {
            var inventoryItem = obj.GetComponent<InventoryItem>();
            if (inventoryItem == null || inventoryItem.obj == null)
                return;
            var addedItem = TryAddItem(inventoryItem.obj);
            if (addedItem)
            {
                ShowMessage($"You picked up {inventoryItem.obj.objectName}");
                OnItemAdded?.Invoke(obj);
                SaveObjects();
            }
        }

        private bool TryAddItem(Object obj)
        {
            if (obj == null)
                return false;

            int index = objects.FindIndex(o => o.obj != null && o.obj.name == obj.name);

            if (index >= 0)
            {
                var item = objects[index];
                if (item.amount < item.obj.maxAmount)
                {
                    item.amount++;
                    objects[index] = item;
                } else
                {
                    ShowMessage($"You cannot carry any more {obj.objectName}");
                    return false;
                }
            } else if (objects.Count < COLUMNS)
            {
                var newObj = new InventoryData { obj = obj, amount = 1 };
                objects.Add(newObj);
            } else
            {
                ShowMessage("Your inventory is full");
                return false;
            }

            UI.UserInterface.OnInventoryUpdated?.Invoke();
            return true;
        }


        private void OnRemoveItem(Object obj)
        {
            int index = objects.FindIndex(o => o.obj != null && o.obj.name == obj.name);
            if (index < 0)
                return;

            var item = objects[index];
            item.amount--;

            if (item.amount <= 0)
            {
                objects.RemoveAt(index);
            } else
            {
                objects[index] = item;
            }

            SaveObjects();
            UI.UserInterface.OnInventoryUpdated?.Invoke();
        }

        private void ShowMessage(string message)
        {
            if (DialogBoxController.IsDialogRunning?.Invoke() != true)
            {
                dialog.text = message;
                DialogBoxController.PlayDialog?.Invoke(dialog);
            }
        }

        private void OnDestroy()
        {
            PickUpFromWorld -= OnPickUpItem;
            RemoveItem -= OnRemoveItem;
            AddItems -= OnAddItems;
            PickUpObject -= OnAddOneItem;
            AddItem -= OnAddItem;
        }
    }
}
