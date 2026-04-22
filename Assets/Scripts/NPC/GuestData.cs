using System.Collections.Generic;
using UnityEngine;

namespace PandaCafe.NPC
{   
    // Stores and provides access to available guest types
    // Allows retrieving specific or random guest configurations
    public class GuestData : MonoBehaviour
    {
        [SerializeField] private List<GuestSO> guestsTypes = new List<GuestSO>();

        private System.Random rdm = new System.Random();

        // Returns a guest by index if the list is valid and index is within bounds
        public GuestSO TryGetGuest(int index)
        {
            if(!IsArrayValid(guestsTypes) || index < 0 || index >= guestsTypes.Count) return null;

            return guestsTypes[index];
        }

        // Returns a random guest from the list
        public GuestSO GetRandomGuest()
        {
            if(!IsArrayValid(guestsTypes)) return null;

            return guestsTypes[rdm.Next(0, guestsTypes.Count)];
        }

        // Checks if the list exists and contains at least one element
        private bool IsArrayValid(List<GuestSO> array)
        {
            return array != null && array.Count > 0;
        }
    }
}