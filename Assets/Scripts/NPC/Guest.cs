using UnityEngine;
using PandaCafe.Managers;
using PandaCafe.Interaction;

namespace PandaCafe.NPC
{
    public class Guest : MonoBehaviour, IInteractable
    {
        [SerializeField] private float speed = 5f;
        [SerializeField] private SpriteRenderer spriteRenderer;

        // Number in the queue if =-1 => out of queue
        public int OrdinalQueueNumber {get; private set;}

        private GuestState guestState;
        public InteractionType Type {get; private set;}

        private Vector3 targetPos;
        private bool isMoving = false;

        private void Awake()
        {
            Type = InteractionType.Guest;
        }

        private void Update()
        {
            if(!isMoving) return;

            HandleMovement();
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

        private void HandleMovement()
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

            if (Vector2.Distance(transform.position, targetPos) < 0.05f)
            {
                transform.position = targetPos;
                isMoving = false;

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

        public void SetState(GuestState guestState)
        {
            this. guestState = guestState;
        }

        public void MoveTo(Vector3 target) 
        {
            targetPos = target;
            isMoving = true;
        }

        public void SetOrdinalQueueNumber(int index)
        {
            OrdinalQueueNumber = index;
        }
    }
}