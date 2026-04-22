using PandaCafe.NPC;
using PandaCafe.Managers;
using PandaCafe.Input;
using UnityEngine;

namespace PandaCafe.Interaction
{
    // Handles interaction routing between input and game logic.
    // Receives input events (clicks on objects), validates whether the object is interactable, and decides who should handle the interaction — a waiter or a guest.
    public class InteractionManager : MonoBehaviour
    {
        InputHandler inputHandler;
        HallManager hallManager;
        Guest selectedGuest;

        public void Init(InputHandler inputHandler, HallManager hallManager)
        {
            this.inputHandler = inputHandler;
            this.hallManager = hallManager;

            inputHandler.interacted += ChooseObjectForInteraction;
        }

        void OnDisable()
        {
            inputHandler.interacted -= ChooseObjectForInteraction;
        }

        // Determines whether the interaction should be handled by a waiter or a selected guest, based on the clicked object.
        private void ChooseObjectForInteraction(GameObject go)
        {
            if(!go.TryGetComponent<IInteractable>(out IInteractable component)) return;

            if(go.TryGetComponent<Guest>(out Guest guest))
            {
                selectedGuest = guest;
            } 
            else
            {
                // Waiter action
                if(selectedGuest == null)
                {
                    hallManager.RequestWaiter(component);
                } 
                // Guest action
                else
                {
                    hallManager.RequestGuest(component, selectedGuest);
                } 

                selectedGuest = null;
            }
        }
    }
}

