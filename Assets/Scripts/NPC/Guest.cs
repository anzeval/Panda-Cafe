using UnityEngine;
using PandaCafe.Interaction;
using PandaCafe.AI;
using System;

namespace PandaCafe.NPC
{
    public class Guest : MonoBehaviour, IInteractable
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        // Queue position, -1 means not in queue
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

        // Events for external systems
        public event Action<Guest> PatienceExpired;
        public event Action<Guest, int> MealCompleted;
        public event Action<GuestState, GuestState> StateChanged;
        public event Action<Guest> GuestRemoved;

        private void Awake()
        {
            Type = InteractionType.Guest;

            // Ensure movement component exists
            if (movement == null)
            {
                movement = GetComponent<NPCMovement>();

                if (movement == null)
                {
                    movement = gameObject.AddComponent<NPCMovement>();
                }
            }

            movement = GetComponent<NPCMovement>();

            // Subscribe to movement event
            movement.destinationReached += OnDestinationReached;
        }

        // Initialize data
        public void Init(GuestSO _guestSO, Transform _quitPoint)
        {
            guestSO = _guestSO;
            quitPoint = _quitPoint;
        }

        private void Update()
        {
            HandleState();
        }

        // Update sprite sorting by Y position
        private void LateUpdate()
        {
            spriteRenderer.sortingOrder = -(int)(transform.position.y * 100) + 20;
        }

        private void OnDestroy()
        {
            // Unsubscribe from movement event
            if (movement != null)
            {
                movement.destinationReached -= OnDestinationReached;
            }
        }

        // Main state logic
        private void HandleState()
        {
            switch (guestState)
            {
                case GuestState.WaitingInQueue : 
                case GuestState.WaitingForOrder : 
                case GuestState.WaitingForFood : 
                    if (TickTimer())
                        HandlePatienceExpired();
                    break;

                case GuestState.ReadingMenu : 
                    if (TickTimer())
                        SetState(GuestState.WaitingForOrder);
                    break;

                case GuestState.Eating :
                    if (TickTimer())
                        HandleMealCompleted();
                    break;
            }
        }

        // Update timer and check if finished
        private bool TickTimer()
        {
            stateTimer -= Time.deltaTime;
            return stateTimer <= 0f;
        }

        // Trigger patience expiration
        private void HandlePatienceExpired()
        {
            PatienceExpired?.Invoke(this);
            SetState(GuestState.GoingToExit);
        }

        // Trigger meal completion and calculate tips
        private void HandleMealCompleted()
        {
            int minTips = Mathf.RoundToInt(Mathf.Min(guestSO.MinTips, guestSO.MaxTips));
            int maxTips = Mathf.RoundToInt(Mathf.Max(guestSO.MinTips, guestSO.MaxTips));
            int tips = UnityEngine.Random.Range(minTips, maxTips + 1);

            MealCompleted?.Invoke(this, Mathf.Max(0, tips));
            SetState(GuestState.GoingToExit);
        }
        
        // Return interaction point
        public bool TryGetWorldPoint(InteractionActor actor, out Vector3 point)
        {
            point = transform.position;
            return true;
        }

        // Change state and notify listeners
        public void SetState(GuestState guestState)
        {
            StateChanged?.Invoke(this.guestState, guestState);
            this.guestState = guestState;
            
            SetStateTimer();
        }

        // Setup timer or actions for new state
        private void SetStateTimer()
        {
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
                    break;

                case GuestState.WaitingForFood:
                    stateTimer = guestSO.WaitingFoodTime;
                    break;

                case GuestState.Eating:
                    stateTimer = guestSO.EatingTime;
                    break;

                case GuestState.GoingToExit:
                    MoveTo(quitPoint.position);
                    break;
            }
        }

        // Move to position
        public bool MoveTo(Vector3 target) 
        {
            hasPendingTableTarget = false;
            return movement.SetTarget(target);
        }

        // Move to table with validation
        public bool MoveToTable(Vector3 tableTarget)
        {
            hasPendingTableTarget = true;
            pendingTableTarget = tableTarget;
            return movement.SetTarget(tableTarget);
        }

        // Set queue index
        public void SetOrdinalQueueNumber(int index)
        {
            OrdinalQueueNumber = index;
        }

        // Handle movement completion
        private void OnDestinationReached()
        {
            if (guestState == GuestState.GoingToQueue)
            {
                SetState(GuestState.WaitingInQueue);
            }
            else if (guestState == GuestState.GoingToTable)
            {
                // Ensure correct table position reached
                if (hasPendingTableTarget && Vector2.Distance(transform.position, pendingTableTarget) > tableArrivalTolerance) return;

                hasPendingTableTarget = false;
                SetState(GuestState.ReadingMenu);
            }
            else if (guestState == GuestState.GoingToExit)
            {
                // Notify removal and destroy object
                GuestRemoved?.Invoke(this);
                Destroy(gameObject);
            } 
        }
    }
}