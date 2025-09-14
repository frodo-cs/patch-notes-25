using Player.Inventory;
using UnityEngine;

[CreateAssetMenu(fileName = "New Shop Data", menuName = "General Gameplay/Shop Data")]
public class ShopData : ScriptableObject
{
    public ShopSlot[] slots;

    [System.Serializable]
    public struct ShopSlot {
        public Player.Inventory.Object obj;
        public int price;
    }
}
