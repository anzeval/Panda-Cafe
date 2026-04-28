using UnityEngine;
using PandaCafe.Interaction;
using PandaCafe.NPC;
using PandaCafe.Menu;
using PandaCafe.WaiterNPC;

namespace PandaCafe.HallManagment
{
    // Central coordinator for hall systems
    public class HallManager : MonoBehaviour
    {
        private QueueManager queueManager;
        private Waiter waiter;

        private SeatingService seatingService;
        private GuestPatienceCoordinator guestPatienceCoordinator;
        private GuestFlowService guestFlowService;
        private WaiterOrderService waiterOrderService;
        private Kitchen kitchen;

        // Initialize all services
        public void Init(QueueManager queueManager, Waiter waiter, MenuData menuData, OrderManager orderManager, Kitchen kitchen)
        {
            this.queueManager = queueManager;
            this.waiter = waiter;
            this.kitchen = kitchen;

            // Create services
            seatingService = new SeatingService();
            guestPatienceCoordinator = new GuestPatienceCoordinator(seatingService);
            guestFlowService = new GuestFlowService(this.queueManager, seatingService, guestPatienceCoordinator);
            waiterOrderService = new WaiterOrderService(menuData, orderManager, seatingService, kitchen);

            // Subscribe to waiter events
            this.waiter.ArrivedAtDestination += waiterOrderService.HandleWaiterArrived;
        }

        private void OnDestroy()
        {
            // Unsubscribe from events
            if (waiter != null && waiterOrderService != null)
            {
                waiter.ArrivedAtDestination -= waiterOrderService.HandleWaiterArrived;
            }
        }

        // Forward waiter request
        public void RequestWaiter(IInteractable component)
        {
            waiterOrderService.RequestWaiter(component, waiter);
        }

        // Forward guest request
        public void RequestGuest(IInteractable component, Guest guest)
        {
            guestFlowService.RequestGuest(component, guest);
        }
    }
}