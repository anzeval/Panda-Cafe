using UnityEngine;
using PandaCafe.NPC;

namespace PandaCafe.Managers
{
    // Manages a queue of guests: stores their order, controls capacity, and assigns world positions where each guest should stand
    public class QueueManager : MonoBehaviour
    {
        [SerializeField] Transform startPoint;
        [SerializeField] int queueCapacity = 4;
        [SerializeField] int distance = 1;

        private Guest[] guestsQueue;
        private System.Random rdm = new System.Random();

        void Awake()
        {
            guestsQueue = new Guest[queueCapacity];
        }

        // Adds a guest to the first available slot in the queue
        // Assigns their queue index (ordinalQueueNumber) and moves them to the correct position using MoveTo
        public void AddGuest(Guest guest)
        {
            if(guestsQueue == null) return;

            for (int i = 0; i < guestsQueue.Length; i++)
            {
                if(guestsQueue[i] == null)
                {
                    guestsQueue[i] = guest;
                    guest.SetOrdinalQueueNumber(i);
                    guest.MoveTo(GetQueueWorldPosition(i));

                    break;
                }
            }
        }

        // Calculates the world position for a guest based on their index in the queue
        // Each next guest stands further from the startPoint by a fixed distance, with a small random Y offset
        private Vector3 GetQueueWorldPosition(int index)
        {
            float x = startPoint.position.x - distance * index;
            float y = startPoint.position.y  + (float)rdm.NextDouble();

            return new Vector3(x, y, startPoint.position.z);
        }

        // Checks if there is any free space in the queue
        // Returns true if at least one slot is empty
        public bool HasSlot()
        {
            if(guestsQueue == null) return false;

            for (int i = 0; i < guestsQueue.Length; i++)
            {
                if(guestsQueue[i] == null) return true;
            }

            return false;
        }

        // Returns the guest at the specified index in the queue.
        // Used to access information about a specific guest.
        public Guest GetGuest(int index)
        {
            if(guestsQueue != null && guestsQueue.Length > 0) return guestsQueue[index];
            return null;
        }

        // Removes a guest from the queue by index.
        // Resets their queue number and frees the slot.
        public void RemoveGuestFromQueue(int index)
        {
            guestsQueue[index].SetOrdinalQueueNumber(-1);
            guestsQueue[index] = null;
        }

        // Reorganizes the queue after a guest is removed.
        // Shifts all guests forward to fill empty slots, updates their indices, and moves them to new positions.
        public void ReorderQueue()
        {
            for (int i = 0; i < guestsQueue.Length; i++)
            {
                if(guestsQueue[i] == null)
                {
                    for (int j = i; j < guestsQueue.Length; j++)
                    {
                        if(guestsQueue[j] == null) continue;

                        guestsQueue[i] = guestsQueue[j];
                        guestsQueue[i].SetOrdinalQueueNumber(i);

                        guestsQueue[j] = null;

                        guestsQueue[i].MoveTo(GetQueueWorldPosition(i));
                        break;
                    }
                } 
            }
        }
    }
}

