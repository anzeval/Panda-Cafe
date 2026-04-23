using UnityEngine;
using System;
using System.Collections.Generic;

namespace PandaCafe.AI
{
    public class NPCMovement : MonoBehaviour
    {
        [SerializeField] private float speed = 5f;
        [SerializeField] private float stopDistance = 0.05f;

        private static PathfindingManager pathfindingManager;

        private readonly Queue<Vector3> waypoints = new Queue<Vector3>();
        private Vector3 finalTarget;
        private bool isMoving;

        public event Action destinationReached;

        public static void Init(PathfindingManager manager)
        {
            pathfindingManager = manager;
        }

        public void SetTarget(Vector3 target)
        {
            finalTarget = target;
            RequestPath();
        }

         private void Update()
        {
            if (!isMoving) return;

            if (waypoints.Count == 0)
            {
                CompleteMovement();
                return;
            }

            Vector3 waypoint = waypoints.Peek();
            transform.position = Vector2.MoveTowards(transform.position, waypoint, speed * Time.deltaTime);

            if (Vector2.Distance(transform.position, waypoint) <= stopDistance)
            {
                transform.position = waypoint;
                waypoints.Dequeue();
            }
        }

        private void RequestPath()
        {
            waypoints.Clear();

            if (pathfindingManager == null)
            {
                Debug.LogWarning($"{name}: PathfindingManager is not initialized, movement request ignored.");
                isMoving = false;

                return;
            }

            List<Cell> path = pathfindingManager.FindPathfFromVector3(transform.position, finalTarget);

            if (path == null || path.Count == 0)
            {
                Debug.LogWarning($"{name}: No valid path found to target {finalTarget}.");
                isMoving = false;

                return;
            }

            for (int i = 1; i < path.Count; i++)
            {
                waypoints.Enqueue(path[i].WorldPosition);
            }
            isMoving = true;
        }

        private void CompleteMovement()
        {
            isMoving = false;
            destinationReached?.Invoke();
        }
    }
}