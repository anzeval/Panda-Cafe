using PandaCafe.Interaction;
using PandaCafe.HallManagment;

namespace PandaCafe.NPC
{
    // Handles guest patience events and table cleanup
    public class GuestPatienceCoordinator
    {
        // Table management service
        private readonly SeatingService seatingService;

        public GuestPatienceCoordinator(SeatingService seatingService)
        {
            this.seatingService = seatingService;
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

            // Free table
            seatingService.ClearTable(table);
        }
    }   
}