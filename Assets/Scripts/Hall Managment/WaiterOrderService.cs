using PandaCafe.Interaction;
using PandaCafe.Menu;

namespace PandaCafe.HallManagment
{
    public class WaiterOrderService 
    {
        private MenuData menuData;
        private OrderManager orderManager;

        private Table pendingOrderTable;

        public WaiterOrderService(MenuData menuData, OrderManager orderManager)
        {
            this.menuData = menuData;
            this.orderManager = orderManager;
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

        public void SetPendingTable(Table table = null)
        {
            pendingOrderTable = table;
        }
    }
}