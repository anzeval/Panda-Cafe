using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PandaCafe.Menu;

namespace PandaCafe.Interaction
{
    // Handles cooking and dish serving
    public class Kitchen : MonoBehaviour, IInteractable
    {
        // Waiter interaction point
        [SerializeField] private Transform waiterPosition;

        // Dish spawn points
        [SerializeField] private Transform[] servingPoints = new Transform[4];

        // Cooking duration
        [SerializeField] private float cookingTime = 5f;

        // Orders in cooking
        private readonly Queue<OrderItem> cookingQueue = new Queue<OrderItem>();

        // Orders ready but not placed
        private readonly Queue<OrderItem> readyQueue = new Queue<OrderItem>();

        // Order to spawned dish
        private readonly Dictionary<OrderItem, GameObject> dishByOrder = new Dictionary<OrderItem, GameObject>();

        // Serving points occupancy
        private readonly bool[] occupiedServingPoints = new bool[4];

        private OrderManager orderManager;
        private Coroutine cookingRoutine;

        public InteractionType Type { get; private set; }

        void Awake()
        {
            Type = InteractionType.Kitchen;
        }

        // Initialize with order manager
        public void Init(OrderManager orderManager)
        {
            this.orderManager = orderManager;

            // Subscribe to new orders
            orderManager.OrderRegistered += EnqueueOrder;
        }

        private void OnDisable()
        {
            // Unsubscribe from orders
            if (orderManager != null)
            {
                orderManager.OrderRegistered -= EnqueueOrder;
            }
        }

        // Return waiter interaction point
        public bool TryGetWorldPoint(InteractionActor actor, out Vector3 point)
        {
            if (actor != InteractionActor.Waiter)
            {
                point = default;
                return false;
            }

            point = waiterPosition != null ? waiterPosition.position : transform.position;
            return true;
        }

        // Add order to cooking queue
        private void EnqueueOrder(OrderItem order)
        {
            if (order == null) return;

            cookingQueue.Enqueue(order);

            // Start processing if idle
            if (cookingRoutine == null)
            {
                cookingRoutine = StartCoroutine(ProcessOrders());
            }
        }

        // Process cooking queue
        private IEnumerator ProcessOrders()
        {
            while (cookingQueue.Count > 0)
            {
                OrderItem order = cookingQueue.Dequeue();

                yield return new WaitForSeconds(cookingTime);

                readyQueue.Enqueue(order);
                TryPlaceReadyOrders();
            }

            cookingRoutine = null;
        }

        // Place ready dishes on serving points
        private void TryPlaceReadyOrders()
        {
            while (readyQueue.Count > 0)
            {
                int freePointIndex = GetFreeServingPointIndex();

                if (freePointIndex < 0) return;

                OrderItem order = readyQueue.Dequeue();
                SpawnDish(order, freePointIndex);
            }
        }

        // Find free serving point
        private int GetFreeServingPointIndex()
        {
            int pointsCount = Mathf.Min(servingPoints.Length, occupiedServingPoints.Length);

            for (int i = 0; i < pointsCount; i++)
            {
                if (!occupiedServingPoints[i])
                {
                    return i;
                }
            }

            return -1;
        }

        // Spawn dish prefab
        private void SpawnDish(OrderItem order, int pointIndex)
        {
            if (order?.MenuItemSO == null || order.MenuItemSO.Prefab == null) return;
            if (pointIndex < 0 || pointIndex >= servingPoints.Length) return;
            
            Transform spawnPoint = servingPoints[pointIndex];
            if (spawnPoint == null) return;

            GameObject dish = Instantiate(order.MenuItemSO.Prefab, spawnPoint.position, Quaternion.identity, spawnPoint);

            // Force render on top
            dish.GetComponent<SpriteRenderer>().sortingOrder = 1000;

            dishByOrder[order] = dish;
            occupiedServingPoints[pointIndex] = true;

            // Mark order completed
            orderManager?.CompleteOrder(order);
        }

        // Take first ready dish
        public bool TryTakeReadyDish(out OrderItem order, out GameObject dish)
        {
            OrderItem firstReadyOrder = null;

            // Get any ready order
            foreach (OrderItem readyOrder in dishByOrder.Keys)
            {
                firstReadyOrder = readyOrder;
                break;
            }

            if (firstReadyOrder == null)
            {
                order = null;
                dish = null;
                return false;
            }

            order = firstReadyOrder;
            dish = dishByOrder[firstReadyOrder];

            // Free serving point
            FreeServingPoint(dish != null ? dish.transform.parent : null);

            dishByOrder.Remove(firstReadyOrder);

            // Try place next dishes
            TryPlaceReadyOrders();

            return true;
        }

        // Free serving point by transform
        private void FreeServingPoint(Transform point)
        {
            if (point == null) return;

            int pointsCount = Mathf.Min(servingPoints.Length, occupiedServingPoints.Length);

            for (int i = 0; i < pointsCount; i++)
            {
                if (servingPoints[i] != point) continue;

                occupiedServingPoints[i] = false;
                return;
            }
        }
    }
}