namespace PandaCafe.Menu
{
    public class OrderItem 
    {
        public MenuItemSO menuItemSO {get; private set;}
        public int count {get; private set;}

        public OrderItem(MenuItemSO menuItemSO, int count)
        {
            this.menuItemSO = menuItemSO;
            this.count = count;
        }
    }
}