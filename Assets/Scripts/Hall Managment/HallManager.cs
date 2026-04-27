using UnityEngine;
using PandaCafe.Interaction;
using PandaCafe.NPC;
using System.Collections.Generic;
using PandaCafe.Menu;

namespace PandaCafe.HallManagment
{
    public class HallManager : MonoBehaviour
    {
        private MenuData menuData;
        private OrderManager orderManager;
        private SeatingService seatingService;
        private WaiterOrderService waiterOrderService;

        private QueueManager queueManager;
        private Waiter waiter;

        public void Init(QueueManager queueManager, Waiter waiter, MenuData menuData, OrderManager orderManager)
        {
            this.queueManager = queueManager;
            this.waiter = waiter;
            this.menuData = menuData;
            this.orderManager = orderManager;

            seatingService = new SeatingService();
            waiterOrderService = new WaiterOrderService(menuData, orderManager);

            this.waiter.ArrivedAtDestination += waiterOrderService.HandleWaiterArrived;
        }

         private void OnDestroy()
        {
            if (waiter != null)
            {
                waiter.ArrivedAtDestination -= waiterOrderService.HandleWaiterArrived;
            }
        }

        public void RequestWaiter(IInteractable component)
        {
            if (!CanInteract(InteractionActor.Waiter, component.Type)) return;

            if (component.TryGetWorldPoint(InteractionActor.Waiter, out Vector3 point))
            {
                bool startedMoving = waiter.MoveTo(point);
                
                if (!startedMoving)
                {
                    waiterOrderService.SetPendingTable(null);
                    return;
                }

                if (component is Table table && seatingService.TryGetGuestAtTable(table, out Guest guest) && guest.State == GuestState.WaitingForOrder)
                {
                    waiterOrderService.SetPendingTable(table);
                    return;
                }

                waiterOrderService.SetPendingTable(null);
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
                    guest.PatienceExpired += HandleGuestPatienceExpired;
                    seatingService.TrySetGuest(guest, table);
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


        private void HandleGuestPatienceExpired(Guest guest)
        {
            if (guest == null) return;

            Table targetTable = null;
            foreach (KeyValuePair<Table, Guest> pair in seatingService.guestsByTable)
            {
                if (pair.Value != guest) continue;
                targetTable = pair.Key;
                break;
            }

            if (targetTable != null)
            {
                seatingService.ClearTable(targetTable);
            }
        }
    }
}

