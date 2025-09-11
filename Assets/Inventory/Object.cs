using UnityEngine;

namespace Player.Inventory {
    [CreateAssetMenu(fileName = "Generic Inventory Obj", menuName = "General Gameplay/Inventory Object")]
    public class Object : ScriptableObject {
        public Texture2D Portrait;
        [Multiline] public string highlightText;
        public int maxAmount = 1;
    }

}