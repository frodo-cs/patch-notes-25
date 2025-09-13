using Cinematics;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Player.Inventory
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] private UIDocument UI;
        [SerializeField] private List<InventoryData> objects;

        public static Action<GameObject> PickUpItem;
        public static Action<Object> PickUpObject;
        public static Action<Object> ItemSelected;

        private int selectedIndex = -1;
        private const int COLUMNS = 9;

        [System.Serializable]
        public struct InventoryData
        {
            public Object obj;
            public int amount;
        }

        public InventoryData? SelectedItem =>
            selectedIndex >= 0 && selectedIndex < objects.Count ? objects[selectedIndex] : null;

        private void Start()
        {
            PickUpItem += OnPickUpItem;
            PickUpObject = OnPickUpItem;
        }

        private void OnPickUpItem(GameObject gameObject)
        {
            if (!gameObject.TryGetComponent(out InventoryItem obj))
                return;

            var objectExists = objects.FirstOrDefault(o => o.obj.name == obj.obj.name);

            if (objectExists.obj != null)
            {
                if (objectExists.amount < objectExists.obj.maxAmount)
                {
                    objectExists.amount++;
                    int idx = objects.IndexOf(objectExists);
                    objects[idx] = objectExists;
                }
            } else if (objects.Count < COLUMNS)
            {
                InventoryData newObj = new InventoryData { obj = obj.obj, amount = 1 };
                objects.Add(newObj);
                Destroy(gameObject);
            }

            Player.UI.UserInterface.OnInventoryUpdated?.Invoke();
        }

        private void OnPickUpItem(Object _obj) {
            if(_obj == null)return;

            var objectExists = objects.FirstOrDefault(o => o.obj.name == _obj.name);

            if(objectExists.obj != null) {
                if(objectExists.amount < objectExists.obj.maxAmount) {
                    objectExists.amount++;
                    int idx = objects.IndexOf(objectExists);
                    objects[idx] = objectExists;
                }
            } else if(objects.Count < COLUMNS) {
                InventoryData newObj = new InventoryData { obj = _obj, amount = 1 };
                objects.Add(newObj);
            }

            Player.UI.UserInterface.OnInventoryUpdated?.Invoke();
        }

        private void OnDestroy()
        {
            PickUpItem -= OnPickUpItem;
        }

        public void SetSelectedIndex(int index)
        {
            selectedIndex = (selectedIndex == index) ? -1 : index;
            Player.UI.UserInterface.OnInventoryUpdated?.Invoke();
            ItemSelected?.Invoke(selectedIndex >= 0 ? objects[selectedIndex].obj : null);
        }

        public int GetSelectedIndex() => selectedIndex;

        public List<InventoryData> GetItems() => objects;
    }
}
