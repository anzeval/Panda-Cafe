using UnityEngine;
using PandaCafe.Interaction;
using PandaCafe.NPC;
using PandaCafe.Menu;
using PandaCafe.WaiterNPC;
using PandaCafe.Interaction.InteractionObjects;
using PandaCafe.Core.LevelManagment;

// Coordinates hall services.
// Routes guest and waiter interaction requests.
// Connects seating, queue, and order systems.
namespace PandaCafe.HallManagment
{
    public class HallManager : MonoBehaviour
    {
        private Waiter waiter;

        private SeatingService seatingService;
        private GuestPatienceCoordinator guestPatienceCoordinator;
        private GuestFlowService guestFlowService;
        private WaiterOrderService waiterOrderService;

        // Initialize all services
        public void Init(QueueManager queueManager, Waiter waiter, MenuData menuData, OrderManager orderManager, Kitchen kitchen, GoalManager goalManager)
        {
            this.waiter = waiter;

            // Create services
            seatingService = new SeatingService();
            guestPatienceCoordinator = new GuestPatienceCoordinator(seatingService, goalManager);
            guestFlowService = new GuestFlowService(queueManager, seatingService, guestPatienceCoordinator);
            waiterOrderService = new WaiterOrderService(menuData, orderManager, seatingService, kitchen, goalManager);

            // Subscribe to waiter events
            this.waiter.ArrivedAtDestination += waiterOrderService.HandleWaiterArrived;
        }

        // Clean up event subscriptions
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
