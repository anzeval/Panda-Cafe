using UnityEngine;

namespace PandaCafe.Interaction
{
    // Defines a contract for objects that can be interacted with
    public interface IInteractable
    {
        //Type of the object
        InteractionType Type {get;}
        //The position in the world 
        bool TryGetWorldPoint(InteractionActor actor, out Vector3 point);
    }
}

