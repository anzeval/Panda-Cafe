using UnityEngine;

namespace PandaCafe.WaiterNPC
{
    public class WaiterAnimation : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Waiter waiter;

        private void OnEnable()
        {
            waiter.StateChanged += HandleStateChange;
        }

        private void Start()
        {
            HandleStateChange();
        }

        void OnDisable()
        {
            waiter.StateChanged -= HandleStateChange;
        }

        private void HandleStateChange()
        {
            switch (waiter.State)
            {
                case WaiterState.Idle:
                    animator.SetTrigger("Idle");
                    break;

                case WaiterState.Walking:
                    animator.SetTrigger("Walking");
                    break;

                case WaiterState.Caring:
                    animator.SetTrigger("Carrying");
                    break;
            }
        }
    }
}