using UnityEngine;

// Stores menu item asset data.
// Defines prefab, sprite, name, and price.
// Used by orders, kitchen, and UI.
namespace PandaCafe.Menu
{
    [CreateAssetMenu(fileName = "MenuItemSO", menuName = "Scriptable Objects/MenuItemSO")]
    public class MenuItemSO : ScriptableObject
    {
        // Spawned object
        public GameObject Prefab;

        // UI sprite
        public Sprite Sprite;

        // Display name
        public string Name;

        // Item price
        public int Price;
    }
}