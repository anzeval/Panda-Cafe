using UnityEngine;

namespace PandaCafe.Interaction
{
    public class Trash : MonoBehaviour, IInteractable
    {
        [SerializeField] private Transform waiterPosition;

        public InteractionType Type {get; private set;}

        void Awake()
        {
            Type = InteractionType.Trash;
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
    }
}