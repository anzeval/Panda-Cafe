using UnityEngine;

public class Waiter : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float speed;

    private Vector3 targetPos;
    private bool isMoving = false;

    private void Update()
    {
        if(!isMoving) return;

        HandleMovement();
    }

    private void LateUpdate()
    {
        spriteRenderer.sortingOrder = -(int)(transform.position.y * 100);
    }

    private void HandleMovement()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPos) < 0.05f)
        {
            transform.position = targetPos;
            isMoving = false;
        }
    }

    public void MoveTo(Vector3 target)
    {
        targetPos = target;
        isMoving = true;
    }
}
