using UnityEngine;
using PandaCafe.Interaction;
using PandaCafe.AI;
using System;

namespace PandaCafe.NPC
{
    public class Guest : MonoBehaviour, IInteractable
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        // Number in the queue if =-1 => out of queue
        public int OrdinalQueueNumber {get; private set;}

        public InteractionType Type {get; private set;}

        public GuestState State => guestState;

        private GuestSO guestSO;
        private GuestState guestState;
        private NPCMovement movement;

        private const float tableArrivalTolerance = 0.01f;
        private bool hasPendingTableTarget;
        private Vector3 pendingTableTarget;

        private float stateTimer;
        private Transform quitPoint;

        public event Action<Guest> PatienceExpired;

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

        public void Init(GuestSO _guestSO, Transform _quitPoint)
        {
            guestSO = _guestSO;
            quitPoint = _quitPoint;
        }

        private void Update()
        {
            HandleState();
        }

        private void LateUpdate()
        {
            spriteRenderer.sortingOrder = -(int)(transform.position.y * 100) + 20;
        }

        private void OnDestroy()
        {
            if (movement != null)
            {
                movement.destinationReached -= OnDestinationReached;
            }
        }

        private void HandleState()
        {
            switch (guestState)
            {
                case GuestState.WaitingInQueue : 
                case GuestState.WaitingForOrder : 
                case GuestState.WaitingForFood : 
                case GuestState.Eating : 
                    if (TickTimer())
                        HandlePatienceExpired();
                        
                    break;
                case GuestState.ReadingMenu : 
                    if (TickTimer())
                        SetState(GuestState.WaitingForOrder);
                    break;
            }
        }

         private bool TickTimer()
        {
            stateTimer -= Time.deltaTime;
            return stateTimer <= 0f;
        }

        private void HandlePatienceExpired()
        {
            PatienceExpired?.Invoke(this);
            SetState(GuestState.GoingToExit);
        }
        public bool TryGetWorldPoint(InteractionActor actor, out Vector3 point)
        {
            point = transform.position;
            return true;
        }

        public void SetState(GuestState guestState)
        {
            this.guestState = guestState;

            switch (guestState)
            {
                case GuestState.WaitingInQueue:
                    stateTimer = guestSO.WaitInQueueTime;
                    break;

                case GuestState.ReadingMenu:
                    stateTimer = guestSO.ReadingMenuTime;
                    break;

                case GuestState.WaitingForOrder:
                    stateTimer = guestSO.WaitingOrderTime;
                    Debug.Log("wait for order");
                    break;

                case GuestState.WaitingForFood:
                    stateTimer = guestSO.WaitingFoodTime;
                    break;

                case GuestState.Eating:
                    stateTimer = guestSO.EatingTime;
                    Debug.Log("eating");
                    break;
                case GuestState.GoingToExit:
                    MoveTo(quitPoint.position);
                    Debug.Log("quit");
                    break;
            }
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
                Destroy(gameObject);
            } 
        }
    }
}