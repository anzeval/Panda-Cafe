using UnityEngine;
using System.Collections.Generic;
using PandaCafe.Core.LevelManagment;

// Stores available level data entries.
// Finds levels by index or default order.
// Unlocks the next level after completion.
namespace PandaCafe.Core.GameManagment
{
    public class LevelDatabase : MonoBehaviour
{
    [SerializeField] private List<LevelDataSO> levelsData = new List<LevelDataSO>();

    // Get level data by index
    public bool TryGetLevelData(int levelIndex, out LevelDataSO levelDatabaseSO)
    {
        levelDatabaseSO = default;

        if (!IsArrayValid(levelsData)) return false;

        foreach (LevelDataSO levelData in levelsData)
        {
            if (levelData != null && levelData.levelIndex == levelIndex)
            {
                levelDatabaseSO = levelData;
                return true;
            }
        }

        return false;
    }

    // Get first available level
    public bool TryGetFirstLevelData(out LevelDataSO levelDatabaseSO)
    {
        levelDatabaseSO = default;

        if (!IsArrayValid(levelsData))
        {
            return false;
        }

        foreach (LevelDataSO levelData in levelsData)
        {
            if (levelData != null)
            {
                levelDatabaseSO = levelData;
                return true;
            }
        }

        return false;
    }

     // Unlock next level after completion
     public bool TryUnlockNextLevel(LevelDataSO completedLevel)
    {
        if (completedLevel == null) return false;
        if (GameManager.GetLevelMaxEarnedStars(completedLevel) <= 0) return false;
        if (!TryGetLevelData(completedLevel.levelIndex + 1, out LevelDataSO nextLevel)) return false;

        GameManager.UnlockLevel(nextLevel);
        return true;
    }

    // Check list validity
    private bool IsArrayValid(List<LevelDataSO> array)
    {
        return array != null && array.Count > 0;
    }
}
}
