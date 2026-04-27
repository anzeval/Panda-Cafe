using UnityEngine;
using PandaCafe.Interaction;
using PandaCafe.NPC;
using PandaCafe.Menu;

namespace PandaCafe.HallManagment
{
    public class HallManager : MonoBehaviour
    {
        private QueueManager queueManager;
        private Waiter waiter;

        private SeatingService seatingService;
        private GuestPatienceCoordinator guestPatienceCoordinator;
        private GuestFlowService guestFlowService;
        private WaiterOrderService waiterOrderService;

        public void Init(QueueManager queueManager, Waiter waiter, MenuData menuData, OrderManager orderManager)
        {
            this.queueManager = queueManager;
            this.waiter = waiter;

            seatingService = new SeatingService();
            guestPatienceCoordinator = new GuestPatienceCoordinator(seatingService);
            guestFlowService = new GuestFlowService(this.queueManager, seatingService, guestPatienceCoordinator);
            waiterOrderService = new WaiterOrderService(menuData, orderManager, seatingService);

            this.waiter.ArrivedAtDestination += waiterOrderService.HandleWaiterArrived;
        }

         private void OnDestroy()
        {
            if (waiter != null && waiterOrderService != null)
            {
                waiter.ArrivedAtDestination -= waiterOrderService.HandleWaiterArrived;
            }
        }

        public void RequestWaiter(IInteractable component)
        {
            waiterOrderService.RequestWaiter(component, waiter);
        }

        public void RequestGuest(IInteractable component, Guest guest)
        {
            guestFlowService.RequestGuest(component, guest);
        }
    }
}

