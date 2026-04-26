using UnityEngine;
using PandaCafe.AI;
using PandaCafe.Input;
using PandaCafe.Interaction;
using PandaCafe.Managers;
using PandaCafe.NPC;
using PandaCafe.Menu;

namespace PandaCafe.Core
{
    public class GameBootstraper : MonoBehaviour
    {
        [SerializeField] InputHandler inputHandler;

        [SerializeField] GuestData guestData;

        [SerializeField] QueueManager queueManager;

        [SerializeField] InteractionManager interactionManager;

        [SerializeField] HallManager hallManager;
        [SerializeField] MenuData menuData;
        [SerializeField] OrderManager orderManager;

        [SerializeField] GridManager gridManager;
        [SerializeField] SpriteRenderer background;

        [SerializeField] NPCSpawner npcSpawner;

        [SerializeField] Waiter waiter;

        private PathfindingManager pathfindingManager;

        void Awake()
        {
            gridManager.Init(background);

            pathfindingManager = new PathfindingManager();
            pathfindingManager.Init(gridManager);

            NPCMovement.Init(pathfindingManager);
            
            interactionManager.Init(inputHandler, hallManager);
            npcSpawner.Init(guestData, queueManager);
            hallManager.Init(queueManager, waiter, menuData, orderManager);

            npcSpawner.RunSpawner();
        }
    }   
}
