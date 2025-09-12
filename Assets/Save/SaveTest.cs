using UnityEngine;

namespace Player.Gameplay {
    public class SaveTest : MouseReaction {
        [SerializeField] SpriteRenderer renderer;

        [SerializeField] int color = 0;
        [SerializeField] Color[] colors;

        private void Start() {
            OnLoad();
        }

        public override void OnInteractStart() {
            color = (color + 1) % colors.Length;
            renderer.color = colors[color];

            PersistentData.Save?.Invoke(transform.name, new PersistentData.IntData("color", color));
        }

        void OnLoad() {
            PersistentData.PropertyData data = PersistentData.GetData?.Invoke(transform.name, "color");
            if(data != null) {
                Debug.Log(data.getData());
                color = (int)data.getData();
                renderer.color = colors[color];
            }
        }
    }

}