using UnityEngine;
using UnityEngine.SceneManagement;
using PandaCafe.Core.LevelManagment;
using System.Collections.Generic;

// Stores global game state and scene flow.
// Keeps runtime balance and selected level.
// Tracks runtime level progress.
// Provides shared money helpers.
namespace PandaCafe.Core.GameManagment
{
    public class GameManager : MonoBehaviour
    {
        private const int StartingBalance = 0;

        public static GameManager Instance { get; private set; }
        public static GameState GameState { get; private set; } = GameState.MainMenu;
        public static LevelDataSO CurrentLevel { get; private set; }
        public static int TotalBalance { get; private set; } = StartingBalance;

        private static readonly Dictionary<int, LevelRuntimeProgress> levelProgress = new Dictionary<int, LevelRuntimeProgress>();

        private const string MainMenuSceneName = "MainMenu";
        private const string LevelMapSceneName = "LevelMap";
        private const string GameSceneName = "Game";
        private const string ShopSceneName = "Shop";

        // Initialize component references
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            TotalBalance = StartingBalance;
        }

        // Load main menu scene
        public static void LoadMainMenu()
        {
            LoadScene(GameState.MainMenu, MainMenuSceneName);
        }

        // Load level map scene
        public static void LoadLevelMap()
        {
            LoadScene(GameState.LevelMap, LevelMapSceneName);
        }

        // Load shop scene
        public static void LoadShop()
        {
            LoadScene(GameState.Shop, ShopSceneName);
        }

        // Load selected level
        public static void LoadLevel(LevelDataSO levelDataSO)
        {
            if (levelDataSO == null || !IsLevelUnlocked(levelDataSO)) return;

            LoadScene(GameState.Game, GameSceneName, levelDataSO);
        }

        // Set active level
        public static void SetCurrentLevel(LevelDataSO levelDataSO)
        {
            CurrentLevel = levelDataSO;
        }

        // Check level unlock state
        public static bool IsLevelUnlocked(LevelDataSO levelDataSO)
        {
            if (levelDataSO == null) return false;

            LevelRuntimeProgress progress = GetOrCreateLevelProgress(levelDataSO);
            return progress.IsUnlocked;
        }

        // Get best earned stars
        public static int GetLevelMaxEarnedStars(LevelDataSO levelDataSO)
        {
            if (levelDataSO == null) return 0;

            LevelRuntimeProgress progress = GetOrCreateLevelProgress(levelDataSO);
            return progress.MaxEarnedStars;
        }

        // Save level result
        public static void RecordLevelResult(LevelDataSO levelDataSO, int earnedStars)
        {
            if (levelDataSO == null) return;

            LevelRuntimeProgress progress = GetOrCreateLevelProgress(levelDataSO);
            progress.MaxEarnedStars = Mathf.Max(progress.MaxEarnedStars, Mathf.Clamp(earnedStars, 0, 3));
            progress.IsUnlocked = true;

            ApplyProgressToLevelData(levelDataSO, progress);
        }

        // Unlock level
        public static void UnlockLevel(LevelDataSO levelDataSO)
        {
            if (levelDataSO == null) return;

            LevelRuntimeProgress progress = GetOrCreateLevelProgress(levelDataSO);
            progress.IsUnlocked = true;

            ApplyProgressToLevelData(levelDataSO, progress);
        }

        // Get runtime level progress
        private static LevelRuntimeProgress GetOrCreateLevelProgress(LevelDataSO levelDataSO)
        {
            // Runtime progress mirrors ScriptableObject defaults without saving them
            if (!levelProgress.TryGetValue(levelDataSO.levelIndex, out LevelRuntimeProgress progress))
            {
                progress = new LevelRuntimeProgress(levelDataSO.isUnlocked, levelDataSO.maxEarnedStars);
                levelProgress.Add(levelDataSO.levelIndex, progress);
            }

            return progress;
        }

        // Add money to total balance
        public static void AddMoneyToBalance(int amount)
        {
            if (amount <= 0) return;

            TotalBalance += amount;
        }

        // Check if balance is enough
        public static bool CanAfford(int amount)
        {
            return amount >= 0 && TotalBalance >= amount;
        }

        // Spend money if possible
        public static bool TrySpendMoney(int amount)
        {
            if (!CanAfford(amount)) return false;

            TotalBalance -= amount;
            return true;
        }

        // Spend money from instance call
        public void DraftMoney(int amount)
        {
            TrySpendMoney(amount);
        }

        // Load scene and set state
        private static void LoadScene(GameState gameState, string sceneName, LevelDataSO levelDataSO = null)
        {
            CurrentLevel = levelDataSO;
            ResetTimeScale();

            // Set state before loading so new scene systems read correct mode
            GameState = gameState;
            SceneManager.LoadScene(sceneName);
        }

        // Copy runtime progress to level data
        private static void ApplyProgressToLevelData(LevelDataSO levelDataSO, LevelRuntimeProgress progress)
        {
            levelDataSO.isUnlocked = progress.IsUnlocked;
            levelDataSO.maxEarnedStars = progress.MaxEarnedStars;
        }

        // Restore normal time scale
        private static void ResetTimeScale()
        {
            Time.timeScale = 1f;
        }

        private sealed class LevelRuntimeProgress
        {
            public bool IsUnlocked;
            public int MaxEarnedStars;

            // Create runtime progress data
            public LevelRuntimeProgress(bool isUnlocked, int maxEarnedStars)
            {
                IsUnlocked = isUnlocked;
                MaxEarnedStars = Mathf.Clamp(maxEarnedStars, 0, 3);
            }
        }
    }
}
