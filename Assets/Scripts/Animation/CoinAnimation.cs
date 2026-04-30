using UnityEngine;

public class CoinAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string floatingStateName = "Floating";

    private void Awake()
    {
        
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void OnEnable()
    {
        if (animator == null) return;

        animator.Play(floatingStateName, 0, 0f);
    }
}
