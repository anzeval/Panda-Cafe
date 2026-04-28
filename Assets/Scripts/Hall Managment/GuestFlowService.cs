using UnityEngine;
using PandaCafe.Interaction;
using PandaCafe.NPC;

namespace PandaCafe.HallManagment
{
    // Handles guest movement from queue to table
    public class GuestFlowService : MonoBehaviour
    {
        private readonly QueueManager queueManager;
        private readonly SeatingService seatingService;
        private readonly GuestPatienceCoordinator guestPatienceCoordinator;

        public GuestFlowService(QueueManager queueManager, SeatingService seatingService, GuestPatienceCoordinator guestPatienceCoordinator)
        {
            this.queueManager = queueManager;
            this.seatingService = seatingService;
            this.guestPatienceCoordinator = guestPatienceCoordinator;
        }

        // Process guest interaction with target
        public void RequestGuest(IInteractable component, Guest guest)
        {
            // Validate input
            if (component == null || guest == null) return;

            // Check interaction rules
            if (!HallInteractionRouter.CanInteract(InteractionActor.Guest, component.Type)) return;

            // Get target position
            if (!component.TryGetWorldPoint(InteractionActor.Guest, out Vector3 point)) return;

            // Start movement
            bool startedMoving = guest.MoveToTable(point);
            if (!startedMoving) return;

            // Set movement state
            guest.SetState(GuestState.GoingToTable);

            // If target is table, try to seat guest
            if (component is Table table)
            {
                bool seated = seatingService.SeatGuestAtTable(guest, table);

                if (seated)
                {
                    // Subscribe to patience events
                    guestPatienceCoordinator.RegisterGuestAtTable(guest);
                }
            }

            // Remove from queue and update order
            queueManager.RemoveGuestFromQueue(guest.OrdinalQueueNumber);
            queueManager.ReorderQueue();
        }
    }
}