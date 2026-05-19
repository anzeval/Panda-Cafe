using UnityEngine;
using PandaCafe.WaiterNPC;

// Syncs waiter state with animator.
// Reads state changes from Waiter component.
// Updates animation parameters for waiter actions.
namespace PandaCafe.Animation
{
    public class WaiterAnimation : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Waiter waiter;

        // Subscribe events
        private void OnEnable()
        {
            waiter.StateChanged += HandleStateChange;
        }

        // Apply initial visual state
        private void Start()
        {
            HandleStateChange();
        }

        // Unsubscribe events
        void OnDisable()
        {
            waiter.StateChanged -= HandleStateChange;
        }

        // Sync animator with state
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