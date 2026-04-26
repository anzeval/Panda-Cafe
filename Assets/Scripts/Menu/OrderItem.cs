namespace PandaCafe.Menu
{
    public class OrderItem 
    {
        public MenuItemSO menuItemSO {get; private set;}
        public int quantity {get; private set;}

        public OrderItem(MenuItemSO menuItemSO, int quantity)
        {
            this.menuItemSO = menuItemSO;
            this.quantity = quantity;
        }
    }
}