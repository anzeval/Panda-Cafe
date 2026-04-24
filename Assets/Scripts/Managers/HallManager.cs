using UnityEngine;
using PandaCafe.Interaction;
using PandaCafe.NPC;
using System.Collections.Generic;

namespace PandaCafe.Managers
{
    public class HallManager : MonoBehaviour
    {
        private QueueManager queueManager;
        private Waiter waiter;

        private Dictionary<Table, Guest> guestsByTable;

        private void Awake()
        {
            guestsByTable = new Dictionary<Table, Guest>();
        }

        public void Init(QueueManager queueManager, Waiter waiter)
        {
            this.queueManager = queueManager;
            this.waiter = waiter;
        }

        public void RequestWaiter(IInteractable component)
        {
            if (!CanInteract(InteractionActor.Waiter, component.Type)) return;

            if (component.TryGetWorldPoint(InteractionActor.Waiter, out Vector3 point))
            {
                waiter.MoveTo(point);
            }
        }

        public void RequestGuest(IInteractable component, Guest guest)
        {
            if (!CanInteract(InteractionActor.Guest, component.Type)) return;

            if (component.TryGetWorldPoint(InteractionActor.Guest, out Vector3 point))
            {
                bool startedMoving = guest.MoveToTable(point);

                if(!startedMoving) return;

                guest.SetState(GuestState.GoingToTable);

                if (component is Table table)
                {
                    table.OccupyTable(guest);
                    guestsByTable[table] = guest;
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

        public bool TryGetGuestAtTable(Table table, out Guest guest)
        {
            guest = null;

            if (table == null)
            {
                return false;
            }

            if (!guestsByTable.TryGetValue(table, out Guest _guest))
            {
                return false;
            }

            guest = _guest;
            return guest != null;
        }

        public void ClearTable(Table table)
        {
            if (table == null) return;

            table.FreeTable();
            guestsByTable.Remove(table);
        }
    }
}

