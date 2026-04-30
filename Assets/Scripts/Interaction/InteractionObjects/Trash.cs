using PandaCafe.Animation;
using UnityEngine;
using System;

namespace PandaCafe.Interaction
{
    // Trash interaction point
    public class Trash : MonoBehaviour, IInteractable
    {
        // Position for waiter interaction
        [SerializeField] private Transform waiterPosition;

        public TrashState State {get; private set;}
        public InteractionType Type {get; private set;}

        public event Action StateChanged;

        void Awake()
        {
            Type = InteractionType.Trash;
            SetState(TrashState.Closed);
        }

        public void SetOpened(bool opened)
        {
            SetState(opened ? TrashState.Opening : TrashState.Closing);
        }

        public void SetState(TrashState newState)
        {
            if (State == newState) return;

            State = newState;
            StateChanged?.Invoke();
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