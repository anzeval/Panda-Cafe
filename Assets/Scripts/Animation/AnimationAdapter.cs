using PandaCafe.NPC;

namespace PandaCafe.Animation
{
    public class AnimationAdapter
    {
        private NPCSpawner npcSpawner;

        public AnimationAdapter(NPCSpawner npcSpawner)
        {
            this.npcSpawner = npcSpawner;

            if(npcSpawner != null)
            {
                npcSpawner.GuestAdded += SubscribeGuest;
            }
        }

        private void OnDestroy() 
        {
            if(npcSpawner != null)
            {
                npcSpawner.GuestAdded -= SubscribeGuest;
            }    
        }

        private void SubscribeGuest(Guest guest)
        {
            guest.StateChanged += ChangeState;
            guest.GuestRemoved += UnsubscribeGuest;
        }

        private void UnsubscribeGuest(Guest guest)
        {
            guest.StateChanged -= ChangeState;
            guest.GuestRemoved -= UnsubscribeGuest;
        }

        private void ChangeState(GuestState oldState, GuestState newState)
        {
            
        }
    }
}

