using UnityEngine;
using System.Collections.Generic;

namespace PandaCafe.Menu
{
    public class MenuData : MonoBehaviour
    {
        [SerializeField] private List<MenuItemSO> menuItems = new List<MenuItemSO>();
        private System.Random rdm = new System.Random();
        
        public MenuItemSO TryGetMenuItem(int index)
        {
            if(!IsArrayValid(menuItems) || index < 0 || index >= menuItems.Count) return null;

            return menuItems[index];
        }

        public MenuItemSO GetRandomMenuItem()
        {
            if(!IsArrayValid(menuItems)) return null;

            return menuItems[rdm.Next(0, menuItems.Count)];
        }

        private bool IsArrayValid(List<MenuItemSO> array)
        {
            return array != null && array.Count > 0;
        }
    }   
}