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

        public InteractionType Type {get; private set;}

        private bool isTaken = false;
        private Guest tableGuest = null;

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
                if(isTaken) 
                { 
                    point = default;
                    return false;
                } 
                
                OccupyTable();// Mark as taken
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
        }

        // Marks table as occupied
        public void OccupyTable()
        {
            isTaken = true;
        }
    }
}