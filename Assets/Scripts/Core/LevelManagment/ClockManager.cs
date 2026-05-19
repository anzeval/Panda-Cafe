using System;
using UnityEngine;

// Controls in-game day clock.
// Advances time while the level is playing.
// Ends the day when cafe hours are over.
namespace PandaCafe.Core.LevelManagment
{
    public class ClockManager : MonoBehaviour
    {
        [SerializeField] private int openHour = 6;
        [SerializeField] private int closeHour = 23;
        [SerializeField] private float realSecondsPerGameHour = 20f;

        public event Action<int> HourChanged;
        public event Action<string> TimeChanged;

        public int CurrentHour { get; private set; }
        public int CurrentMinute { get; private set; }

        public bool IsCafeOpen => CurrentHour >= openHour && CurrentHour < closeHour;
        
        public string CurrentTimeText => FormatTime(CurrentHour, CurrentMinute);

        private const int MinutesPerHalfHour = 30;
        private const int HalfHoursPerHour = 2;


        private LevelManager levelManager;
        private GoalManager goalManager;
        private float hourTimer;
        private bool isDayCompleted;

        // Initialize dependencies
        public void Init(LevelManager levelManager, GoalManager goalManager)
        {
            this.levelManager = levelManager;
            this.goalManager = goalManager;
            ResetDayClock();
        }

        // Update runtime state each frame
        private void Update()
        {
            if (isDayCompleted || levelManager == null || levelManager.LevelState != LevelState.Playing) return;

            var realSecondsPerHalfHour = realSecondsPerGameHour / HalfHoursPerHour;
            hourTimer += Time.deltaTime;

            if (hourTimer < realSecondsPerHalfHour) return;

            while (hourTimer >= realSecondsPerHalfHour && !isDayCompleted)
            {
                hourTimer -= realSecondsPerHalfHour;
                AdvanceHalfHour();
            }
        }

        // Advance in-game time
        private void AdvanceHalfHour()
        {
            CurrentMinute += MinutesPerHalfHour;

            if (CurrentMinute >= 60)
            {
                CurrentMinute = 0;
                CurrentHour++;
                HourChanged?.Invoke(CurrentHour);
            }

            NotifyTimeChanged();

            if (CurrentHour >= closeHour)
            {
                EndDay();
            }
        }

        // Reset clock to opening time
        private void ResetDayClock()
        {
            CurrentHour = openHour;
            CurrentMinute = 0;

            hourTimer = 0f;
            isDayCompleted = false;
            NotifyTimeChanged();
        }

        // Notify listeners about time
        private void NotifyTimeChanged()
        {
            HourChanged?.Invoke(CurrentHour);
            TimeChanged?.Invoke(CurrentTimeText);
        }

        // Format clock text
        private string FormatTime(int hour, int minute)
        {
            var normalizedHour = ((hour % 24) + 24) % 24;
            return $"{normalizedHour:00}:{minute:00}";
        }

        // Finish current day
        private void EndDay()
        {
            isDayCompleted = true;
            levelManager?.CompleteDay(goalManager);
        }
    }
}