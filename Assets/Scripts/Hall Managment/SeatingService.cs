using System.Collections.Generic;
using PandaCafe.Interaction;
using PandaCafe.NPC;

namespace PandaCafe.HallManagment
{
    public class SeatingService
    {
        private readonly Dictionary<Table, Guest> guestsByTable = new Dictionary<Table, Guest>();
        private readonly Dictionary<Guest, Table> tablesByGuest = new Dictionary<Guest, Table>();

        public bool TryGetGuestAtTable(Table table, out Guest guest)
        {
            guest = null;

            if (table == null) return false;
            if (!guestsByTable.TryGetValue(table, out Guest foundGuest)) return false;

            guest = foundGuest;
            return guest != null;
        }

        public bool TryGetTableByGuest(Guest guest, out Table table)
        {
            table = null;

            if (guest == null) return false;
            if (!tablesByGuest.TryGetValue(guest, out Table foundTable)) return false;

            table = foundTable;
            return table != null;
        }

        public bool SeatGuestAtTable(Guest guest, Table table)
        {
            if (guest == null || table == null) return false;

            table.OccupyTable(guest);
            guestsByTable[table] = guest;
            tablesByGuest[guest] = table;
            return true;
        }

        public void ClearTable(Table table)
        {
            if (table == null) return;

            if (guestsByTable.TryGetValue(table, out Guest guest) && guest != null)
            {
                tablesByGuest.Remove(guest);
            }

            table.FreeTable();
            guestsByTable.Remove(table);
        }
    }
}