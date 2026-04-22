using UnityEngine;
using PandaCafe.NPC;
using PandaCafe.Interaction;
using PandaCafe.Managers;
using PandaCafe.AI;
using PandaCafe.Input;

public class GameBootstraper : MonoBehaviour
{
    [SerializeField] InputHandler inputHandler;

    [SerializeField] GuestData guestData;

    [SerializeField] QueueManager queueManager;
    [SerializeField] InteractionManager interactionManager;
    [SerializeField] HallManager hallManager;

    [SerializeField] GridManager gridManager;
    [SerializeField] PathfindingManager pathfindingManager;
    [SerializeField] SpriteRenderer background;

    [SerializeField] NPCSpawner npcSpawner;

    [SerializeField] Waiter waiter;

    void Awake()
    {
        gridManager.Init(background);
        pathfindingManager.Init(gridManager);
        
        interactionManager.Init(inputHandler, hallManager);
        npcSpawner.Init(guestData, queueManager);
        hallManager.Init(queueManager, waiter);

        npcSpawner.RunSpawner();
    }
}
