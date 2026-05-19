using UnityEngine;
using System.Collections.Generic;

// Stores available menu items.
// Provides menu item by index or random choice.
// Used when guests create orders.
namespace PandaCafe.Menu
{
    public class MenuData : MonoBehaviour
    {
        [SerializeField] private List<MenuItemSO> menuItems = new List<MenuItemSO>();

        // Random generator
        private System.Random rdm = new System.Random();
        
        // Get item by index
        public MenuItemSO TryGetMenuItem(int index)
        {
            if (!IsArrayValid(menuItems) || index < 0 || index >= menuItems.Count) return null;

            return menuItems[index];
        }

        // Get random item
        public MenuItemSO GetRandomMenuItem()
        {
            if (!IsArrayValid(menuItems)) return null;

            return menuItems[rdm.Next(0, menuItems.Count)];
        }

        // Check list validity
        private bool IsArrayValid(List<MenuItemSO> array)
        {
            return array != null && array.Count > 0;
        }
    }   
}