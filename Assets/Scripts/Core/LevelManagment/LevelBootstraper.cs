using UnityEngine;
using PandaCafe.AI;
using PandaCafe.Input;
using PandaCafe.Interaction;
using PandaCafe.HallManagment;
using PandaCafe.NPC;
using PandaCafe.Menu;
using PandaCafe.WaiterNPC;
using PandaCafe.Core.GameManagment;
using PandaCafe.Interaction.InteractionObjects;
using PandaCafe.UI;

// Wires level scene systems together.
// Passes dependencies between managers and UI.
// Starts the loaded level runtime.
namespace PandaCafe.Core.LevelManagment
{
    public class LevelBootstraper : MonoBehaviour
    {
        [SerializeField] LevelManager levelManager;

        [SerializeField] LevelUIController levelUIController;
        [SerializeField] LevelGoalsUI levelGoalsUI;
        [SerializeField] LevelHeaderUI levelHeaderUI;
        [SerializeField] CompletedLevelUI completedLevelUI;
        [SerializeField] PauseLevelUI pauseLevelUI;

        [SerializeField] LevelDatabase levelDatabase;
        [SerializeField] GoalManager goalManager;

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
        [SerializeField] ClockManager clockManager;

        [SerializeField] Waiter waiter;
        [SerializeField] Kitchen kitchen;

        private PathfindingManager pathfindingManager;

        // Initialize component references
        void Awake()
        {
            if (!levelManager.Init(levelDatabase))
            {
                enabled = false;
                return;
            }
            goalManager.Init(levelManager);
            clockManager.Init(levelManager, goalManager);
            levelUIController.Init(levelManager, goalManager, clockManager, levelHeaderUI, pauseLevelUI, completedLevelUI, levelGoalsUI);

            gridManager.Init(background);
            queueManager.Init(goalManager);

            pathfindingManager = new PathfindingManager();
            pathfindingManager.Init(gridManager);

            NPCMovement.Init(pathfindingManager);
            
            interactionManager.Init(inputHandler, hallManager);
            npcSpawner.Init(guestData, queueManager, clockManager, levelManager);
            hallManager.Init(queueManager, waiter, menuData, orderManager, kitchen, goalManager);
            kitchen.Init(orderManager);

            npcSpawner.RunSpawner();
        }
    }   
}
