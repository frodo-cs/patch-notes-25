using UnityEngine;

namespace Player.Gameplay.ClickableItems
{

    public class ItemDroppables : ItemDependent
    {
        [SerializeField] protected Inventory.Object[] droppables;

        protected virtual void AddItems() { }

        protected bool CanAddItems()
        {
            return neededObjects.Length + Inventory.Inventory.SpaceLeft - droppables.Length >= 0;

        }
    }


}