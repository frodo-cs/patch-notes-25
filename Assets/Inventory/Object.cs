using UnityEngine;

namespace Player.Inventory {
    [CreateAssetMenu(fileName = "Generic Inventory Obj", menuName = "General Gameplay/Inventory Object")]
    public class Object : ScriptableObject {
        public Texture2D Portrait;
        public Texture2D PortraitHover;
        public Texture2D PortraitMerge;
        public string objectName;
        [Multiline] public string highlightText;
        public int maxAmount = 1;
        public Object[] combinableWith;
        public bool canMerge = false;
    }

}