using UnityEngine;

// Stores shop upgrade asset data.
// Defines price, icon, type, and value.
// Used by shop items and upgrade logic.
namespace PandaCafe.Core.Shop
{
    [CreateAssetMenu(fileName = "ShopUpgradeSO", menuName = "Scriptable Objects/ShopUpgradeSO")]
    public class ShopUpgradeSO : ScriptableObject
    {
        public string itemName;
        [TextArea] public string description;

        public int price;

        public Sprite icon;

        public string id;
        public UpgradeType type;
        public float value;
    }
}
