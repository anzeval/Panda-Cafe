using UnityEngine;

namespace PandaCafe.NPC
{
    // ScriptableObject that stores configuration data for a guest NPC
    // Defines appearance, patience, and tipping behavior
    [CreateAssetMenu(fileName = "GuestSO", menuName = "Scriptable Objects/GuestSO")]
    public class GuestSO : ScriptableObject
    {
        // Prefab of the guest NPC that will be spawned in the game
        public GameObject prefab;

        // Maximum time (in seconds) the guest is willing to wait before leaving
        // Represents the patience level of the NPC
        public float WaitTime;

        // Minimum amount of tips the guest can leave
        public float MinTips;

        // Maximum amount of tips the guest can leave
        public float MaxTips;
    }   
}