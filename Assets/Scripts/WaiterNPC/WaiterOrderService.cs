using PandaCafe.Interaction;
using PandaCafe.Menu;
using PandaCafe.NPC;
using PandaCafe.HallManagment;
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
        private OrderItem carriedOrder;
        private GameObject carriedDish;

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
                    Debug.Log("deliver dish");
                    break;
            }

            currentTask = WaiterTask.None;
        }

        private WaiterTask TryPrepareTableTask(Table table)
        {
            if (table == null) return WaiterTask.None;

            if (carriedOrder != null && carriedOrder.Table == table)
            {
                return WaiterTask.DeliveringDish;
            }

            if (seatingService.TryGetGuestAtTable(table, out Guest guest) && guest.State == GuestState.WaitingForOrder)
            {
                pendingOrderTable = table;
                return WaiterTask.TakingOrder;
            }

            pendingOrderTable = null;
            return WaiterTask.None;
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
                carriedDish.transform.SetParent(waiter.transform, true);
            }
        }

        private void HandleDeliveringDish()
        {
            if (carriedOrder == null) return;

            if (carriedOrder.Guest != null && carriedOrder.Guest.State == GuestState.WaitingForFood)
            {
                carriedOrder.Guest.SetState(GuestState.Eating);
            }

            if (carriedDish != null)
            {
                Object.Destroy(carriedDish);
                carriedDish = null;
            }

            carriedOrder = null;
        }
    }
}