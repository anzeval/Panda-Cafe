namespace PandaCafe.WaiterNPC
{
    // Defines waiter tasks
    public enum WaiterTask
    {
        None,               // No task
        TakingOrder,        // Take order from guest
        PickingUpDish,      // Pick dish from kitchen
        DeliveringDish,     // Deliver dish to table
        DiscardingDish,     // Throw away dish
        CollectingPayment   // Collect payment
    }   
}