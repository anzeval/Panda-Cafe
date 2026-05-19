using UnityEngine;
using UnityEngine.UI;
using PandaCafe.Core.GameManagment;

// Controls level map navigation UI.
// Handles main menu and shop buttons.
// Keeps map button listeners in one place.
namespace PandaCafe.UI
{
    public class LevelMapUI : MonoBehaviour
    {
        [SerializeField] private Button backToMenuButton;
        [SerializeField] private Button shopButton;

        // Subscribe events
        private void OnEnable()
        {
            if (backToMenuButton != null)
            {
                backToMenuButton.onClick.AddListener(GoToMainMenu);
            }
            if (shopButton != null)
            {
                shopButton.onClick.AddListener(GoToShop);
            }
        }

        // Unsubscribe events
        private void OnDisable()
        {
            if (backToMenuButton != null)
            {
                backToMenuButton.onClick.RemoveListener(GoToMainMenu);
            }
            if (shopButton != null)
            {
                shopButton.onClick.RemoveListener(GoToShop);
            }
        }

        // Open main menu
        public void GoToMainMenu()
        {
            GameManager.LoadMainMenu();
        }

        // Open shop
        public void GoToShop()
        {
            GameManager.LoadShop();
        }
    }   
}
