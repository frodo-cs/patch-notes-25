using UnityEngine;
using static PersistentData;

namespace Player.Gameplay.ClickableItems
{

    public class ItemDroppables : ItemDependent
    {
        [SerializeField] protected Inventory.Object[] droppables;

        public void Start()
        {
            LoadDroppables();
        }

        protected virtual void AddItems() { }

        protected bool CanAddItems()
        {
            return neededObjects.Length + Inventory.Inventory.SpaceLeft - droppables.Length >= 0;

        }

        protected void SaveDroppables()
        {
            Save(itemId, new ObjectArrayData("RemainingItems", droppables));
        }

        protected void LoadDroppables()
        {
            var data = GetData(itemId, "RemainingItems") as ObjectArrayData;
            if (data != null)
            {
                droppables = data.value;
            }

        }
    }


}