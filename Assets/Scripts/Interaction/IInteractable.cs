using UnityEngine;

// Defines interactable object contract.
// Exposes interaction type and world point.
// Used by input and hall interaction flow.
namespace PandaCafe.Interaction
{
    public interface IInteractable
    {
        //Type of the object
        InteractionType Type {get;}
        //The position in the world 
        bool TryGetWorldPoint(InteractionActor actor, out Vector3 point);

    }
}

