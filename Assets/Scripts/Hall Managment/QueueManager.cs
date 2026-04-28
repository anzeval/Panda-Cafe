using UnityEngine;
using PandaCafe.NPC;

namespace PandaCafe.HallManagment
{
    // Manages guest queue
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

        // Add guest to first free slot
        public void AddGuest(Guest guest)
        {
            if (guestsQueue == null || guest == null) return;

            for (int i = 0; i < guestsQueue.Length; i++)
            {
                if (guestsQueue[i] == null)
                {
                    guestsQueue[i] = guest;

                    // Subscribe to patience event
                    guest.PatienceExpired += OnGuestPatienceExpired;

                    guest.SetOrdinalQueueNumber(i);
                    guest.MoveTo(GetQueueWorldPosition(i));
                    break;
                }
            }
        }

        // Calculate position for queue slot
        private Vector3 GetQueueWorldPosition(int index)
        {
            float x = startPoint.position.x - distance * index;
            float y = startPoint.position.y + (float)rdm.NextDouble();

            return new Vector3(x, y, startPoint.position.z);
        }

        // Check if queue has free slot
        public bool HasSlot()
        {
            if (guestsQueue == null) return false;

            for (int i = 0; i < guestsQueue.Length; i++)
            {
                if (guestsQueue[i] == null) return true;
            }

            return false;
        }

        // Get guest by index
        public Guest GetGuest(int index)
        {
            if (guestsQueue != null && guestsQueue.Length > 0) return guestsQueue[index];
            return null;
        }

        // Remove guest by index
        public void RemoveGuestFromQueue(int index)
        {
            if (guestsQueue == null || index < 0 || index >= guestsQueue.Length) return;
            if (guestsQueue[index] == null) return;

            // Unsubscribe from event
            guestsQueue[index].PatienceExpired -= OnGuestPatienceExpired;

            guestsQueue[index].SetOrdinalQueueNumber(-1);
            guestsQueue[index] = null;
        }

        // Remove guest by reference
        public void RemoveGuestFromQueue(Guest guest)
        {
            if (guestsQueue == null || guest == null) return;

            for (int i = 0; i < guestsQueue.Length; i++)
            {
                if (guestsQueue[i] != guest) continue;

                RemoveGuestFromQueue(i);
                return;
            }
        }

        // Shift queue forward after removal
        public void ReorderQueue()
        {
            if (guestsQueue == null) return;

            for (int i = 0; i < guestsQueue.Length; i++)
            {
                if (guestsQueue[i] == null)
                {
                    for (int j = i; j < guestsQueue.Length; j++)
                    {
                        if (guestsQueue[j] == null) continue;

                        guestsQueue[i] = guestsQueue[j];
                        guestsQueue[i].SetOrdinalQueueNumber(i);

                        guestsQueue[j] = null;

                        // Move guest to new position
                        guestsQueue[i].MoveTo(GetQueueWorldPosition(i));
                        break;
                    }
                } 
            }
        }

        // Handle guest leaving due to no patience
        private void OnGuestPatienceExpired(Guest guest)
        {
            if (guest == null || guest.OrdinalQueueNumber < 0) return;

            RemoveGuestFromQueue(guest);
            ReorderQueue();
        }
    }
}