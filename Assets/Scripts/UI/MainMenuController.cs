using UnityEngine;
using UnityEngine.UI;
using PandaCafe.Core.GameManagment;

// Controls main menu start button.
// Finds button reference if needed.
// Opens level map from main menu.
namespace PandaCafe.UI
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private Button startButton;

        // Initialize component references
        private void Awake()
        {
            if (startButton == null)
            {
                startButton = GetComponent<Button>();
            }
        }

        // Subscribe events
        private void OnEnable()
        {
            if (startButton != null)
            {
                startButton.onClick.AddListener(GoToLevelMap);
            }
        }

        // Unsubscribe events
        private void OnDisable()
        {
            if (startButton != null)
            {
                startButton.onClick.RemoveListener(GoToLevelMap);
            }
        }

        // Open level map
        public void GoToLevelMap()
        {
            GameManager.LoadLevelMap();
        }
    }   
}