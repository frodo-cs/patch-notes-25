using Cinematics;
using UnityEngine;
using static PersistentData;

namespace Player.Gameplay.ClickableItems
{
    public class CoffeeDispenser : ItemDependent
    {
        private enum DispenserState { Untouched, Dispensed }
        private DispenserState state;

        protected override void Start()
        {
            LoadState();
        }

        public override void OnInteractStart()
        {
            if (DialogBoxController.IsDialogRunning?.Invoke() == true)
                return;

            var selected = InteractionController.Instance.ItemSelected;

            switch (state)
            {
                case DispenserState.Untouched:
                    HandleUntouched(selected);
                    break;

                case DispenserState.Dispensed:
                    dialog.text = "Huh, seems that was all.";
                    OpenDialog();
                    break;
            }
        }

        private void HandleUntouched(Inventory.Object selected)
        {
            if (!HasItemNeeded(selected))
            {
                dialog.text = "Bet I could find some cash in here";
                OpenDialog();
                return;
            }

            dialog.text = "Nothing like an early payday.";
            OpenDialog();

            Inventory.Inventory.RemoveItem?.Invoke(selected);

            DialogBoxController.OnDialogEnds += FinalizeDispense;
        }

        private void FinalizeDispense()
        {
            DialogBoxController.OnDialogEnds -= FinalizeDispense;
            state = DispenserState.Dispensed;
            SaveState();
        }

        private void SaveState()
        {
            Save(itemId, new IntData("State", (int)state));
        }

        private void LoadState()
        {
            var data = GetData(itemId, "State") as IntData;
            state = data != null ? (DispenserState)data.value : DispenserState.Untouched;
        }

        private void OnDestroy()
        {
            DialogBoxController.OnDialogEnds -= FinalizeDispense;
        }
    }
}
