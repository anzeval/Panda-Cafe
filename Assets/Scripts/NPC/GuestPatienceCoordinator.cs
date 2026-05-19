using PandaCafe.Interaction;
using PandaCafe.HallManagment;
using PandaCafe.Interaction.InteractionObjects;
using PandaCafe.Core.LevelManagment;

// Coordinates guest patience events.
// Clears table state when guests leave.
// Registers tips when meals are completed.
namespace PandaCafe.NPC
{
    public class GuestPatienceCoordinator
    {
        // Table management service
        private readonly SeatingService seatingService;
        private readonly GoalManager goalManager;

        // Create guest patience coordinator
        public GuestPatienceCoordinator(SeatingService seatingService, GoalManager goalManager)
        {
            this.seatingService = seatingService;
            this.goalManager = goalManager;
        }

        // Subscribe guest to events
        public void RegisterGuestAtTable(Guest guest)
        {
            if (guest == null) return;

            // Prevent duplicate subscriptions
            guest.PatienceExpired -= HandleGuestPatienceExpired;
            guest.PatienceExpired += HandleGuestPatienceExpired;

            guest.MealCompleted -= HandleGuestMealCompleted;
            guest.MealCompleted += HandleGuestMealCompleted;
        }

        // Unsubscribe guest from events
        public void UnregisterGuest(Guest guest)
        {
            if (guest == null) return;

            guest.PatienceExpired -= HandleGuestPatienceExpired;
            guest.MealCompleted -= HandleGuestMealCompleted;
        }

        // Guest left due to no patience
        private void HandleGuestPatienceExpired(Guest guest)
        {
            if (guest == null) return;

            // Get guest table
            if (!seatingService.TryGetTableByGuest(guest, out Table table)) return;

            UnregisterGuest(guest);

            goalManager?.RegisterLostGuest();

            // Free table
            seatingService.ClearTable(table);
        }

        // Guest finished meal
        private void HandleGuestMealCompleted(Guest guest, int tips)
        {
            if (guest == null) return;

            // Get guest table
            if (!seatingService.TryGetTableByGuest(guest, out Table table)) return;

            // Add tips
            table.AddTips(tips);

            UnregisterGuest(guest);
            goalManager?.RegisterServedGuest(0);

            // Free table
            seatingService.ClearTable(table);
        }
    }   
}