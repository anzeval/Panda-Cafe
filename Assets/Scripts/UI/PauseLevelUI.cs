using PandaCafe.Core.GameManagment;
using PandaCafe.Core.LevelManagment;
using UnityEngine;
using UnityEngine.UI;

// Controls pause menu UI.
// Handles pause, resume, and map navigation.
// Toggles pause panel visibility.
namespace PandaCafe.UI
{
    public class PauseLevelUI : MonoBehaviour
    {
        [SerializeField] private GameObject childContainer;

        [SerializeField] private Button pauseBTN;

        [SerializeField] private Button launchBTN;
        [SerializeField] private Button goToMapBTN;

        private LevelManager levelManager;

        // Initialize dependencies
        public void Init(LevelManager levelManager)
        {
            this.levelManager = levelManager;
            SetActive(false);
        }

        // Subscribe events
        private void OnEnable()
        {
            if (pauseBTN != null)
            {
                pauseBTN.onClick.AddListener(Pause);
            }

            if (launchBTN != null)
            {
                launchBTN.onClick.AddListener(Launch);
            }

            if (goToMapBTN != null)
            {
                goToMapBTN.onClick.AddListener(LoadLevelMap);
            }
        }

        // Unsubscribe events
        private void OnDisable()
        {
            if (pauseBTN != null)
            {
                pauseBTN.onClick.RemoveListener(Pause);
            }

            if (launchBTN != null)
            {
                launchBTN.onClick.RemoveListener(Launch);
            }

            if (goToMapBTN != null)
            {
                goToMapBTN.onClick.RemoveListener(LoadLevelMap);
            }
        }

        // Toggle UI visibility
        public void SetActive(bool isActive)
        {
            if (childContainer != null)
            {
                childContainer.SetActive(isActive);
            }

            if (pauseBTN != null)
            {
                pauseBTN.interactable = !isActive;
            }
        }

        // Load level map scene
        private void LoadLevelMap()
        {
            Time.timeScale = 1f;
            GameManager.LoadLevelMap();
        }

        // Resume from pause
        private void Launch()
        {
             if (levelManager != null)
            {
                levelManager.ResumeLevel();
                return;
            }

            Time.timeScale = 1f;
            SetActive(false);
        }

        // Pause level
        private void Pause()
        {
            if (levelManager != null)
            {
                levelManager.PauseLevel();
                return;
            }

            Time.timeScale = 0f;
            SetActive(true);
        }
    }  
}

