using PandaCafe.NPC;
using UnityEngine;

namespace PandaCafe.Interaction
{
    // Table with guest and waiter interaction
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
        public bool HasPendingPayout => PendingPayout > 0;

        // Total pending money
        public int PendingPayout => pendingDishRevenue + pendingTips;

        public InteractionType Type {get; private set;}

        private bool isTaken = false;

        // Accumulated revenue
        private int pendingDishRevenue;
        private int pendingTips;

        private Coin activeCoin;

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
            pendingDishRevenue += amount;
            EnsureCoinVisible();
        }

        // Add tips
        public void AddTips(int amount)
        {
            if (amount <= 0) return;
            pendingTips += amount;
            EnsureCoinVisible();
        }

        // Collect all money
        public int CollectPendingPayout()
        {
            int payout = PendingPayout;

            pendingDishRevenue = 0;
            pendingTips = 0;

            if (activeCoin != null)
            {
                activeCoin.Collect();
                activeCoin = null;
            }

            return payout;
        }

        private void EnsureCoinVisible()
        {
            if (activeCoin != null || coinPrefab == null) return;

            Transform spawnPoint = coinSpawnPoint != null ? coinSpawnPoint : transform;
            activeCoin = Instantiate(coinPrefab, spawnPoint.position, Quaternion.identity, transform);
        }
    }
}