using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace PandaCafe.Input
{
    //Handles player input
    public class InputHandler : MonoBehaviour
    {
        [SerializeField] private PlayerInput input;
        
        private InputAction primaryClick, pointerPosition;
        public Action<GameObject> interacted;

        private void Awake()
        {
            primaryClick = input.actions["PrimaryClick"];
            pointerPosition = input.actions["PointerPosition"];
        }

        private void OnEnable()
        {
            primaryClick.Enable();
            pointerPosition.Enable();

            primaryClick.performed += HandleClick;
        }

        private void OnDisable()
        {
            primaryClick.performed -= HandleClick;

            primaryClick.Disable();
            pointerPosition.Disable();
        }

        private void HandleClick(InputAction.CallbackContext inputValue)
        {
            Vector2 mousePosition = pointerPosition.ReadValue<Vector2>();
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            worldPosition.z = 0;

            RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

            //if the object has collider => activate event Action
            if (hit.collider != null)
            {
                interacted?.Invoke(hit.collider.gameObject);
            }
        }
    }   
}
