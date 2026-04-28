using UnityEngine;

namespace PandaCafe.Interaction
{
    // Trash interaction point
    public class Trash : MonoBehaviour, IInteractable
    {
        // Position for waiter interaction
        [SerializeField] private Transform waiterPosition;

        public InteractionType Type {get; private set;}

        void Awake()
        {
            Type = InteractionType.Trash;
        }

        // Return interaction point for waiter
        public bool TryGetWorldPoint(InteractionActor actor, out Vector3 point)
        {
            // Only waiter can interact
            if (actor != InteractionActor.Waiter)
            {
                point = default;
                return false;
            }

            // Use custom position or fallback
            point = waiterPosition != null ? waiterPosition.position : transform.position;
            return true;
        }
    }
}