using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PandaCafe.Menu;
using Unity.VisualScripting;

namespace PandaCafe.Interaction
{
    public class Kitchen : MonoBehaviour, IInteractable
    {
        [SerializeField] private Transform waiterPosition;
        [SerializeField] private Transform[] servingPoints = new Transform[4];
        [SerializeField] private float cookingTime = 5f;

        private readonly Queue<OrderItem> cookingQueue = new Queue<OrderItem>();
        private readonly Queue<OrderItem> readyQueue = new Queue<OrderItem>();
        private readonly Dictionary<OrderItem, GameObject> dishByOrder = new Dictionary<OrderItem, GameObject>();
        private readonly bool[] occupiedServingPoints = new bool[4];

        private OrderManager orderManager;
        private Coroutine cookingRoutine;

        public InteractionType Type { get; private set; }

        void Awake()
        {
            Type = InteractionType.Kitchen;
        }

        public void Init(OrderManager orderManager)
        {
            this.orderManager = orderManager;
            orderManager.OrderRegistered += EnqueueOrder;
        }

        private void OnDisable()
        {
            if (orderManager != null)
            {
                orderManager.OrderRegistered -= EnqueueOrder;
            }
        }

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

        private void EnqueueOrder(OrderItem order)
        {
            if (order == null) return;

            cookingQueue.Enqueue(order);

            if (cookingRoutine == null)
            {
                cookingRoutine = StartCoroutine(ProcessOrders());
            }
        }

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

        private void SpawnDish(OrderItem order, int pointIndex)
        {
            if (order?.MenuItemSO == null || order.MenuItemSO.Prefab == null || pointIndex < 0 || pointIndex >= servingPoints.Length) return;
            
            Transform spawnPoint = servingPoints[pointIndex];

            if (spawnPoint == null) return;

            GameObject dish = Instantiate(order.MenuItemSO.Prefab, spawnPoint.position, Quaternion.identity, spawnPoint);
            dish.GetComponent<SpriteRenderer>().sortingOrder = 1000;

            dishByOrder[order] = dish;
            occupiedServingPoints[pointIndex] = true;
            orderManager?.CompleteOrder(order);
        }

        public bool TryTakeReadyDish(out OrderItem order, out GameObject dish)
        {
            OrderItem firstReadyOrder = null;
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
            FreeServingPoint(dish != null ? dish.transform.parent : null);
            dishByOrder.Remove(firstReadyOrder);
            TryPlaceReadyOrders();
            return true;
        }

        private void FreeServingPoint(Transform point)
        {
            if (point == null) return;

            int pointsCount = Mathf.Min(servingPoints.Length, occupiedServingPoints.Length);
            for (int i = 0; i < pointsCount; i++)
            {
                if (servingPoints[i] != point)
                    continue;

                occupiedServingPoints[i] = false;
                return;
            }
        }
    }
}