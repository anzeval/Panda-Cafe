using UnityEngine;
using System;
using System.Collections.Generic;

namespace PandaCafe.AI
{
    // Moves NPC along a path to a target using waypoints.
    // Requests path, follows it step-by-step, and signals when destination is reached.
    public class NPCMovement : MonoBehaviour
    {
        [SerializeField] private float speed = 5f;
        [SerializeField] private float stopDistance = 0.05f;

        private static PathfindingManager pathfindingManager;

        // Path points queue    
        private readonly Queue<Vector3> waypoints = new Queue<Vector3>();
        private Vector3 finalTarget;
        private bool isMoving;

        // Fired when destination is reached
        public event Action destinationReached;

        public static void Init(PathfindingManager manager)
        {
            pathfindingManager = manager;
        }

        // Sets destination and tries to build a path
        public bool SetTarget(Vector3 target)
        {
            finalTarget = target;
            return RequestPath();
        }

        // Moves along waypoints each frame until destination is reached
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

        // Requests path from pathfinding system and fills waypoint queue
        private bool RequestPath()
        {
            waypoints.Clear();

            if (pathfindingManager == null)
            {
                isMoving = false;
                return false;
            }

            List<Cell> path = pathfindingManager.FindPathfFromVector3(transform.position, finalTarget);

            if (path == null || path.Count == 0)
            {
                isMoving = false;
                return false;
            }

            for (int i = 1; i < path.Count; i++)
            {
                waypoints.Enqueue(path[i].WorldPosition);
            }
            
            waypoints.Enqueue(finalTarget);

            isMoving = true;
            return true;
        }

        // Stops movement and triggers completion event
        private void CompleteMovement()
        {
            isMoving = false;
            destinationReached?.Invoke();
        }
    }
}