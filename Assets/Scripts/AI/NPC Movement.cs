using UnityEngine;

namespace PandaCafe.AI
{
    public class NPCMovement : MonoBehaviour
    {
        private Vector3 target;

        public void SetTarget(Vector3 target)
        {
            this.target = target;
        }

        private void RequestPath()
        {
            
        }
    }
}