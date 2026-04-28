using PandaCafe.Interaction;
using PandaCafe.NPC;

namespace PandaCafe.Menu
{
    // Represents a single order entry
    public class OrderItem 
    {
        // Ordered item
        public MenuItemSO MenuItemSO { get; private set; }

        // Item count
        public int Quantity { get; private set; }

        // Guest who ordered
        public Guest Guest { get; private set; }

        // Table where order belongs
        public Table Table { get; private set; }

        public OrderItem(MenuItemSO menuItemSO, int quantity, Guest guest, Table table)
        {
            MenuItemSO = menuItemSO;
            Quantity = quantity;
            Guest = guest;
            Table = table;
        }
    }
}