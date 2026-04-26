using UnityEngine;
using System.Collections.Generic;
using PandaCafe.NPC;
using PandaCafe.Interaction;

namespace PandaCafe.Menu
{
    public class OrderManager : MonoBehaviour
    {
        private List<OrderItem> activeOrders = new List<OrderItem>();

         public void RegisterOrder(Guest guest, Table table, MenuItemSO menuItemSO, int quantity = 1)
        {
            if (guest == null || table == null || menuItemSO == null || quantity <= 0)
            {
                return;
            }

            activeOrders.Add(new OrderItem(menuItemSO, quantity));
        }

        //public int ActiveOrderCount => activeOrders.Count;
    }
}