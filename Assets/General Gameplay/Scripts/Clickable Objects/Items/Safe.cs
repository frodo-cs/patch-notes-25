using Cinematics;
using UnityEngine;

namespace Player.Gameplay.ClickableItems
{
    public class Safe : ItemDroppables
    {
        [SerializeField] GameObject puzzleObject;


        public override void OnInteractStart()
        {
            if (DialogBoxController.IsDialogRunning?.Invoke() == true)
                return;

            var selected = InteractionController.Instance.ItemSelected;

            if (!HasItemNeeded(selected))
            {
                GameObject puzzle = Instantiate(
                    puzzleObject,
                    new Vector3(0, 0, 0),
                    Quaternion.identity
                );
                puzzle.transform.parent = transform;
                Puzzles.NumberInput.OnPasswordCorrect += OpenStorage;
            } else if (HasItemNeeded(selected))
            {
                OpenStorage();
            }
        }

        protected override void AddItems()
        {
            var selected = InteractionController.Instance.ItemSelected;
            if (selected && HasItemNeeded(selected))
            {
                Debug.Log("Used item on safe");
                InteractionController.Instance.ClearSelection();
                Inventory.Inventory.RemoveItem?.Invoke(selected);
            }

            gameObject.GetComponent<Collider2D>().enabled = false;
            Inventory.Inventory.AddItems?.Invoke(droppables);
            droppables = new Inventory.Object[0];
        }

        private void OpenStorage()
        {
            dialog.text = "You opened the safe";
            OpenDialog();
            DialogBoxController.OnDialogEnds += AddItems;
        }


        private void OnDestroy()
        {
            DialogBoxController.OnDialogEnds -= AddItems;
            Puzzles.NumberInput.OnPasswordCorrect -= OpenStorage;
        }


    }

}
