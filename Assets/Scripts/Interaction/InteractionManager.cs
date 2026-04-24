using PandaCafe.NPC;
using PandaCafe.Managers;
using PandaCafe.Input;
using UnityEngine;

namespace PandaCafe.Interaction
{
    // Routes interactions from input to game logic (waiter or guest).
    // Selects guest or triggers interaction request based on clicked object.
    public class InteractionManager : MonoBehaviour
    {
        InputHandler inputHandler;
        HallManager hallManager;
        Guest selectedGuest;

        // Initializes dependencies and subscribes to input events
        public void Init(InputHandler inputHandler, HallManager hallManager)
        {
            this.inputHandler = inputHandler;
            this.hallManager = hallManager;

            inputHandler.interacted += ChooseObjectForInteraction;
        }

        // Unsubscribes from input events
        void OnDisable()
        {
            inputHandler.interacted -= ChooseObjectForInteraction;
        }

        // Determines interaction type and routes request to appropriate system
        private void ChooseObjectForInteraction(GameObject go)
        {
            // Ignore non-interactable objects
            if(!go.TryGetComponent<IInteractable>(out IInteractable component)) return;

            // Select guest if clicked
            if(go.TryGetComponent<Guest>(out Guest guest))
            {
                // Ignore guests not in queue
                if (guest.OrdinalQueueNumber < 0)
                {
                    return;
                }
                
                selectedGuest = guest;
            } 
            else
            {
                // No guest selected → waiter handles interaction
                if(selectedGuest == null)
                {
                    hallManager.RequestWaiter(component);
                } 
                // Guest selected → guest handles interaction
                else
                {
                    hallManager.RequestGuest(component, selectedGuest);
                } 

                // Reset selection after action
                selectedGuest = null;
            }
        }
    }
}

