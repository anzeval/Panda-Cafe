using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace PandaCafe.Input
{
    // Handles player input and detects clicked objects in the world.
    // Converts screen input to world position and triggers interaction event.
    public class InputHandler : MonoBehaviour
    {
        [SerializeField] private PlayerInput input;
        
        private InputAction primaryClick, pointerPosition;
        // Fired when player clicks on an object
        public Action<GameObject> interacted;

        // Gets input actions from PlayerInput
        private void Awake()
        {
            primaryClick = input.actions["PrimaryClick"];
            pointerPosition = input.actions["PointerPosition"];
        }

        // Enables input and subscribes to click event
        private void OnEnable()
        {
            primaryClick.Enable();
            pointerPosition.Enable();

            primaryClick.performed += HandleClick;
        }

        // Disables input and unsubscribes from events
        private void OnDisable()
        {
            primaryClick.performed -= HandleClick;

            primaryClick.Disable();
            pointerPosition.Disable();
        }

        // Handles click: converts mouse position to world and raycasts for hit
        private void HandleClick(InputAction.CallbackContext inputValue)
        {
            Vector2 mousePosition = pointerPosition.ReadValue<Vector2>();
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            worldPosition.z = 0;

            RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

            //If collider hit → invoke interaction event
            if (hit.collider != null)
            {
                interacted?.Invoke(hit.collider.gameObject);
            }
        }
    }   
}
