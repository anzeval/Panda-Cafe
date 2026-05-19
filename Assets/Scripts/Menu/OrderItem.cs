using PandaCafe.Interaction;
using PandaCafe.NPC;
using PandaCafe.Interaction.InteractionObjects;

// Stores one guest order.
// Keeps menu item, quantity, guest, and table.
// Passed between waiter, kitchen, and order manager.
namespace PandaCafe.Menu
{
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

        // Create order item data
        public OrderItem(MenuItemSO menuItemSO, int quantity, Guest guest, Table table)
        {
            MenuItemSO = menuItemSO;
            Quantity = quantity;
            Guest = guest;
            Table = table;
        }
    }
}