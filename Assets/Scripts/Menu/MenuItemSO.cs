using UnityEngine;

namespace PandaCafe.Menu
{
    // Menu item data asset
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