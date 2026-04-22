using UnityEngine;
using PandaCafe.Interaction;
using PandaCafe.NPC;
using PandaCafe.AI;

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
            if(component.TryGetWorldPoint(InteractionActor.Waiter, out Vector3 point))
            {
                waiter.MoveTo(point); 
            }
            
        }

        public void RequestGuest(IInteractable component, Guest guest)
        {
            if(component.Type == InteractionType.Table)
            {
                if(component.TryGetWorldPoint(InteractionActor.Guest, out Vector3 point))
                {
                    guest.SetState(GuestState.GoingToTable); 
                    guest.MoveTo(point);

                    queueManager.RemoveGuestFromQueue(guest.OrdinalQueueNumber);
                    queueManager.ReorderQueue(); 
                }
            }
        }
    }
}

