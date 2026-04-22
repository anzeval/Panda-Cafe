using UnityEngine;

namespace PandaCafe.Interaction
{
    // Table behavior and interaction points
    public class Table : MonoBehaviour, IInteractable
    {
        [SerializeField] private Transform guestPosition; 
        [SerializeField] private Transform waiterPosition; 

        [SerializeField] SpriteRenderer spriteRenderer;

        private bool isTaken = false;

        public InteractionType Type {get; private set;}

        void Awake() 
        { 
            Type = InteractionType.Table; 
            spriteRenderer.sortingOrder = -(int)(transform.position.y * 100);
        } 

        // Get position by actor
        public bool TryGetWorldPoint(InteractionActor actor, out Vector3 point) 
        { 
            if(actor == InteractionActor.Guest) 
            { 
                if(isTaken) 
                { 
                    point = default;
                    return false;
                } 
                
                OccupyTable();
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

        // Set table free
        public void FreeTable()
        {
            isTaken = false;
        }

        // Set table occupied
        public void OccupyTable()
        {
            isTaken = true;
        }
    }
}