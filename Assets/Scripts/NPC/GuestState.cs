namespace PandaCafe.NPC
{
    // Represents all possible states of a guest NPC during their lifecycle in the cafe
    // Used to control behavior flow via a state machine.
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