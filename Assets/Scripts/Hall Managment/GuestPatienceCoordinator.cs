using PandaCafe.Interaction;
using PandaCafe.NPC;

namespace PandaCafe.HallManagment
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
        }

        public void UnregisterGuest(Guest guest)
        {
            if (guest == null) return;

            guest.PatienceExpired -= HandleGuestPatienceExpired;
        }

        private void HandleGuestPatienceExpired(Guest guest)
        {
            if (guest == null) return;
            if (!seatingService.TryGetTableByGuest(guest, out Table table)) return;

            UnregisterGuest(guest);
            seatingService.ClearTable(table);
        }
    }   
}
