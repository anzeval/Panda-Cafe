using UnityEngine;
using System.Collections.Generic;
using PandaCafe.NPC;
using PandaCafe.Interaction;
using System;

namespace PandaCafe.Menu
{
    public class OrderManager : MonoBehaviour
    {
        private readonly List<OrderItem> activeOrders = new List<OrderItem>();

        public event Action<OrderItem> OrderRegistered;

         public void RegisterOrder(Guest guest, Table table, MenuItemSO menuItemSO, int quantity = 1)
        {

            if (guest == null || table == null || menuItemSO == null || quantity <= 0) return;

            OrderItem order = new OrderItem(menuItemSO, quantity, guest, table);
            activeOrders.Add(order);
            OrderRegistered?.Invoke(order);
        }

        public void CompleteOrder(OrderItem order)
        {
            if (order == null) return;

            activeOrders.Remove(order);
        }
    }
}