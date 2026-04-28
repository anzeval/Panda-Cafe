using PandaCafe.Interaction;
using PandaCafe.HallManagment;


namespace PandaCafe.NPC
{
    public class GuestPatienceCoordinator
    {
        private readonly SeatingService seatingService;

        public GuestPatienceCoordinator(SeatingService seatingService)
        {
            this.seatingService = seatingService;
        }

        public void RegisterGuestAtTable(Guest guest)
        {
            if (guest == null) return;

            guest.PatienceExpired -= HandleGuestPatienceExpired;
            guest.PatienceExpired += HandleGuestPatienceExpired;
            guest.MealCompleted -= HandleGuestMealCompleted;
            guest.MealCompleted += HandleGuestMealCompleted;
        }

        public void UnregisterGuest(Guest guest)
        {
            if (guest == null) return;

            guest.PatienceExpired -= HandleGuestPatienceExpired;
            guest.MealCompleted -= HandleGuestMealCompleted;
        }

        private void HandleGuestPatienceExpired(Guest guest)
        {
            if (guest == null) return;
            if (!seatingService.TryGetTableByGuest(guest, out Table table)) return;

            UnregisterGuest(guest);
            seatingService.ClearTable(table);
        }

        private void HandleGuestMealCompleted(Guest guest, int tips)
        {
            if (guest == null) return;
            if (!seatingService.TryGetTableByGuest(guest, out Table table)) return;

            table.AddTips(tips);
            UnregisterGuest(guest);
            seatingService.ClearTable(table);
        }
    }   
}
