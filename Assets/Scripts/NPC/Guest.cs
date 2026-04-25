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

        private GuestSO guestSO;
        private GuestState guestState;
        private NPCMovement movement;

        private const float tableArrivalTolerance = 0.01f;
        private bool hasPendingTableTarget;
        private Vector3 pendingTableTarget;

        private float menuReadingTimer;

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

        public void Init(GuestSO _guestSO)
        {
            guestSO = _guestSO;
            menuReadingTimer = guestSO.ReadingMenuTime;
        }

        private void Update()
        {
            HandleMenuReading();
        }

        private void HandleMenuReading()
        {
            if (guestState != GuestState.ReadingMenu) return;

            menuReadingTimer -= Time.deltaTime;

            if (menuReadingTimer <= 0f)
            {
                SetState(GuestState.WaitingForOrder);
            }
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
            spriteRenderer.sortingOrder = -(int)(transform.position.y * 100) + 20;
        }

        public bool TryGetWorldPoint(InteractionActor actor, out Vector3 point)
        {
            point = transform.position;
            return true;
        }

        public void SetState(GuestState guestState)
        {
            Debug.Log("set state " + guestState);
            this.guestState = guestState;
        }

        public bool MoveTo(Vector3 target) 
        {
            hasPendingTableTarget = false;
            return movement.SetTarget(target);
        }

        public bool MoveToTable(Vector3 tableTarget)
        {
            hasPendingTableTarget = true;
            pendingTableTarget = tableTarget;
            return movement.SetTarget(tableTarget);
        }

        public void SetOrdinalQueueNumber(int index)
        {
            OrdinalQueueNumber = index;
        }

        private void OnDestinationReached()
        {
            if (guestState == GuestState.GoingToQueue)
            {
                SetState(GuestState.WaitingInQueue);
            }
            else if (guestState == GuestState.GoingToTable)
            {
                if (hasPendingTableTarget && Vector2.Distance(transform.position, pendingTableTarget) > tableArrivalTolerance) return;

                hasPendingTableTarget = false;
                SetState(GuestState.ReadingMenu);
            }
            else if (guestState == GuestState.GoingToExit)
            {
                SetState(GuestState.Quit);
            } 
        }
    }
}