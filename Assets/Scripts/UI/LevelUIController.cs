using System.Collections;
using PandaCafe.Core.LevelManagment;
using UnityEngine;

// Coordinates level UI panels.
// Switches HUD, goals, pause, and result screens.
// Reacts to level state changes.
namespace PandaCafe.UI
{
    public class LevelUIController : MonoBehaviour
    {
        [SerializeField] private float levelGoalsDisplaySeconds = 2f;
        private LevelManager levelManager;
        private GoalManager goalManager;
        private ClockManager clockManager;

        private LevelHeaderUI levelHeaderUI;
        private PauseLevelUI pauseLevelUI;
        private CompletedLevelUI completedLevelUI;
        private LevelGoalsUI levelGoalsUI;

        private bool hasShownLevelIntro;

        private Coroutine levelGoalsRoutine;

        // Initialize dependencies
        public void Init(LevelManager levelManager,
                         GoalManager goalManager, 
                        ClockManager clockManager, 
                        LevelHeaderUI levelHeaderUI, 
                        PauseLevelUI pauseLevelUI, 
                        CompletedLevelUI completedLevelUI, 
                        LevelGoalsUI levelGoalsUI)
        {
            this.levelManager = levelManager;
            this.goalManager = goalManager;
            this.clockManager = clockManager;

            this.levelHeaderUI = levelHeaderUI;
            this.pauseLevelUI = pauseLevelUI;
            this.completedLevelUI = completedLevelUI;
            this.levelGoalsUI = levelGoalsUI;

            levelHeaderUI.Init(goalManager, clockManager);
            completedLevelUI.Init(levelManager);
            pauseLevelUI.Init(levelManager);

            levelManager.LevelStateChanged += HandleLevelState;

            levelHeaderUI.SetInitialValues(levelManager.CurrentLevel.targetMoney, levelManager.CurrentLevel.targetServedGuests, levelManager.CurrentLevel.maxLostGuests);
            levelGoalsUI.SetInitialValues(levelManager.CurrentLevel.targetMoney, levelManager.CurrentLevel.targetServedGuests, levelManager.CurrentLevel.maxLostGuests);

            HandleLevelState();
        }

        // Unsubscribe events
        void OnDisable()
        {
            if (levelManager != null)
            {
                levelManager.LevelStateChanged -= HandleLevelState;
            }

            StopLevelGoalsRoutine();
        }

        // Route level state UI
        private void HandleLevelState()
        {
            switch (levelManager.LevelState)
            {
                case LevelState.Playing:
                    if (!hasShownLevelIntro)
                    {
                        HandleFirstPlaying();
                        break;
                    } 
                    HandlePlaying();
                    break;
                case LevelState.Win: 
                    HandleWin();
                    break;
                case LevelState.Lost: 
                    HandleLost();
                    break;
                case LevelState.Pause:
                    HandlePause();
                    break; 
            }
        }

        // Show level goals before play
        private IEnumerator ShowLevelGoalsIntro()
        {
            Time.timeScale = 0f;

            yield return new WaitForSecondsRealtime(levelGoalsDisplaySeconds);

            HandlePlaying();

            levelGoalsRoutine = null;
        }

        // Show first playing state
        private void HandleFirstPlaying()
        {
            levelHeaderUI.SetActive(false);
            pauseLevelUI.SetActive(false);
            completedLevelUI.SetActive(false);
            levelGoalsUI.SetActive(true);

            StopLevelGoalsRoutine();

            hasShownLevelIntro = true;
            levelGoalsRoutine = StartCoroutine(ShowLevelGoalsIntro());
        }

        // Show playing UI
        private void HandlePlaying()
        {
            levelHeaderUI.SetActive(true);
            pauseLevelUI.SetActive(false);
            completedLevelUI.SetActive(false);
            levelGoalsUI.SetActive(false);

            Time.timeScale = 1f;
        }

        // Show win result
        private void HandleWin()
        {
            HandleCompleted(isLevelWon: true);
        }

        // Show loss UI
        private void HandleLost()
        {
            HandleCompleted(isLevelWon: false);
        }

        // Show completed level UI
        private void HandleCompleted(bool isLevelWon)
        {
            StopLevelGoalsRoutine();

            levelHeaderUI.SetActive(false);
            pauseLevelUI.SetActive(false);
            levelGoalsUI.SetActive(false);

            completedLevelUI.SetResult(isLevelWon, goalManager.CurrentProgress.CoinsEarned, goalManager.CurrentProgress.ServedGuests, goalManager.CurrentProgress.LostGuests, goalManager.CurrentProgress.EarnedStars);
            completedLevelUI.SetActive(true);

            Time.timeScale = 0f;
        }

        // Show pause UI
        private void HandlePause()
        {
            StopLevelGoalsRoutine();

            levelHeaderUI.SetActive(false);
            completedLevelUI.SetActive(false);
            levelGoalsUI.SetActive(false);
            pauseLevelUI.SetActive(true);

            Time.timeScale = 0f;
        }

        // Stop goals intro routine
        private void StopLevelGoalsRoutine()
        {
            if (levelGoalsRoutine == null) return;

            StopCoroutine(levelGoalsRoutine);
            levelGoalsRoutine = null;
        }
    }
}