using UnityEngine;
using static PersistentData;

namespace Player.Gameplay.ClickableItems
{
    public class ItemDroppables : ItemDependent
    {
        [SerializeField] protected Inventory.Object[] droppables;


        protected override void Start()
        {
            base.Start();
            if (touched && droppables.Length > 0)
            {
                gameObject.GetComponent<Collider2D>().enabled = true;
            } else if (touched || droppables.Length == 0)
            {
                gameObject.GetComponent<Collider2D>().enabled = false;
            }
        }

        protected virtual void AddItems() { }

        protected virtual bool CanAddItems()
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