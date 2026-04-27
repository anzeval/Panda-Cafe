using UnityEngine;
using PandaCafe.AI;
using System;

namespace PandaCafe.WaiterNPC
{
    public class Waiter : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
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
    }
}

