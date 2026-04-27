using PandaCafe.Interaction;

namespace PandaCafe.HallManagment
{
    public  static class HallInteractionRouter
    {
        public static bool CanInteract(InteractionActor actor, InteractionType interactionType)
        {
            if (actor == InteractionActor.Guest)
            {
                return interactionType == InteractionType.Table;
            }

            return interactionType == InteractionType.Table || interactionType == InteractionType.Kitchen || interactionType == InteractionType.Trash;
        }
    }
}
