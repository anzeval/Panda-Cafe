using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PandaCafe.Core.GameManagment;
using PandaCafe.Core.LevelManagment;

// Shows completed level result screen.
// Displays win or loss stats and stars.
// Handles navigation to map or shop.
namespace PandaCafe.UI
{
    public class CompletedLevelUI : MonoBehaviour
    {
        [SerializeField] private GameObject childContainer;

        [SerializeField] private Image angrySign;

        [SerializeField] private Image star1;
        [SerializeField] private Image star2;
        [SerializeField] private Image star3;

        [SerializeField] private TMP_Text dayCompletedHeader;
        [SerializeField] private TMP_Text dayLooseHeader;

        [SerializeField] private TMP_Text coinGoal;
        [SerializeField] private TMP_Text servedGuestsGoal;
        [SerializeField] private TMP_Text lostGuestsGoal;

        [SerializeField] private TMP_Text coinCurrent;
        [SerializeField] private TMP_Text servedGuestsCurrent;
        [SerializeField] private TMP_Text lostGuestsCurrent;

        [SerializeField] private Button GoToMapBTN;
        [SerializeField] private Button shopBTN;

        private LevelManager levelManager;

        // Initialize dependencies
        public void Init(LevelManager levelManager)
        {
            this.levelManager = levelManager;

            coinGoal.text = levelManager.CurrentLevel.targetMoney.ToString();
            servedGuestsGoal.text = levelManager.CurrentLevel.targetServedGuests.ToString();
            lostGuestsGoal.text = levelManager.CurrentLevel.maxLostGuests.ToString();
        }

        // Subscribe events
        private void OnEnable()
        {
            if (GoToMapBTN != null)
            {
                GoToMapBTN.onClick.AddListener(LoadLevelMap);
            }

            if (shopBTN != null)
            {
                shopBTN.onClick.AddListener(LoadShop);
            }
        }

        // Unsubscribe events
        private void OnDisable()
        {
            if (GoToMapBTN != null)
            {
                GoToMapBTN.onClick.RemoveListener(LoadLevelMap);
            }

            if (shopBTN != null)
            {
                shopBTN.onClick.RemoveListener(LoadShop);
            }
        }

        // Load level map scene
        private void LoadLevelMap()
        {   
            GameManager.LoadLevelMap();
        }

        // Load shop scene
        private void LoadShop()
        {   
            GameManager.LoadShop();
        }

        // Toggle UI visibility
        public void SetActive(bool isActive)
        {
            childContainer.SetActive(isActive);
        }

        // Show level result
        public void SetResult(bool isLevelWon, int earnedCoins, int servedGuests, int lostGuests, int earnedStars)
        {
            coinCurrent.text = earnedCoins.ToString();
            servedGuestsCurrent.text = servedGuests.ToString();
            lostGuestsCurrent.text = (levelManager.CurrentLevel.maxLostGuests - lostGuests).ToString();

            SetStars(earnedStars);

            if(isLevelWon) HandleWin();
            else HandleLoss();
        }  

        // Update result stars
        private void SetStars(int earnedStars)
        {
            SetStarEnabled(star1, earnedStars >= 1);
            SetStarEnabled(star2, earnedStars >= 2);
            SetStarEnabled(star3, earnedStars >= 3);
        }

        // Toggle star image
        private void SetStarEnabled(Image star, bool isEnabled)
        {
            if (star == null) return;

            star.enabled = isEnabled;
        } 

        // Show win result
        private void HandleWin()
        {
            dayCompletedHeader.enabled = true;
            dayLooseHeader.enabled = false;
            angrySign.enabled = false;
        }

        // Show loss result
        private void HandleLoss()
        {
            dayCompletedHeader.enabled = false;
            dayLooseHeader.enabled = true;
            angrySign.enabled = true; 
        }
    }
}

