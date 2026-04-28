using PandaCafe.Interaction;

namespace PandaCafe.HallManagment
{
    // Defines interaction rules between actors and targets
    public static class HallInteractionRouter
    {
        // Check if actor can interact with target type
        public static bool CanInteract(InteractionActor actor, InteractionType interactionType)
        {
            // Guests interact only with tables
            if (actor == InteractionActor.Guest)
            {
                return interactionType == InteractionType.Table;
            }

            // Other actors can interact with multiple types
            return interactionType == InteractionType.Table  || interactionType == InteractionType.Kitchen  || interactionType == InteractionType.Trash;
        }
    }
}