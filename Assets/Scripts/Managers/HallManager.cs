using UnityEngine;
using PandaCafe.Interaction;
using PandaCafe.NPC;

namespace PandaCafe.Managers
{
    public class HallManager : MonoBehaviour
    {
        private QueueManager queueManager;
        private Waiter waiter;

        public void Init(QueueManager queueManager, Waiter waiter)
        {
            this.queueManager = queueManager;
            this.waiter = waiter;
        }

        public void RequestWaiter(IInteractable component)
        {
            if (!CanInteract(InteractionActor.Waiter, component.Type))
            {
                return;
            }

            if (component.TryGetWorldPoint(InteractionActor.Waiter, out Vector3 point))
            {
                waiter.MoveTo(point);
            }
        }

        public void RequestGuest(IInteractable component, Guest guest)
        {
            if (!CanInteract(InteractionActor.Guest, component.Type))
            {
                return;
            }

            if (component.TryGetWorldPoint(InteractionActor.Guest, out Vector3 point))
            {
                bool startedMoving = guest.MoveTo(point);

                guest.SetState(GuestState.GoingToTable);

                if(component is Table)
                {
                    
                }

                queueManager.RemoveGuestFromQueue(guest.OrdinalQueueNumber);
                queueManager.ReorderQueue();
            }
        }

        private bool CanInteract(InteractionActor actor, InteractionType interactionType)
        {
            if (actor == InteractionActor.Guest)
            {
                return interactionType == InteractionType.Table;
            }

            return interactionType == InteractionType.Table || interactionType == InteractionType.Kitchen || interactionType == InteractionType.Trash;
        }
    }
}

