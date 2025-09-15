using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Player.Gameplay
{
    public class ItemDependent : DescriptionObject
    {
        [Space(5)]
        [SerializeField] protected Inventory.Object[] neededObjects;

        [Space(5)]
        [SerializeField] UnityEvent onUnlock;
        private HashSet<Inventory.Object> neededSet;

        private void Awake()
        {
            neededSet = new HashSet<Inventory.Object>(neededObjects);
        }

        protected bool HasItemNeeded(Inventory.Object selected)
        {
            return neededSet.Contains(selected);
        }

        private List<Inventory.Inventory.InventoryData> GetItemsNeed()
        {
            var selectedIndexes = InteractionController.Instance.SelectedIndexes;
            var inventoryItems = Inventory.Inventory.Instance.GetItems();

            return selectedIndexes
                .Where(index => index >= 0 && index < inventoryItems.Count)
                .Select(index => inventoryItems[index])
                .ToList();
        }

        protected bool HasItemsNeeded()
        {
            var selectedItems = GetItemsNeed();
            var counter = new Dictionary<Object, int>();
            foreach (var needed in neededObjects)
            {
                if (counter.ContainsKey(needed))
                    counter[needed]++;
                else
                    counter[needed] = 1;
            }

            foreach (var kvp in counter)
            {
                var match = selectedItems.FirstOrDefault(item => item.obj == kvp.Key);
                if (match.obj == null || match.amount < kvp.Value)
                    return false;
            }

            return true;
        }

    }

}
