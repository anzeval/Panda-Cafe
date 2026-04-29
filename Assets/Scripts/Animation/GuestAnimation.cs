using UnityEngine;
using PandaCafe.NPC;

namespace PandaCafe.Animation
{
    public class GuestAnimation : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Guest guest;

        private void OnEnable()
        {
            guest.StateChanged += HandleStateChange;
        }

        private void Start()
        {
            HandleStateChange();
        }

        void OnDisable()
        {
            guest.StateChanged -= HandleStateChange;
        }

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

