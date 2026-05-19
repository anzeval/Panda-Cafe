using UnityEngine;

// Controls coin visual setup.
// Sets render order for coin sprites.
// Keeps coin animation component isolated.
namespace PandaCafe.Animation
{
    public class CoinAnimation : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private string floatingStateName = "Floating";

        // Initialize component references
        private void Awake()
        {
            
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }
        }

        // Subscribe events
        private void OnEnable()
        {
            if (animator == null) return;

            animator.Play(floatingStateName, 0, 0f);
        }
    }   
}