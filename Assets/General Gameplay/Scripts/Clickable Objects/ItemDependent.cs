using Cinematics;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using static PersistentData;

namespace Player.Gameplay
{
    public class ItemDependent : DescriptionObject
    {
        [Space(5)]
        [SerializeField] protected string itemId;

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

        protected virtual void Start()
        {
            LoadTouched();
        }

        protected bool HasItemNeeded(Inventory.Object selected)
        {
            return neededSet.Contains(selected);
        }

        protected void SaveTouched()
        {
            Save(itemId, new BoolData("Touched", touched));
        }

        protected void LoadTouched()
        {
            var data = GetData(itemId, "Touched") as BoolData;
            if (data != null)
            {
                touched = data.value;
            }

        }

    }

}
