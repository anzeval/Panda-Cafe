using UnityEngine;

// Stores progress for one level run.
// Tracks coins, guests, losses, and stars.
// Calculates win goals and success rates.
namespace PandaCafe.Core.LevelManagment
{
    public class LevelProgressData
    {
        public int LevelId { get; private set; }

        public int CoinsEarned { get; private set; }
        public int CoinsGoal { get; private set; }

        public int ServedGuests { get; private set; }
        public int ServedGuestsGoal { get; private set; }

        public int LostGuests { get; private set; }
        public int MaxLostGuests { get; private set; }
        public int RemainingLostGuests => Mathf.Max(0, MaxLostGuests - LostGuests);
        public int CoinsEarnedAboveGoal => Mathf.Max(0, CoinsEarned - CoinsGoal);

        public int EarnedStars { get; private set; }

        private const float TwoStarsSuccessRate = 1.25f;
        private const float ThreeStarsSuccessRate = 1.5f;

        // Create level progress data
        public LevelProgressData(int levelId, int coinsGoal = 0, int servedGuestsGoal = 0, int maxLostGuests = 0)
        {
            LevelId = levelId;
            
            CoinsGoal = Mathf.Max(0, coinsGoal);
            ServedGuestsGoal = Mathf.Max(0, servedGuestsGoal);
            MaxLostGuests = Mathf.Max(0, maxLostGuests);

            CoinsEarned = 0;
            ServedGuests = 0;
            LostGuests = 0;
            EarnedStars = 0;
        }

        // Add coins to progress
        public void AddCoins(int amount)
        {
            if (amount <= 0) return;

            CoinsEarned += amount;
        }

        // Register served guest
        public void RegisterServedGuest(int earnedCoins)
        {
            ServedGuests++;
            AddCoins(earnedCoins);
        }

        // Register lost guest
        public void RegisterLostGuest()
        {
            LostGuests++;
        }

        // Check lost guest limit
        public bool IsLostGuestsLimitExceeded()
        {
            if (MaxLostGuests <= 0) return LostGuests > 0;

            return LostGuests >= MaxLostGuests;
        }

        // Calculate earned stars
        public int CalculateEarnedStars()
        {
            EarnedStars = GetCalculatedStars();
            return EarnedStars;
        }

        // Check win goals
        public bool IsLevelWon()
        {
            bool moneyGoalReached = CoinsEarned >= CoinsGoal;
            bool servedGoalReached = ServedGuests >= ServedGuestsGoal;
            bool hasLostGuestLives = !IsLostGuestsLimitExceeded();

            return moneyGoalReached && servedGoalReached && hasLostGuestLives;
        }

        // Calculate stars from progress
        private int GetCalculatedStars()
        {
            if (!IsLevelWon()) return 0;

            float successRate = GetAverageSuccessRate();

            if (successRate >= ThreeStarsSuccessRate) return 3;
            if (successRate >= TwoStarsSuccessRate) return 2;

            return 1;
        }

        // Calculate average goal rate
        private float GetAverageSuccessRate()
        {
            float moneyRate = GetPositiveGoalRate(CoinsEarned, CoinsGoal);
            float servedGuestsRate = GetPositiveGoalRate(ServedGuests, ServedGuestsGoal);
            float lostGuestsRate = GetLostGuestsRate();

            return (moneyRate + servedGuestsRate + lostGuestsRate) / 3f;
        }

        // Calculate positive goal rate
        private float GetPositiveGoalRate(int currentValue, int goalValue)
        {
            if (goalValue <= 0) return 1f;

            float goalRate = Mathf.Max(0f, (float)currentValue / goalValue);
            return Mathf.Min(goalRate, ThreeStarsSuccessRate);
        }

        // Calculate lost guests rate
        private float GetLostGuestsRate()
        {
            if (MaxLostGuests <= 0) return LostGuests <= 0 ? ThreeStarsSuccessRate : 0f;

            if (IsLostGuestsLimitExceeded()) return 0f;

            float savedGuestRate = (float)(MaxLostGuests - LostGuests) / MaxLostGuests;
            float perfectLostGuestsBonus = ThreeStarsSuccessRate - 1f;
            return 1f + savedGuestRate * perfectLostGuestsBonus;
        }
    }
}
