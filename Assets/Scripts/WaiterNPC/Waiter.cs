using UnityEngine;
using PandaCafe.AI;
using System;

namespace PandaCafe.WaiterNPC
{
    // Controls waiter movement and actions
    public class Waiter : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        // Offset for carried dish
        [SerializeField] private Vector3 carriedDishLocalOffset = new Vector3(0f, 1f, 0f);

        private NPCMovement movement;

        // Notify when destination reached
        public event Action ArrivedAtDestination;

        private void Awake()
        {
            // Ensure movement component
            if (movement == null)
            {
                movement = GetComponent<NPCMovement>();

                if (movement == null)
                {
                    movement = gameObject.AddComponent<NPCMovement>();
                }
            }

            // Subscribe to movement event
            movement.destinationReached += OnDestinationReached;
        }

        // Update sprite sorting
        private void LateUpdate()
        {
            spriteRenderer.sortingOrder = -(int)(transform.position.y * 100);
        }

        private void OnDestroy()
        {
            // Unsubscribe from event
            if (movement != null)
            {
                movement.destinationReached -= OnDestinationReached;
            }
        }

        // Handle arrival
        private void OnDestinationReached()
        {
            ArrivedAtDestination?.Invoke();
        }

        // Move to target
        public bool MoveTo(Vector3 target)
        {
            return movement.SetTarget(target);
        }

        // Attach dish to hands
        public void PlaceDishInHands(Transform dishTransform)
        {
            if (dishTransform == null) return;

            dishTransform.SetParent(transform, false);
            dishTransform.localPosition = carriedDishLocalOffset;
        }
    }
}