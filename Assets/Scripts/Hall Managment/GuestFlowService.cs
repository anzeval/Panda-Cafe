using UnityEngine;
using PandaCafe.Interaction;
using PandaCafe.NPC;

namespace PandaCafe.HallManagment
{
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

        public void RequestGuest(IInteractable component, Guest guest)
        {
            if (component == null || guest == null) return;
            if (!HallInteractionRouter.CanInteract(InteractionActor.Guest, component.Type)) return;
            if (!component.TryGetWorldPoint(InteractionActor.Guest, out Vector3 point)) return;

            bool startedMoving = guest.MoveToTable(point);
            
            if (!startedMoving) return;

            guest.SetState(GuestState.GoingToTable);

            if (component is Table table)
            {
                bool seated = seatingService.SeatGuestAtTable(guest, table);
                if (seated)
                {
                    guestPatienceCoordinator.RegisterGuestAtTable(guest);
                }
            }

            queueManager.RemoveGuestFromQueue(guest.OrdinalQueueNumber);
            queueManager.ReorderQueue();
        }
    }
}