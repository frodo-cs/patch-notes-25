using Cinematics;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Player.Gameplay
{
    public class ItemDependent : DescriptionObject
    {
        Dialog lockedDialog;

        [Space(5)]
        [SerializeField] protected Inventory.Object[] neededObjects;

        [Space(5)]
        [SerializeField] UnityEvent onUnlock;
        private HashSet<Inventory.Object> neededSet;
        protected bool touched = false;

        private void Awake()
        {
            neededSet = new HashSet<Inventory.Object>(neededObjects);
        }

        protected bool HasItemNeeded(Inventory.Object selected)
        {
            return neededSet.Contains(selected);
        }

        protected void SetTouched()
        {
            touched = true;
        }
    }

}
