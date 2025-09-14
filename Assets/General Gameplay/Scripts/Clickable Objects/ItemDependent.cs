using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static PersistentData;

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

    }

}
