using UnityEngine;
using PandaCafe.NPC;

// Syncs guest state with animator.
// Reads state changes from Guest component.
// Updates animation parameters for movement and actions.
namespace PandaCafe.Animation
{
    public class GuestAnimation : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Guest guest;

        // Subscribe events
        private void OnEnable()
        {
            guest.StateChanged += HandleStateChange;
        }

        // Apply initial visual state
        private void Start()
        {
            HandleStateChange();
        }

        // Unsubscribe events
        void OnDisable()
        {
            guest.StateChanged -= HandleStateChange;
        }

        // Sync animator with state
        private void HandleStateChange()
        {
            switch (guest.State)
            {
                case GuestState.WaitingInQueue:
                    animator.SetTrigger("Idle");
                    break;

                case GuestState.GoingToQueue:
                case GuestState.GoingToTable:
                case GuestState.GoingToExit:
                    animator.SetTrigger("Walk");
                    break;

                case GuestState.ReadingMenu:
                    animator.SetTrigger("Reading");
                    break;

                case GuestState.WaitingForOrder:
                    animator.SetTrigger("CallingWaiter");
                    break;

                case GuestState.WaitingForFood:
                    animator.SetTrigger("Sitting");
                    break;

                case GuestState.Eating:
                    animator.SetTrigger("Eating");
                    break;
            }
        }
    } 
}

