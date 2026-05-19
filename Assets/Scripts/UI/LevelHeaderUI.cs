using UnityEngine;
using TMPro;
using PandaCafe.Core.LevelManagment;

// Shows active level HUD values.
// Displays goals, progress, and day time.
// Subscribes to goal and clock updates.
namespace PandaCafe.UI
{
    public class LevelHeaderUI : MonoBehaviour
    {
        [SerializeField] private GameObject childContainer;

        [SerializeField] private TMP_Text coinGoal;
        [SerializeField] private TMP_Text servedGuestsGoal;
        [SerializeField] private TMP_Text lostGuestsGoal;

        [SerializeField] private TMP_Text coinCurrent;
        [SerializeField] private TMP_Text servedGuestsCurrent;
        [SerializeField] private TMP_Text lostGuestsCurrent;

        [SerializeField] private TMP_Text dayTime;

        private int maxLostGuests;

        private GoalManager goalManager;
        private ClockManager clockManager;

        // Initialize dependencies
        public void Init(GoalManager goalManager, ClockManager clockManager)
        {
            this.goalManager = goalManager;
            this.clockManager = clockManager;

            SubscribeEvents();
        }

        // Subscribe UI events
        private void SubscribeEvents()
        {
            if (goalManager != null)
            {
                goalManager.CoinEarned += UpdateCoinUI;
                goalManager.GuestLosted += UpdateLostGuestsUI;
                goalManager.GuestServed += UpdateServedGuestsUI;
            }

            if (clockManager != null)
            {
                UpdateTimeUI(clockManager.CurrentTimeText);
                clockManager.TimeChanged += UpdateTimeUI;
            }
        }

        // Subscribe events
        private void OnEnable()
        {
            SubscribeEvents();
        }

        // Unsubscribe events
        private void OnDisable()
        {
            if (goalManager != null)
            {
                goalManager.CoinEarned -= UpdateCoinUI;
                goalManager.GuestLosted -= UpdateLostGuestsUI;
                goalManager.GuestServed -= UpdateServedGuestsUI;
            }

            if (clockManager != null)
            {
                clockManager.TimeChanged -= UpdateTimeUI;
            }
        }

        // Set initial goal values
        public void SetInitialValues(int _coinGoal, int _servedGuestsGoal, int _maxLostGuests)
        {
            coinGoal.text = _coinGoal.ToString();
            servedGuestsGoal.text = _servedGuestsGoal.ToString();
            lostGuestsGoal.text = _maxLostGuests.ToString();
            maxLostGuests = Mathf.Max(0, _maxLostGuests);
            lostGuestsGoal.text = maxLostGuests.ToString();

            coinCurrent.text = 0.ToString();
            servedGuestsCurrent.text = 0.ToString();
            lostGuestsCurrent.text = _maxLostGuests.ToString();
            lostGuestsCurrent.text = maxLostGuests.ToString();
        }

        // Toggle UI visibility
        public void SetActive(bool isActive)
        {
            childContainer.SetActive(isActive);
        }

        // Update coin counter
        private void UpdateCoinUI(int coinsEarned)
        {
            coinCurrent.text = coinsEarned.ToString();   
        }

        // Update lost guests counter
        private void UpdateLostGuestsUI(int lostGuests)
        {
            lostGuestsCurrent.text = lostGuests.ToString();
            int remainingLostGuests = Mathf.Max(0, maxLostGuests - lostGuests);
            lostGuestsCurrent.text = remainingLostGuests.ToString();
        }

        // Update served guests counter
        private void UpdateServedGuestsUI(int servedGuests)
        {
            servedGuestsCurrent.text = servedGuests.ToString();
        }

        // Update time text
        private void UpdateTimeUI(string time)
        {
            dayTime.text = time;
        }
    }
}