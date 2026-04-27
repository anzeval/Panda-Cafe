using UnityEngine;

namespace PandaCafe.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameState GameState {get; private set;}
        public static GameManager Instance {get; private set;}

        public LevelProgressData CurrentLevelProgress { get; private set; }

        void Awake()
        {
            if(Instance != null && Instance != this)
            {
                Destroy (gameObject);
            } 

            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            GameState = GameState.Playing;
            StartLevel(1,200);
        }

        public void StartLevel(int levelId, int coinsGoal = 0)
        {
            CurrentLevelProgress = new LevelProgressData(levelId, coinsGoal);
        }

        public void AddCoinsForCurrentLevel(int amount)
        {
            CurrentLevelProgress?.AddCoins(amount);
        }

        public void SetCurrentLevelCoinsGoal(int goal)
        {
            CurrentLevelProgress?.SetGoal(goal);
        }

        public bool IsCurrentLevelGoalReached()
        {
            return CurrentLevelProgress != null && CurrentLevelProgress.IsGoalReached();
        }
    }
}
