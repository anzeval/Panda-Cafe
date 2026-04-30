using UnityEngine;
using PandaCafe.Interaction;

namespace PandaCafe.Animation
{
   public class TrashAnimation : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Trash trash;
        private static readonly int BaseLayer = 0;

        private void Awake()
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }

            if (trash == null)
            {
                trash = GetComponent<Trash>();
            }
        }

        private void OnEnable()
        {
            if (trash != null)
            {
                trash.StateChanged += HandleStateChanged;
            }
        }

        private void Start()
        {
            HandleStateChanged();
        }

        private void OnDisable()
        {
            if (trash != null)
            {
                trash.StateChanged -= HandleStateChanged;
            }
        }

        private void Update()
        {
            if (animator == null) return;

            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(BaseLayer);

            if (trash != null && trash.State == TrashState.Opening && stateInfo.IsName("Open") && stateInfo.normalizedTime >= 1f)
            {
                trash.SetState(TrashState.Opened);
            }
            else if (trash != null && trash.State == TrashState.Closing && stateInfo.IsName("Close") && stateInfo.normalizedTime >= 1f)
            {
                trash.SetState(TrashState.Closed);
            }
        }

        private void HandleStateChanged()
        {
            if (trash == null || animator == null) return;

            ApplyState(trash.State);
        }

        private void ApplyState(TrashState state)
        {
            if (animator == null) return;

            switch (state)
            {
                case TrashState.Opening:
                    animator.speed = 1f;
                    animator.Play("Open", BaseLayer, 0f);
                    break;

                case TrashState.Closing:
                    animator.speed = 1f;
                    animator.Play("Close", BaseLayer, 0f);
                    break;

                case TrashState.Opened:
                    animator.speed = 0f;
                    animator.Play("Open", BaseLayer, 1f);
                    animator.Update(0f);
                    break;

                case TrashState.Closed:
                    animator.speed = 0f;
                    animator.Play("Idle", BaseLayer, 1f);
                    animator.Update(0f);
                    break;
            }
        }
    } 
}