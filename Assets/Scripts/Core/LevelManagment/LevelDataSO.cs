using UnityEngine;

// Stores level configuration data.
// Defines goals, unlock state, and best stars.
// Used by level selection and progress logic.
namespace PandaCafe.Core.LevelManagment
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/LevelData")]
    public class LevelDataSO : ScriptableObject
    {
        public int levelIndex;

        public int targetMoney;
        public int targetServedGuests;
        public int maxLostGuests;

        public bool isUnlocked;

        [Range(0, 3)] public int maxEarnedStars;

        // Unlock level data
        public void Unlock()
        {
            isUnlocked = true;
        }

        // Store best stars
        public void RecordEarnedStars(int earnedStars)
        {
            maxEarnedStars = Mathf.Max(maxEarnedStars, Mathf.Clamp(earnedStars, 0, 3));
        }
    }
}