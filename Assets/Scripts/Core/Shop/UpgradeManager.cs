using System;
using System.Collections.Generic;
using UnityEngine;
using PandaCafe.Core.GameManagment;

// Manages purchased shop upgrades.
// Validates purchases and spends player money.
// Stores purchased upgrades by id.
// Rebuilds gameplay multipliers from bought upgrades.
namespace PandaCafe.Core.Shop
{
    public class UpgradeManager : MonoBehaviour
    {
        private static UpgradeManager instance;

        public static bool HasInstance => instance != null;

        public static UpgradeManager Instance
        {
            get
            {
                // Create manager on demand if scene does not contain one
                if (instance == null)
                {
                    instance = FindObjectOfType<UpgradeManager>();

                    if (instance == null)
                    {
                        GameObject managerObject = new GameObject(nameof(UpgradeManager));
                        instance = managerObject.AddComponent<UpgradeManager>();
                    }
                }

                return instance;
            }
            private set
            {
                instance = value;
            }
        }

        public event Action<ShopUpgradeSO> UpgradePurchased;

        private readonly Dictionary<string, ShopUpgradeSO> purchasedUpgrades = new Dictionary<string, ShopUpgradeSO>();
        private readonly Dictionary<UpgradeType, float> multipliers = new Dictionary<UpgradeType, float>();

        // Initialize component references
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // Try buy upgrade
        public UpgradePurchaseResult TryPurchaseUpgrade(ShopUpgradeSO upgrade)
        {
            UpgradePurchaseResult validationResult = ValidateUpgradeForPurchase(upgrade);
            if (validationResult != UpgradePurchaseResult.Success) return validationResult;
            if (!GameManager.CanAfford(upgrade.price)) return UpgradePurchaseResult.NotEnoughMoney;
            if (!GameManager.TrySpendMoney(upgrade.price)) return UpgradePurchaseResult.NotEnoughMoney;

            return CompleteUpgradePurchase(upgrade);
        }

        // Apply upgrade without payment
        public UpgradePurchaseResult ApplyUpgrade(ShopUpgradeSO upgrade)
        {
            UpgradePurchaseResult validationResult = ValidateUpgradeForPurchase(upgrade);
            if (validationResult != UpgradePurchaseResult.Success) return validationResult;

            return CompleteUpgradePurchase(upgrade);
        }

        // Check purchased upgrade
        public bool IsPurchased(ShopUpgradeSO upgrade)
        {
            return upgrade != null && IsUpgradeDataValid(upgrade) && purchasedUpgrades.ContainsKey(upgrade.id);
        }

        // Get purchased upgrade level
        public int GetUpgradeLevel(ShopUpgradeSO upgrade)
        {
            if (upgrade == null || !IsUpgradeDataValid(upgrade)) return 0;

            return purchasedUpgrades.ContainsKey(upgrade.id) ? 1 : 0;
        }

        // Get upgrade multiplier
        public float GetMultiplier(UpgradeType type)
        {
            return multipliers.TryGetValue(type, out float value) ? value : 1f;
        }

        // Return purchased upgrades
        public IReadOnlyDictionary<string, ShopUpgradeSO> GetPurchasedUpgrades()
        {
            return purchasedUpgrades;
        }

        // Validate upgrade purchase
        private UpgradePurchaseResult ValidateUpgradeForPurchase(ShopUpgradeSO upgrade)
        {
            if (upgrade == null) return UpgradePurchaseResult.NoUpgradeSelected;
            if (!IsUpgradeDataValid(upgrade)) return UpgradePurchaseResult.InvalidUpgradeData;
            if (IsPurchased(upgrade)) return UpgradePurchaseResult.AlreadyPurchased;

            return UpgradePurchaseResult.Success;
        }

        // Store purchased upgrade
        private void RegisterPurchasedUpgrade(ShopUpgradeSO upgrade)
        {
            purchasedUpgrades.Add(upgrade.id, upgrade);
            RebuildMultipliers();
        }

        // Finish purchase flow
        private UpgradePurchaseResult CompleteUpgradePurchase(ShopUpgradeSO upgrade)
        {
            RegisterPurchasedUpgrade(upgrade);
            UpgradePurchased?.Invoke(upgrade);

            return UpgradePurchaseResult.Success;
        }

        // Recalculate upgrade multipliers
        private void RebuildMultipliers()
        {
            multipliers.Clear();

            // Multipliers of the same type stack by multiplication
            foreach (ShopUpgradeSO upgrade in purchasedUpgrades.Values)
            {
                multipliers[upgrade.type] = GetMultiplier(upgrade.type) * upgrade.value;
            }
        }

        // Check upgrade data validity
        private static bool IsUpgradeDataValid(ShopUpgradeSO upgrade)
        {
            return !string.IsNullOrWhiteSpace(upgrade.id) && upgrade.price >= 0;
        }
    }
}
