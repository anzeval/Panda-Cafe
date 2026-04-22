using UnityEngine;
using PandaCafe.Interaction;
using PandaCafe.AI;

namespace PandaCafe.NPC
{
    public class Guest : MonoBehaviour, IInteractable
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        // Number in the queue if =-1 => out of queue
        public int OrdinalQueueNumber {get; private set;}
        public InteractionType Type {get; private set;}

        private GuestState guestState;
        private NPCMovement movement;

        private void Awake()
        {
            Type = InteractionType.Guest;

            if (movement == null)
            {
                movement = GetComponent<NPCMovement>();

                if (movement == null)
                {
                    movement = gameObject.AddComponent<NPCMovement>();
                }
            }

            movement = GetComponent<NPCMovement>();
            movement.destinationReached += OnDestinationReached;
        }

        private void OnDestroy()
        {
            if (movement != null)
            {
                movement.destinationReached -= OnDestinationReached;
            }
        }

        private void LateUpdate()
        {
            spriteRenderer.sortingOrder = -(int)(transform.position.y * 100);
        }

        public bool TryGetWorldPoint(InteractionActor actor, out Vector3 point)
        {
            point = transform.position;
            return true;
        }

        public void SetState(GuestState guestState)
        {
            this.guestState = guestState;
        }

        public void MoveTo(Vector3 target) 
        {
            movement.SetTarget(target);
        }

        public void SetOrdinalQueueNumber(int index)
        {
            OrdinalQueueNumber = index;
        }

        private void OnDestinationReached()
        {
            if (guestState == GuestState.GoingToQueue)
            {
                guestState = GuestState.WaitingInQueue;
            }
            else if (guestState == GuestState.GoingToTable)
            {
                guestState = GuestState.ReadingMenu;
            }
            else if (guestState == GuestState.GoingToExit)
            {
                guestState = GuestState.Quit;
            }
        }
    }
}