using Cinematics;
using UnityEngine;
using static PersistentData;

namespace Player.Gameplay.ClickableItems
{
    public class Crate : ItemDroppables
    {
        [SerializeField] private SpriteRenderer sr;
        [SerializeField] private Texture2D openSprite;

        private enum CrateState { Closed, Open }
        private CrateState state;

        protected override void Start()
        {
            LoadState();
            LoadDroppables();
            UpdateVisual();
        }

        public override void OnInteractStart()
        {
            if (DialogBoxController.IsDialogRunning?.Invoke() == true)
                return;
            var selected = InteractionController.Instance.ItemSelected;
            switch (state)
            {
                case CrateState.Closed:
                    if (HasItemNeeded(selected))
                    {

                        OpenCrate();
                    } else
                    {
                        dialog.text = "I need more than my hands to open this";
                        OpenDialog();
                    }
                    break;

                case CrateState.Open:
                    if (droppables.Length > 0 && CanAddItems())
                    {
                        AddItems();
                    } else
                    {
                        dialog.text = "Doesn't seem to be more shit in here";
                        OpenDialog();
                    }

                    break;
            }
        }

        private void OpenCrate()
        {
            state = CrateState.Open;
            SaveState();
            UpdateVisual();
            AddItems();
        }

        protected override void AddItems()
        {
            if (CanAddItems())
            {

                Inventory.Inventory.AddItems?.Invoke(droppables);
                droppables = new Inventory.Object[0];
                SaveDroppables();
            } else
            {

                dialog.text = "I have no empty pockets";
                OpenDialog();
            }

        }

        private void UpdateVisual()
        {
            if (sr == null)
                return;

            if (state == CrateState.Open)
            {
                sr.sprite = Utilities.ToSprite(openSprite);
            }
        }

        private void SaveState()
        {
            Save(name, new IntData("State", (int)state));
        }

        private void LoadState()
        {
            var data = GetData(name, "State") as IntData;
            state = data != null ? (CrateState)data.value : CrateState.Closed;
        }
    }

}
