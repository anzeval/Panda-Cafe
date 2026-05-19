using UnityEngine;
using PandaCafe.Core.GameManagment;
using PandaCafe.UI;
using System;

// Controls level lifecycle state.
// Loads current or next level data.
// Handles win, loss, pause, and resume flow.
namespace PandaCafe.Core.LevelManagment
{
    public class LevelManager : MonoBehaviour
    {
        public LevelDataSO CurrentLevel { get; private set;}
        public LevelState LevelState { get; private set;}

        public event Action LevelStateChanged;

        private LevelDatabase levelDatabase;

        // Initialize dependencies
        public bool Init(LevelDatabase levelDatabase)
        {
            SetLevelState(LevelState.Playing);

            this.levelDatabase = levelDatabase;

            if (GameManager.CurrentLevel != null)
            {
                return TryLoadLevel(GameManager.CurrentLevel.levelIndex);
            }

            return TryLoadDefaultLevel();
        }

        // Complete level day
        public void CompleteDay(GoalManager goalManager)
        {
            if (LevelState != LevelState.Playing) return;

            int earnedStars = goalManager != null ? goalManager.CalculateEarnedStars() : 0;

            GameManager.RecordLevelResult(CurrentLevel, earnedStars);
            AddCoinsEarnedAboveGoalToBalance(goalManager);

            if (earnedStars > 0)
            {
                levelDatabase?.TryUnlockNextLevel(CurrentLevel);
            }

            SetLevelState(earnedStars > 0 ? LevelState.Win : LevelState.Lost);
        }

        // Mark level as lost
        public void FailLevel()
        {
            if (LevelState != LevelState.Playing) return;

            SetLevelState(LevelState.Lost);
        }

        // Pause level
        public void PauseLevel()
        {
            if (LevelState != LevelState.Playing) return;

            SetLevelState(LevelState.Pause);
        }

        // Resume level
        public void ResumeLevel()
        {
            if (LevelState != LevelState.Pause) return;

            SetLevelState(LevelState.Playing);
        }

        // Add bonus coins to balance
        private void AddCoinsEarnedAboveGoalToBalance(GoalManager goalManager)
        {
            int coinsEarnedAboveGoal = goalManager?.CurrentProgress?.CoinsEarnedAboveGoal ?? 0;
            GameManager.AddMoneyToBalance(coinsEarnedAboveGoal);
        }

        // Set level state
        private void SetLevelState(LevelState levelState)
        {
            if (LevelState == levelState) return;

            LevelState = levelState;
            LevelStateChanged?.Invoke();
        }

        // Load level by index
        public bool TryLoadLevel(int levelIndex)
        {
            if (levelDatabase == null) return false;
            if (!levelDatabase.TryGetLevelData(levelIndex, out var levelData)) return false;

            if (!GameManager.IsLevelUnlocked(levelData)) return false;

            SetCurrentLevel(levelData);
            return true;
        }

        // Load next level
        public bool TryLoadNextLevel()
        {
            if (CurrentLevel == null) return false;

            return TryLoadLevel(CurrentLevel.levelIndex + 1);
        }

        // Load first level
        private bool TryLoadDefaultLevel()
        {
            if (levelDatabase == null) return false;
            if (!levelDatabase.TryGetFirstLevelData(out var levelData)) return false;

            SetCurrentLevel(levelData);
            return true;
        }

        // Set current level in local and global state
        private void SetCurrentLevel(LevelDataSO levelData)
        {
            CurrentLevel = levelData;
            GameManager.SetCurrentLevel(levelData);
        }
    }
}
