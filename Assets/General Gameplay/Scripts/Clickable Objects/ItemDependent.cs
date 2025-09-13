using Cinematics;
using System.Collections.Generic;
using UnityEngine;

namespace Player.Gameplay
{
    public class ItemDependent : DescriptionObject
    {
        [SerializeField] protected Inventory.Object[] neededObjects;
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

        protected void OpenDialog()
        {
            var a = DialogBoxController.IsDialogRunning?.Invoke();
            if (a != null && !a.Value)
                DialogBoxController.PlayDialog?.Invoke(dialog);
        }

        protected void SetTouched()
        {
            touched = true;
        }
    }

}
