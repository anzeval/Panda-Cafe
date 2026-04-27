using System.Collections.Generic;
using PandaCafe.Interaction;
using PandaCafe.NPC;

namespace PandaCafe.HallManagment
{
    public class SeatingService
    {
        public Dictionary<Table, Guest> guestsByTable {get; private set;}

        public SeatingService()
        {
            guestsByTable = new Dictionary<Table, Guest>();
        }

        public bool TryGetGuestAtTable(Table table, out Guest guest)
        {
            guest = null;

            if (table == null)
            {
                return false;
            }

            if (!guestsByTable.TryGetValue(table, out Guest _guest))
            {
                return false;
            }

            guest = _guest;
            return guest != null;
        }

        public bool TrySetGuest(Guest guest, Table table)
        {
            if(guest == null || table == null) return false;

            guestsByTable[table] = guest;
            return true;
        }

        public void ClearTable(Table table)
        {
            if (table == null) return;

             if (table.CurrentGuest != null)
            {
                table.CurrentGuest.PatienceExpired -= HandleGuestPatienceExpired;
            }

            table.FreeTable();
            guestsByTable.Remove(table);
        }
    }
}