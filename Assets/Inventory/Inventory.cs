using System;
using System.Collections.Generic;
using UnityEngine;

namespace Player.Inventory
{
    public class Inventory : MonoBehaviour
    {

        public static Inventory Instance { get; private set; }

        [SerializeField] private List<InventoryData> objects;
        public static Action<GameObject> PickUpFromWorld;
        public static Action<Object> RemoveItem;
        public static Action<GameObject> OnItemAdded;
        public static Action<Object[]> AddItems;
        public static Action<Object> PickUpObject;
        public static int SpaceLeft => COLUMNS - Instance.objects.Count;

        private const int COLUMNS = 9;

        [System.Serializable]
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
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            PickUpFromWorld += OnPickUpItem;
            PickUpObject += OnAddOneItem;
            RemoveItem += OnRemoveItem;
            AddItems += OnAddItems;
        }

        public List<InventoryData> GetItems() => objects;

        private void OnAddItems(Object[] obj)
        {
            foreach (var item in obj)
            {
                TryAddItem(item);
            }
        }

        private void OnAddOneItem(Object obj)
        {
            TryAddItem(obj);
        }

        private void OnPickUpItem(GameObject obj)
        {
            var inventoryItem = obj.GetComponent<InventoryItem>();
            if (inventoryItem == null || inventoryItem.obj == null)
                return;
            var addedItem = TryAddItem(inventoryItem.obj);
            if (addedItem)
                OnItemAdded?.Invoke(obj);
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
                    Debug.Log("Item stack is full");
                    return false;
                }
            } else if (objects.Count < COLUMNS)
            {
                var newObj = new InventoryData { obj = obj, amount = 1 };
                objects.Add(newObj);
            } else
            {
                Debug.Log("Inventory is full");
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

            UI.UserInterface.OnInventoryUpdated?.Invoke();
        }

        private void OnDestroy()
        {
            PickUpFromWorld -= OnPickUpItem;
            RemoveItem -= OnRemoveItem;
            AddItems -= OnAddItems;
            PickUpObject -= OnAddOneItem;
        }
    }
}
