using UnityEngine;

// Represents collectible payout coin.
// Sets coin render order in the scene.
// Destroys coin after collection.
namespace PandaCafe.Environment
{
    public class Coin : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer[] renderers;

        private int[] sortingOffsets;

        // Initialize component references
        private void Awake()
        {
            CacheSortingOffsets();
            UpdateSortingOrder(-(int)(transform.position.y * 100));
        }

        // Set coin render order
        public void SetSortingOrder(int sortingOrder)
        {
            UpdateSortingOrder(sortingOrder);
        }

        // Apply sorting order to all renderers
        private void UpdateSortingOrder(int sortingOrder)
        {
            if (renderers == null || renderers.Length <= 0) return;

            for (int i = 0; i < renderers.Length; i++)
            {
                SpriteRenderer spriteRenderer = renderers[i];
                if (spriteRenderer == null) continue;

                int offset = sortingOffsets != null && i < sortingOffsets.Length ? sortingOffsets[i] : i;
                spriteRenderer.sortingOrder = sortingOrder + offset;
            }
        }

        // Preserve the prefab's front-to-back sprite order inside the coin.
        private void CacheSortingOffsets()
        {
            if (renderers == null || renderers.Length <= 0) return;

            sortingOffsets = new int[renderers.Length];

            for (int i = 0; i < renderers.Length; i++)
            {
                SpriteRenderer currentRenderer = renderers[i];
                if (currentRenderer == null)
                {
                    sortingOffsets[i] = i;
                    continue;
                }

                int offset = 0;

                for (int j = 0; j < renderers.Length; j++)
                {
                    SpriteRenderer otherRenderer = renderers[j];
                    if (otherRenderer == null) continue;

                    if (otherRenderer.sortingOrder < currentRenderer.sortingOrder)
                    {
                        offset++;
                    }
                }

                sortingOffsets[i] = offset;
            }
        }

        // Destroy collected coin
        public void Collect()
        {
            Destroy(gameObject);
        }
    } 
}
