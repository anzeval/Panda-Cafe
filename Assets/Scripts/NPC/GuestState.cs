// Defines guest lifecycle states.
// Drives guest timers, movement, and actions.
// Used by guest behavior and animation.
namespace PandaCafe.NPC
{
    public enum GuestState
    {
        GoingToQueue, 
        WaitingInQueue,
        GoingToTable,
        ReadingMenu,
        WaitingForOrder,
        WaitingForFood,
        Eating,
        GoingToExit
    }
}