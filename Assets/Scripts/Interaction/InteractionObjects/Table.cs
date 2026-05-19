using PandaCafe.NPC;
using UnityEngine;
using PandaCafe.Interaction;
using PandaCafe.Environment;
using PandaCafe.Core.Shop;

// Represents guest table interaction.
// Stores seated guest and pending payout.
// Handles revenue, tips, and coin visibility.
namespace PandaCafe.Interaction.InteractionObjects
{
    public class Table : MonoBehaviour, IInteractable
    {
        [SerializeField] private Transform guestPosition; 
        [SerializeField] private Transform waiterPosition; 

        [SerializeField] SpriteRenderer spriteRenderer;

        [SerializeField] private Coin coinPrefab;
        [SerializeField] private Transform coinSpawnPoint;

        // Current seated guest
        public Guest CurrentGuest { get; private set; }

        // Table occupied state
        public bool IsTaken => isTaken;

        // Has money to collect
        public bool HasPendingPayout => isPayoutReady && PendingPayout > 0;

        // Total pending money
        public int PendingPayout => pendingDishRevenue + pendingTips;

        public InteractionType Type {get; private set;}

        private bool isTaken = false;
        private bool isPayoutReady = false;

        // Accumulated revenue
        private int pendingDishRevenue;
        private int pendingTips;

        private const int CoinSortingOffset = 100;

        private Coin activeCoin;

        // Initialize component references
        void Awake() 
        { 
            Type = InteractionType.Table;

            // Set render order
            spriteRenderer.sortingOrder = -(int)(transform.position.y * 100);
        } 

        // Return interaction point by actor
        public bool TryGetWorldPoint(InteractionActor actor, out Vector3 point) 
        { 
            if (actor == InteractionActor.Guest) 
            { 
                // Block if occupied or not cleared
                if (isTaken || HasPendingPayout)
                { 
                    point = default;
                    return false;
                } 
                
                point = guestPosition.position; 
                return true; 
            } 
            
            if (actor == InteractionActor.Waiter) 
            { 
                point = waiterPosition.position; 
                return true; 
            } 
            
            point = default;
            return false; 
        }

        // Free table
        public void FreeTable()
        {
            isTaken = false;
            CurrentGuest = null;
        }

        // Assign guest
        public void OccupyTable(Guest guest)
        {
            isTaken = true;
            CurrentGuest = guest;
        }

        // Add dish income
        public void AddDishRevenue(int amount)
        {
            if (amount <= 0) return;
            pendingDishRevenue += ApplyIncomeMultiplier(amount);
        }

        // Add tips
        public void AddTips(int amount)
        {
            if (amount > 0)
            {
                pendingTips += ApplyIncomeMultiplier(amount);
            }

            isPayoutReady = PendingPayout > 0;

            if (isPayoutReady)
            {
                EnsureCoinVisible();
            }
        }

        // Collect all money
        public int CollectPendingPayout()
        {
            int payout = PendingPayout;

            pendingDishRevenue = 0;
            pendingTips = 0;
            isPayoutReady = false;

            if (activeCoin != null)
            {
                activeCoin.Collect();
                activeCoin = null;
            }

            return payout;
        }

        // Apply income upgrade multiplier
        private int ApplyIncomeMultiplier(int amount)
        {
            float multiplier = UpgradeManager.Instance.GetMultiplier(UpgradeType.IncomeMultiplier);
            return Mathf.Max(0, Mathf.RoundToInt(amount * multiplier));
        }

        // Spawn payout coin if needed
        private void EnsureCoinVisible()
        {
            if (activeCoin != null || coinPrefab == null) return;

            Transform spawnPoint = coinSpawnPoint != null ? coinSpawnPoint : transform;
            activeCoin = Instantiate(coinPrefab, spawnPoint, false);
            activeCoin.transform.localPosition = Vector3.zero;
            activeCoin.SetSortingOrder(spriteRenderer.sortingOrder + CoinSortingOffset);
        }
    }
}
