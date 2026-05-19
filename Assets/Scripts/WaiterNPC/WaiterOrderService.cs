using PandaCafe.Interaction;
using PandaCafe.Menu;
using PandaCafe.NPC;
using PandaCafe.Interaction.InteractionObjects;
using PandaCafe.Core.LevelManagment;
using PandaCafe.HallManagment;
using System.Collections.Generic;
using UnityEngine;

// Handles waiter order workflow.
// Chooses table, kitchen, payment, or trash tasks.
// Moves dishes and registers served guests.
namespace PandaCafe.WaiterNPC
{
    public class WaiterOrderService 
    {
        private readonly MenuData menuData;
        private readonly OrderManager orderManager;
        private readonly SeatingService seatingService;
        private readonly GoalManager goalManager;
        private readonly Kitchen kitchen;

        private Waiter waiter;

        // Current table target
        private Table pendingTable;

        // Carried order and visual
        private OrderItem carriedOrder;
        private GameObject carriedDish;

        // Track guest orders
        private readonly Dictionary<Guest, MenuItemSO> orderedItemsByGuest = new Dictionary<Guest, MenuItemSO>();

        private WaiterTask currentTask;
        private Trash openedTrash;

        // Create waiter order service
        public WaiterOrderService(MenuData menuData, OrderManager orderManager, SeatingService seatingService, Kitchen kitchen, GoalManager goalManager)
        {
            this.menuData = menuData;
            this.orderManager = orderManager;
            this.goalManager = goalManager;
            this.seatingService = seatingService;
            this.kitchen = kitchen;
        }

        // Start waiter action
        public void RequestWaiter(IInteractable component, Waiter waiter)
        {
            if (component == null || waiter == null) return;
            if (currentTask != WaiterTask.None) return;

            // Validate interaction
            if (!HallInteractionRouter.CanInteract(InteractionActor.Waiter, component.Type)) return;

            // Determine task
            currentTask = GetTaskFor(component);
            if (currentTask == WaiterTask.None) return;

            if (!component.TryGetWorldPoint(InteractionActor.Waiter, out Vector3 point))
            {
                ClearPendingTask();
                return;
            }

            if (!waiter.MoveTo(point))
            {
                ClearPendingTask();
                return;
            }

            this.waiter = waiter;
            UpdateTrashState(component);
            waiter.UpdateMovementState(carriedDish != null);
        }

        // Determine task for target
        private WaiterTask GetTaskFor(IInteractable component)
        {
            if (component is Kitchen) return WaiterTask.PickingUpDish;
            if (component is Table table) return TryPrepareTableTask(table);
            if (component is Trash) return carriedDish != null ? WaiterTask.DiscardingDish : WaiterTask.None;

            return WaiterTask.None;
        }

        // Sync trash state with waiter task
        private void UpdateTrashState(IInteractable component)
        {
            if (openedTrash != null && !ReferenceEquals(openedTrash, component))
            {
                openedTrash.SetOpened(false);
                openedTrash = null;
            }

            if (component is Trash trash)
            {
                trash.SetOpened(true);
                openedTrash = trash;
            }
        }

        // Execute task on arrival
        public void HandleWaiterArrived()
        {
            switch (currentTask)
            {
                case WaiterTask.TakingOrder:
                    HandleTakingOrder();
                    break;

                case WaiterTask.PickingUpDish:
                    HandlePickingUpDish();
                    break;

                case WaiterTask.DeliveringDish:
                    HandleDeliveringDish();
                    break;

                case WaiterTask.DiscardingDish:
                    HandleDiscardingDish();
                    break;

                case WaiterTask.CollectingPayment:
                    HandleCollectingPayment();
                    break;
            }

            ClearPendingTask();
            waiter?.SetIdleState();
        }

