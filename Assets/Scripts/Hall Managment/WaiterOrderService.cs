using PandaCafe.Interaction;
using PandaCafe.Menu;
using PandaCafe.NPC;
using UnityEngine;

namespace PandaCafe.HallManagment
{
    public class WaiterOrderService 
    {
        private readonly MenuData menuData;
        private readonly OrderManager orderManager;
        private readonly SeatingService seatingService;

        private Table pendingOrderTable;

        public WaiterOrderService(MenuData menuData, OrderManager orderManager, SeatingService seatingService)
        {
            this.menuData = menuData;
            this.orderManager = orderManager;
            this.seatingService = seatingService;
        }

        public void RequestWaiter(IInteractable component, Waiter waiter)
        {
            if (component == null || waiter == null) return;
            if (!HallInteractionRouter.CanInteract(InteractionActor.Waiter, component.Type)) return;
            if (!component.TryGetWorldPoint(InteractionActor.Waiter, out Vector3 point)) return;

            bool startedMoving = waiter.MoveTo(point);
            if (!startedMoving)
            {
                pendingOrderTable = null;
                return;
            }

            if (component is Table table
                && seatingService.TryGetGuestAtTable(table, out Guest guest)
                && guest.State == GuestState.WaitingForOrder)
            {
                pendingOrderTable = table;
                return;
            }

            pendingOrderTable = null;
        }

        public void HandleWaiterArrived()
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
    }
}