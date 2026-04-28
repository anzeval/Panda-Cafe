using PandaCafe.NPC;
using UnityEngine;

namespace PandaCafe.Interaction
{
    // Represents a table with interaction points for guest and waiter.
    // Manages table occupancy and provides positions for interactions.
    public class Table : MonoBehaviour, IInteractable
    {
        [SerializeField] private Transform guestPosition; 
        [SerializeField] private Transform waiterPosition; 

        [SerializeField] SpriteRenderer spriteRenderer;

        public Guest CurrentGuest { get; private set; }
        public bool IsTaken => isTaken;
        public bool HasPendingPayout => PendingPayout > 0;
        public int PendingPayout => pendingDishRevenue + pendingTips;

        public InteractionType Type {get; private set;}

        private bool isTaken = false;
        private int pendingDishRevenue;
        private int pendingTips;

        // Initializes table type and sets correct render order
        void Awake() 
        { 
            Type = InteractionType.Table; 
            spriteRenderer.sortingOrder = -(int)(transform.position.y * 100);
        } 

        // Returns interaction point based on actor (guest/waiter)
        public bool TryGetWorldPoint(InteractionActor actor, out Vector3 point) 
        { 
            if(actor == InteractionActor.Guest) 
            { 
                // Prevent guest if table is occupied
                if(isTaken || HasPendingPayout)
                { 
                    point = default;
                    return false;
                } 
                
                point = guestPosition.position; 
                return true; 
            } 
            
            if(actor == InteractionActor.Waiter) 
            { 
                point = waiterPosition.position; 
                return true; 
            } 
            
            point = default;
            return false; 
        }

        // Marks table as free
        public void FreeTable()
        {
            isTaken = false;
            CurrentGuest = null;
        }

        // Marks table as occupied
        public void OccupyTable(Guest guest)
        {
            isTaken = true;
            CurrentGuest = guest;
        }

        public void AddDishRevenue(int amount)
        {
            if (amount <= 0) return;
            pendingDishRevenue += amount;
        }

        public void AddTips(int amount)
        {
            if (amount <= 0) return;
            pendingTips += amount;
        }

        public int CollectPendingPayout()
        {
            int payout = PendingPayout;
            pendingDishRevenue = 0;
            pendingTips = 0;

            return payout;
        }
    }
}