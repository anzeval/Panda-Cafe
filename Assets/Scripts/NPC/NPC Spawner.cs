using System.Collections;
using PandaCafe.HallManagment;
using PandaCafe.Core.LevelManagment;
using UnityEngine;

// Spawns guest NPCs during the level.
// Uses guest data and queue capacity.
// Stops spawning when cafe time is over.
namespace PandaCafe.NPC
{
    public class NPCSpawner : MonoBehaviour
    {
        [SerializeField] private float spawnRate = 5f;
        [SerializeField] private Transform quitPoint;

        private GuestData guestData;
        private QueueManager queueManager;
        private ClockManager clockManager;
        private LevelManager levelManager;

        // Initializes the spawner with required dependencies
        public void Init(GuestData guestData, QueueManager queueManager, ClockManager clockManager, LevelManager levelManager)
        {
            this.guestData = guestData;
            this.queueManager = queueManager;
            this.clockManager = clockManager;
            this.levelManager = levelManager;
        }

        // Start spawning guests
        public void RunSpawner()
        {
            StartCoroutine(SpawnRoutine());
        }

        // Continuously spawns guests while the game is in Playing state.
        // Respects queue capacity and spawn rate
        private IEnumerator SpawnRoutine()
        {
            while(levelManager.LevelState == LevelState.Playing)
            {
                if (clockManager != null && !clockManager.IsCafeOpen)
                {
                    yield return new WaitForSeconds(spawnRate);
                    continue;
                }
                if(queueManager.HasSlot()) SpawnGuest();
                    
                yield return new WaitForSeconds(spawnRate); 
            }
        }

        // Create and queue a guest
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

            // Set initial state
            guest.SetState(GuestState.GoingToQueue);
            guest.Init(guestSO, quitPoint);
        }
    }
}
