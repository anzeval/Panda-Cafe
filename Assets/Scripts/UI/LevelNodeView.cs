using PandaCafe.Core.GameManagment;
using PandaCafe.Core.LevelManagment;
using UnityEngine;
using UnityEngine.UI;

// Controls one level map node.
// Shows lock state and earned stars.
// Loads selected level when clicked.
namespace PandaCafe.UI
{
    public class LevelNodeView : MonoBehaviour
    {
        [SerializeField] private LevelDataSO levelDataSO;
        [SerializeField] private Button levelBtn;
        [SerializeField] private Image lockImg;
        [SerializeField] private Image[] stars;

        // Initialize component references
        private void Awake()
        {
            if (levelBtn == null)
            {
                levelBtn = GetComponent<Button>();
            }

            RefreshView();
        }

        // Subscribe events
        private void OnEnable()
        {
            if (levelBtn != null)
            {
                levelBtn.onClick.AddListener(LoadLevel);
            }

            RefreshView();
        }

        // Unsubscribe events
        private void OnDisable()
        {
            if (levelBtn != null)
            {
                levelBtn.onClick.RemoveListener(LoadLevel);
            }
        }

        // Load selected level
        private void LoadLevel()
        {
            if (levelDataSO == null || !GameManager.IsLevelUnlocked(levelDataSO)) return;
            
            GameManager.LoadLevel(levelDataSO);
        }

         // Update level stars
         private void RefreshStars(bool isUnlocked)
        {
            if (stars == null) return;

            int earnedStars = isUnlocked && levelDataSO != null ? GameManager.GetLevelMaxEarnedStars(levelDataSO) : 0;

            for (int i = 0; i < stars.Length; i++)
            {
                if (stars[i] == null) continue;

                stars[i].enabled = i < earnedStars;
            }
        }

        // Refresh level node state
        private void RefreshView()
        {
            bool isUnlocked = levelDataSO != null && GameManager.IsLevelUnlocked(levelDataSO);

            if (levelBtn != null)
            {
                levelBtn.interactable = isUnlocked;
            }

            if (lockImg != null)
            {
                lockImg.enabled = !isUnlocked;
            }

            RefreshStars(isUnlocked);
        }
    }
}
