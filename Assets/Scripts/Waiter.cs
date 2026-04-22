using UnityEngine;
using PandaCafe.AI;

public class Waiter : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    private NPCMovement movement;

    private void Awake()
    {
        if (movement == null)
        {
            movement = GetComponent<NPCMovement>();

            if (movement == null)
            {
                movement = gameObject.AddComponent<NPCMovement>();
            }
        }
    }

    private void LateUpdate()
    {
        spriteRenderer.sortingOrder = -(int)(transform.position.y * 100);
    }

    public void MoveTo(Vector3 target)
    {
        movement.SetTarget(target);
    }
}
