using System;
using UnityEngine;

// Tracks current level goals.
// Registers coins, served guests, and lost guests.
// Calculates win state and earned stars.
namespace PandaCafe.Core.LevelManagment
{
    public class GoalManager : MonoBehaviour
    {
        public LevelProgressData CurrentProgress {get; private set;}
        private LevelManager levelManager;

        public event Action<int> CoinEarned;
        public event Action<int> GuestServed;
        public event Action<int> GuestLosted;

        // Initialize dependencies
        public void Init(LevelManager levelManager)
        {
            this.levelManager = levelManager;
            ResetProgress();
        }

        // Reset current level progress
        private void ResetProgress()
        {
            LevelDataSO level = levelManager != null ? levelManager.CurrentLevel : null;

            CurrentProgress = level != null
                ? new LevelProgressData(level.levelIndex, level.targetMoney, level.targetServedGuests, level.maxLostGuests)
                : new LevelProgressData(0);
        }

        // Register served guest
        public void RegisterServedGuest(int earnedCoins)
        {
            EnsureProgressInitialized();
            CurrentProgress.RegisterServedGuest(earnedCoins);

            GuestServed?.Invoke(CurrentProgress.ServedGuests);
        }

        // Register lost guest
        public void RegisterLostGuest()
        {
            EnsureProgressInitialized();
            CurrentProgress.RegisterLostGuest();

            GuestLosted?.Invoke(CurrentProgress.LostGuests);

            if (CurrentProgress.IsLostGuestsLimitExceeded())
            {
                levelManager?.FailLevel();
            }
        }

        // Add earned money
        public void AddMoney(int amount)
        {
            EnsureProgressInitialized();
            CurrentProgress.AddCoins(amount);

            CoinEarned?.Invoke(CurrentProgress.CoinsEarned);
        }

        // Check win goals
        public bool IsLevelWon()
        {
            EnsureProgressInitialized();
            return CurrentProgress.IsLevelWon();
        }

        // Calculate earned stars
        public int CalculateEarnedStars()
        {
            EnsureProgressInitialized();
            return CurrentProgress.CalculateEarnedStars();
        }

        // Create progress if missing
        private void EnsureProgressInitialized()
        {
            if (CurrentProgress == null) ResetProgress();
        }        
    }
}
