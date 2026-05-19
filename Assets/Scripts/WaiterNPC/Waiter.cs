using UnityEngine;
using PandaCafe.AI;
using PandaCafe.Core.Shop;
using System;

// Controls waiter movement and state.
// Applies speed upgrades and carries dishes.
// Notifies listeners when destination is reached.
namespace PandaCafe.WaiterNPC
{
    public class Waiter : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        // Offset for carried dish
        [SerializeField] private Vector3 carriedDishLocalOffset = new Vector3(0f, 1f, 0f);

        private NPCMovement movement;

        // Notify when destination reached
        public event Action ArrivedAtDestination;

        public WaiterState State {get; private set;}

        public event Action StateChanged;

        // Initialize component references
        private void Awake()
        {
            State = WaiterState.Idle;

            // Ensure movement component
            if (movement == null)
            {
                movement = GetComponent<NPCMovement>();

                if (movement == null)
                {
                    movement = gameObject.AddComponent<NPCMovement>();
                }
            }

            // Subscribe to movement event
            movement.destinationReached += OnDestinationReached;

            ApplySpeedUpgrade();
            UpgradeManager.Instance.UpgradePurchased += OnUpgradePurchased;
        }

        // Change state and notify listeners
        public void SetState(WaiterState waiterState)
        {
            State = waiterState;
            StateChanged?.Invoke();
        }

        // Set walking state by carried dish
        public void UpdateMovementState(bool hasDishInHands)
        {
            SetState(hasDishInHands ? WaiterState.Caring : WaiterState.Walking);
        }

        // Set waiter idle state
        public void SetIdleState()
        {
            SetState(WaiterState.Idle);
        }

        // Update sprite sorting
        private void LateUpdate()
        {
            spriteRenderer.sortingOrder = -(int)(transform.position.y * 100);
        }

        // Clean up event subscriptions
        private void OnDestroy()
        {
            // Unsubscribe from event
            if (movement != null)
            {
                movement.destinationReached -= OnDestinationReached;
            }

            if (UpgradeManager.HasInstance)
            {
                UpgradeManager.Instance.UpgradePurchased -= OnUpgradePurchased;
            }
        }

        // React to purchased upgrade
        private void OnUpgradePurchased(ShopUpgradeSO upgrade)
        {
            if (upgrade == null || upgrade.type != UpgradeType.WaiterSpeed) return;

            ApplySpeedUpgrade();
        }

        // Apply waiter speed upgrade
        private void ApplySpeedUpgrade()
        {
            if (movement == null) return;

            movement.SetSpeedMultiplier(UpgradeManager.Instance.GetMultiplier(UpgradeType.WaiterSpeed));
        }

        // Handle arrival
        private void OnDestinationReached()
        {
            ArrivedAtDestination?.Invoke();
        }

        // Move to target
        public bool MoveTo(Vector3 target)
        {
            return movement.SetTarget(target);
        }

        // Attach dish to hands
        public void PlaceDishInHands(Transform dishTransform)
        {
            if (dishTransform == null) return;

            dishTransform.SetParent(transform, false);
            dishTransform.localPosition = carriedDishLocalOffset;
        }
    }
}