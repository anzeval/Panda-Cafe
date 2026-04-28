using System.Collections;
using PandaCafe.HallManagment;
using PandaCafe.Core;
using UnityEngine;
using System;

namespace PandaCafe.NPC
{
    // Responsible for spawning guest NPCs at a fixed interval
    // Uses GuestData to select guests and QueueManager to manage queue logic
    public class NPCSpawner : MonoBehaviour
    {
        [SerializeField] private float spawnRate = 5f;
        [SerializeField] private Transform quitPoint;

        private GuestData guestData;
        private QueueManager queueManager;

        public event Action<Guest> GuestAdded;

        // Initializes the spawner with required dependencies
        public void Init(GuestData guestData, QueueManager queueManager)
        {
            this.guestData = guestData;
            this.queueManager = queueManager;
        }

        public void RunSpawner()
        {
            StartCoroutine(SpawnRoutine());
        }

        // Continuously spawns guests while the game is in Playing state.
        // Respects queue capacity and spawn rate
        private IEnumerator SpawnRoutine()
        {
            while(GameManager.GameState == GameState.Playing)
            {
                if(queueManager.HasSlot()) SpawnGuest();
                    
                yield return new WaitForSeconds(spawnRate); 
            }
        }

        private void SpawnGuest()
        {
            GuestSO guestSO = guestData.GetRandomGuest();

            // Spawn random guest
            GameObject gameObject = Instantiate( guestSO.prefab, transform.position, Quaternion.identity, transform);
        
            // Ensure that guest has Guest component
            if(!gameObject.TryGetComponent<Guest>(out Guest guest))
            {
                guest = gameObject.AddComponent<Guest>();
            }

            // Register the guest
            queueManager.AddGuest(guest);
            GuestAdded?.Invoke(guest);

            // Set initial state
            guest.SetState(GuestState.GoingToQueue);
            guest.Init(guestSO, quitPoint);
        }
    }
}
