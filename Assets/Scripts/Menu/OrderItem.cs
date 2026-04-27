using PandaCafe.Interaction;
using PandaCafe.NPC;
namespace PandaCafe.Menu
{
    public class OrderItem 
    {
        public MenuItemSO MenuItemSO { get; private set; }
        public int Quantity { get; private set; }
        public Guest Guest { get; private set; }
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