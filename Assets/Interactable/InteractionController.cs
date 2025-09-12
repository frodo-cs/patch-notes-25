using UnityEngine;

namespace Player {
    public class InteractionController : MonoBehaviour {
        public static InteractionController Instance;
        public Inventory.Object ItemSelected;

        private void Awake() {
            if(Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            Inventory.Inventory.ItemSelected += OnItemSelected;
        }

        private void OnItemSelected(Inventory.Object obj)
        {
            ItemSelected = obj;
        }

        private void OnDestroy()
        {
            Inventory.Inventory.ItemSelected -= OnItemSelected;
        }
    }
}
