using PandaCafe.Interaction;
using PandaCafe.Menu;
using PandaCafe.NPC;
using PandaCafe.Core;
using PandaCafe.HallManagment;
using System.Collections.Generic;
using UnityEngine;

namespace PandaCafe.WaiterNPC
{
    public class WaiterOrderService 
    {
        private readonly MenuData menuData;
        private readonly OrderManager orderManager;
        private readonly SeatingService seatingService;
        private readonly Kitchen kitchen;

        private Waiter waiter;
        private Table pendingOrderTable;
        private Table pendingDeliveryTable;
        private Table pendingPaymentTable;
        private OrderItem carriedOrder;
        private GameObject carriedDish;
        private readonly Dictionary<Guest, MenuItemSO> orderedItemsByGuest = new Dictionary<Guest, MenuItemSO>();

        private WaiterTask currentTask;

        public WaiterOrderService(MenuData menuData, OrderManager orderManager, SeatingService seatingService, Kitchen kitchen)
        {
            this.menuData = menuData;
            this.orderManager = orderManager;
            this.seatingService = seatingService;
            this.kitchen = kitchen;
        }

        public void RequestWaiter(IInteractable component, Waiter waiter)
        {
            if (component == null || waiter == null) return;
            if (currentTask != WaiterTask.None) return;

            this.waiter = waiter;

            if (!HallInteractionRouter.CanInteract(InteractionActor.Waiter, component.Type)) return;
            if (!component.TryGetWorldPoint(InteractionActor.Waiter, out Vector3 point)) return;
            if (!waiter.MoveTo(point)) return;

            switch (component)
            {
               case Kitchen:
                    currentTask = WaiterTask.PickingUpDish;
                    break;
                case Table table:
                    currentTask = TryPrepareTableTask(table);
                    break;
                case Trash:
                    currentTask = carriedDish != null ? WaiterTask.DiscardingDish : WaiterTask.None;
                    break;
                default:
                    currentTask = WaiterTask.None;
                    break; 
            }
        }

         public void HandleWaiterArrived()
        {
            switch (currentTask)
            {
                case WaiterTask.TakingOrder:
                    HandleTakingOrder();
                    break;
                case WaiterTask.PickingUpDish:
                    HandlePickingUpDish();
                    break;
                case WaiterTask.DeliveringDish:
                    HandleDeliveringDish();
                    break;
                case WaiterTask.DiscardingDish:
                    HandleDiscardingDish();
                    break;
                case WaiterTask.CollectingPayment:
                    HandleCollectingPayment();
                    break;
            }

            currentTask = WaiterTask.None;
        }

        private WaiterTask TryPrepareTableTask(Table table)
        {
            if (table == null) return WaiterTask.None;

            if (table.HasPendingPayout)
            {
                pendingPaymentTable = table;
                return WaiterTask.CollectingPayment;
            }

            if (CanDeliverToTable(table))
            {
                pendingDeliveryTable = table;
                return WaiterTask.DeliveringDish;
            }

            if (seatingService.TryGetGuestAtTable(table, out Guest guest) && guest.State == GuestState.WaitingForOrder)
            {
                pendingOrderTable = table;
                return WaiterTask.TakingOrder;
            }

            pendingOrderTable = null;
            pendingDeliveryTable = null;
            pendingPaymentTable = null;
            return WaiterTask.None;
        }

        private bool CanDeliverToTable(Table table)
        {
            if (carriedOrder?.MenuItemSO == null) return false;
            if (!seatingService.TryGetGuestAtTable(table, out Guest seatedGuest) || seatedGuest == null) return false;
            if (seatedGuest.State != GuestState.WaitingForFood) return false;

            return orderedItemsByGuest.TryGetValue(seatedGuest, out MenuItemSO orderedItem) && orderedItem == carriedOrder.MenuItemSO;
        }

        private void HandleTakingOrder()
        {
            if (pendingOrderTable == null) return;

            if (!seatingService.TryGetGuestAtTable(pendingOrderTable, out Guest guest) || guest.State != GuestState.WaitingForOrder)
            {
                pendingOrderTable = null;
                return;
            }

            MenuItemSO orderedItem = menuData != null ? menuData.GetRandomMenuItem() : null;
            if (orderedItem != null && orderManager != null)
            {
                orderManager.RegisterOrder(guest, pendingOrderTable, orderedItem);
                orderedItemsByGuest[guest] = orderedItem;
            }

            guest.SetState(GuestState.WaitingForFood);
            pendingOrderTable = null;
        }

        private void HandlePickingUpDish()
        {
            if (kitchen == null || carriedOrder != null) return;

            if (!kitchen.TryTakeReadyDish(out carriedOrder, out carriedDish))
            {
                carriedOrder = null;
                carriedDish = null;
                return;
            }

            if (carriedDish != null && waiter != null)
            {
                waiter.PlaceDishInHands(carriedDish.transform);
            }
        }

        private void HandleDeliveringDish()
        {
            if (carriedOrder == null) return;
            if (carriedOrder == null || pendingDeliveryTable == null) return;
            if (!seatingService.TryGetGuestAtTable(pendingDeliveryTable, out Guest seatedGuest) || seatedGuest == null) return;
            if (!CanDeliverToTable(pendingDeliveryTable)) return;

            seatedGuest.SetState(GuestState.Eating);
            pendingDeliveryTable.AddDishRevenue(carriedOrder.MenuItemSO.Price * carriedOrder.Quantity);
            orderedItemsByGuest.Remove(seatedGuest);

            if (carriedDish != null)
            {
                Object.Destroy(carriedDish);
                carriedDish = null;
            }

            pendingDeliveryTable = null;
            carriedOrder = null;
        }

        private void HandleCollectingPayment()
        {
            if (pendingPaymentTable == null) return;

            int payout = pendingPaymentTable.CollectPendingPayout();
            if (payout > 0)
            {
                GameManager.Instance?.AddCoinsForCurrentLevel(payout);
            }

            pendingPaymentTable = null;
        }

        private void HandleDiscardingDish()
        {
            if (carriedDish != null)
            {
                Object.Destroy(carriedDish);
                carriedDish = null;
            }

            pendingDeliveryTable = null;
            carriedOrder = null;
        }
    }
}