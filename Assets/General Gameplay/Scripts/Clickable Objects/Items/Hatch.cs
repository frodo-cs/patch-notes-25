using Cinematics;
using UnityEngine;
using static PersistentData;

namespace Player.Gameplay.ClickableItems
{

    public class Hatch : ItemDependent
    {
        // la inspiracion llega al puro final ah
        [SerializeField] private SpriteRenderer sr;
        [SerializeField] private Texture2D[] bgs = new Texture2D[3];

        private enum HatchState { CarDown, CarUp, OpenHatch }
        private HatchState state;

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
                case HatchState.CarDown:

                    if (!HasItemNeeded(selected))
                    {
                        dialog.text = "Too heavy for you";
                        OpenDialog();
                        return;
                    } else
                    {
                        LiftCar();
                    }
                    break;
                case HatchState.CarUp:
                    if (!HasItemNeeded(selected))
                    {
                        dialog.text = "You need a keycard";
                        OpenDialog();
                        return;
                    } else
                    {
                        OpenHatch();
                    }

                    break;
                case HatchState.OpenHatch:
                    FinalStep();
                    break;

            }
        }

        protected override bool HasItemNeeded(Inventory.Object selected)
        {
            int index = (int)state;

            if (index < neededObjects.Length)
            {
                return neededObjects[index] == selected;
            }
            return false;
        }

        private void FinalStep()
        {
            var result = ChangeScene.LoadScene?.Invoke(16);
            if (result == null || !result.Value)
                Debug.LogWarning("Failed to change scene");
        }

        private void OpenHatch()
        {
            state = HatchState.OpenHatch;
            SaveState();
            sr.sprite = Utilities.ToSprite(bgs[(int)state]);
            var selected = InteractionController.Instance.ItemSelected;
            Inventory.Inventory.RemoveItem?.Invoke(selected);
        }

        private void LiftCar()
        {
            state = HatchState.CarUp;
            SaveState();
            sr.sprite = Utilities.ToSprite(bgs[(int)state]);
            var selected = InteractionController.Instance.ItemSelected;
            Inventory.Inventory.RemoveItem?.Invoke(selected);
        }

        protected void SaveState()
        {
            Save(itemId, new IntData("State", (int)state));
        }

        protected void LoadState()
        {
            var data = GetData(itemId, "State") as IntData;
            if (data != null)
            {
                state = (HatchState)data.value;
            }

        }

    }
}