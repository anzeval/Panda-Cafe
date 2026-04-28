using UnityEngine;

namespace PandaCafe.Core
{
    public class LevelProgressData
    {
        public int LevelId { get; private set; }
        public int CoinsEarned { get; private set; }
        public int CoinsGoal { get; private set; }

        public LevelProgressData(int levelId, int coinsGoal = 0)
        {
            LevelId = levelId;
            CoinsGoal = Mathf.Max(0, coinsGoal);
            CoinsEarned = 0;
        }

        public void SetGoal(int goal)
        {
            CoinsGoal = Mathf.Max(0, goal);
        }

        public void AddCoins(int amount)
        {
            if (amount <= 0) return;

            CoinsEarned += amount;
        }

        public bool IsGoalReached()
        {
            return CoinsGoal > 0 && CoinsEarned >= CoinsGoal;
        }
    }
}
