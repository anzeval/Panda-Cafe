using UnityEngine;
using PandaCafe.Interaction;
using PandaCafe.NPC;
using System.Collections.Generic;
using PandaCafe.Menu;

namespace PandaCafe.Managers
{
    public class HallManager : MonoBehaviour
    {
        private MenuData menuData;
        private OrderManager orderManager;

        private QueueManager queueManager;
        private Waiter waiter;

        private Dictionary<Table, Guest> guestsByTable;
        private Table pendingOrderTable;

        private void Awake()
        {
            guestsByTable = new Dictionary<Table, Guest>();
        }

        public void Init(QueueManager queueManager, Waiter waiter, MenuData menuData, OrderManager orderManager)
        {
            this.queueManager = queueManager;
            this.waiter = waiter;
            this.menuData = menuData;
            this.orderManager = orderManager;

            this.waiter.ArrivedAtDestination += HandleWaiterArrived;
        }

        private void OnDestroy()
        {
            if (waiter != null)
            {
                waiter.ArrivedAtDestination -= HandleWaiterArrived;
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
                    pendingOrderTable = null;
                    return;
                }

                if (component is Table table && TryGetGuestAtTable(table, out Guest guest) && guest.State == GuestState.WaitingForOrder)
                {
                    pendingOrderTable = table;
                    return;
                }

                pendingOrderTable = null;
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

             if (table.CurrentGuest != null)
            {
                table.CurrentGuest.PatienceExpired -= HandleGuestPatienceExpired;
            }

            table.FreeTable();
            guestsByTable.Remove(table);
        }

        private void HandleGuestPatienceExpired(Guest guest)
        {
            if (guest == null) return;

            Table targetTable = null;
            foreach (KeyValuePair<Table, Guest> pair in guestsByTable)
            {
                if (pair.Value != guest) continue;
                targetTable = pair.Key;
                break;
            }

            if (targetTable != null)
            {
                ClearTable(targetTable);
            }
        }

        private void HandleWaiterArrived()
        {
            if (pendingOrderTable == null) return;

            if (!TryGetGuestAtTable(pendingOrderTable, out Guest guest) || guest.State != GuestState.WaitingForOrder)
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
    }
}

