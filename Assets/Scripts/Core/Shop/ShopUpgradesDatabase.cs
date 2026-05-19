using UnityEngine;
using System.Collections.Generic;

// Stores shop upgrade entries.
// Provides upgrade data by shop slot index.
// Keeps shop UI independent from asset list details.
namespace PandaCafe.Core.Shop
{
    public class ShopUpgradesDatabase : MonoBehaviour
    {
        [SerializeField] private List<ShopUpgradeSO> upgradesData = new List<ShopUpgradeSO>();

        // Get upgrade by slot index
        public bool TryGetUpgradeData(int itemIndex, out ShopUpgradeSO upgradeData)
        {
            upgradeData = default;

            if (upgradesData == null || itemIndex < 0 || itemIndex >= upgradesData.Count) return false;

            upgradeData = upgradesData[itemIndex];
            return upgradeData != null;
        }
    }   
}
