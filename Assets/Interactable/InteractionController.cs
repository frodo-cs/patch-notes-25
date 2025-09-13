using System;
using System.Collections.Generic;
using UnityEngine;

namespace Player {
    public class InteractionController : MonoBehaviour
    {
        public static InteractionController Instance;

        public List<int> SelectedIndexes { get; private set; } = new();
        public bool MultipleSelection { get; private set; }
        public Inventory.Object ItemSelected { get; private set; }

        public static event Action OnSelectionChanged;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        public void OnLeftClick(int index)
        {
            MultipleSelection = false;

            if (SelectedIndexes.Contains(index))
            {
                ClearSelection();
                return;
            }

            SelectedIndexes.Clear();
            SelectedIndexes.Add(index);

            UpdateSelection(index);
        }

        public void OnRightClick(int index)
        {
            if (!MultipleSelection)
            {
                MultipleSelection = true;
                SelectedIndexes.Clear();
            }

            if (SelectedIndexes.Contains(index))
            {
                SelectedIndexes.Remove(index);

                if (SelectedIndexes.Count == 0)
                {
                    ClearSelection();
                } else
                {
                    UpdateSelection(SelectedIndexes[SelectedIndexes.Count - 1]);
                }
                return;
            }

            if (SelectedIndexes.Count < 2)
            {
                SelectedIndexes.Add(index);
                UpdateSelection(index);
            }
        }

        private void UpdateSelection(int index)
        {
            ItemSelected = Inventory.Inventory.Instance.GetItems()[index].obj;
            OnSelectionChanged?.Invoke();
        }

        public void ClearSelection()
        {
            SelectedIndexes.Clear();
            MultipleSelection = false;
            ItemSelected = null;
            OnSelectionChanged?.Invoke();
        }

        public int GetLastIndexSelected()
        {
            return SelectedIndexes.Count == 0 ? -1 : SelectedIndexes[SelectedIndexes.Count - 1];
        }
    }
}
