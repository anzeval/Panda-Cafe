using UnityEngine;
using PandaCafe.AI;
using System;

namespace PandaCafe.WaiterNPC
{
    public class Waiter : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Vector3 carriedDishLocalOffset = new Vector3(0f, 1f, 0f);
        private NPCMovement movement;

        public event Action ArrivedAtDestination;

        private void Awake()
        {
            if (movement == null)
            {
                movement = GetComponent<NPCMovement>();

                if (movement == null)
                {
                    movement = gameObject.AddComponent<NPCMovement>();
                }
            }

            movement.destinationReached += OnDestinationReached;
        }

        private void LateUpdate()
        {
            spriteRenderer.sortingOrder = -(int)(transform.position.y * 100);
        }

        private void OnDestroy()
        {
            if (movement != null)
            {
                movement.destinationReached -= OnDestinationReached;
            }
        }

        private void OnDestinationReached()
        {
            ArrivedAtDestination?.Invoke();
        }

        public bool MoveTo(Vector3 target)
        {
            return movement.SetTarget(target);
        }

        public void PlaceDishInHands(Transform dishTransform)
        {
            if (dishTransform == null) return;

            dishTransform.SetParent(transform, false);
            dishTransform.localPosition = carriedDishLocalOffset;
        }
    }
}

