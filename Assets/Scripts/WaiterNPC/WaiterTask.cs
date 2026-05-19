// Defines waiter task types.
// Used by waiter order service flow.
// Separates order, dish, trash, and payment tasks.
namespace PandaCafe.WaiterNPC
{
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