        // Decide table task
        private WaiterTask TryPrepareTableTask(Table table)
        {
            if (table == null) return WaiterTask.None;

            // Payment priority
            if (table.HasPendingPayout)
            {
                pendingTable = table;
                return WaiterTask.CollectingPayment;
            }

            // Delivery check
            if (CanDeliverToTable(table))
            {
                pendingTable = table;
                return WaiterTask.DeliveringDish;
            }

            // Order taking
            if (seatingService.TryGetGuestAtTable(table, out Guest guest) && guest.State == GuestState.WaitingForOrder)
            {
                pendingTable = table;
                return WaiterTask.TakingOrder;
            }

            // Reset
            pendingTable = null;

            return WaiterTask.None;
        }

        // Check if delivery is valid
        private bool CanDeliverToTable(Table table)
        {
            if (carriedOrder?.MenuItemSO == null) return false;
            if (!seatingService.TryGetGuestAtTable(table, out Guest seatedGuest) || seatedGuest == null) return false;
            if (seatedGuest.State != GuestState.WaitingForFood) return false;

            return orderedItemsByGuest.TryGetValue(seatedGuest, out MenuItemSO orderedItem) 
                && orderedItem == carriedOrder.MenuItemSO;
        }

        // Take order from guest
        private void HandleTakingOrder()
        {
            if (pendingTable == null) return;

            if (!seatingService.TryGetGuestAtTable(pendingTable, out Guest guest) || guest.State != GuestState.WaitingForOrder)
            {
                pendingTable = null;
                return;
            }

            MenuItemSO orderedItem = menuData != null ? menuData.GetRandomMenuItem() : null;

            if (orderedItem != null && orderManager != null)
            {
                orderManager.RegisterOrder(guest, pendingTable, orderedItem);
                orderedItemsByGuest[guest] = orderedItem;
                guest.ShowOrderThought(orderedItem.Sprite);
            }

            guest.SetState(GuestState.WaitingForFood);
            pendingTable = null;
        }

        // Pick dish from kitchen
        private void HandlePickingUpDish()
        {
            if (kitchen == null || carriedOrder != null) return;

            if (!kitchen.TryTakeReadyDish(out carriedOrder, out carriedDish))
            {
                carriedOrder = null;
                carriedDish = null;
                return;
            }

            if (carriedDish != null && waiter != null)
            {
                waiter.PlaceDishInHands(carriedDish.transform);
                waiter.SetState(WaiterState.Caring);
            }
        }

        // Deliver dish to table
        private void HandleDeliveringDish()
        {
            if (carriedOrder == null) return;
            if (pendingTable == null) return;

            if (!seatingService.TryGetGuestAtTable(pendingTable, out Guest seatedGuest) || seatedGuest == null) return;
            if (!CanDeliverToTable(pendingTable)) return;

            seatedGuest.HideOrderThought();
            seatedGuest.SetState(GuestState.Eating);

            // Add revenue
            pendingTable.AddDishRevenue(carriedOrder.MenuItemSO.Price * carriedOrder.Quantity);

            orderedItemsByGuest.Remove(seatedGuest);

            // Destroy visual
            if (carriedDish != null)
            {
                Object.Destroy(carriedDish);
                carriedDish = null;
            }

            pendingTable = null;
            carriedOrder = null;
        }

        // Collect payment from table
        private void HandleCollectingPayment()
        {
            if (pendingTable == null) return;

            int payout = pendingTable.CollectPendingPayout();

            if (payout > 0)
            {
                goalManager?.AddMoney(payout);
            }

            pendingTable = null;
        }

        // Discard carried dish
        private void HandleDiscardingDish()
        {
            if (carriedDish != null)
            {
                Object.Destroy(carriedDish);
                carriedDish = null;
            }

            pendingTable = null;
            carriedOrder = null;
        }

        // Reset current task data
        private void ClearPendingTask()
        {
            currentTask = WaiterTask.None;
            pendingTable = null;

            if (openedTrash != null)
            {
                openedTrash.SetOpened(false);
                openedTrash = null;
            }
        }
    }
}
