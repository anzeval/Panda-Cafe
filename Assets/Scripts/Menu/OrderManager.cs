using UnityEngine;
using System.Collections.Generic;
using PandaCafe.NPC;
using PandaCafe.Interaction;
using System;

namespace PandaCafe.Menu
{
    // Manages active orders
    public class OrderManager : MonoBehaviour
    {
        // Current orders list
        private readonly List<OrderItem> activeOrders = new List<OrderItem>();

        // Notify when order is created
        public event Action<OrderItem> OrderRegistered;

        // Create and register order
        public void RegisterOrder(Guest guest, Table table, MenuItemSO menuItemSO, int quantity = 1)
        {
            // Validate input
            if (guest == null || table == null || menuItemSO == null || quantity <= 0) return;

            OrderItem order = new OrderItem(menuItemSO, quantity, guest, table);

            activeOrders.Add(order);

            OrderRegistered?.Invoke(order);
        }

        // Remove completed order
        public void CompleteOrder(OrderItem order)
        {
            if (order == null) return;

            activeOrders.Remove(order);
        }
    }
}