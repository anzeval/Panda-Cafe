// Defines upgrade purchase outcomes.
// Used by upgrade validation and shop flow.
// Separates success from purchase errors.
namespace PandaCafe.Core.Shop
{
    public enum UpgradePurchaseResult
    {
        Success,
        NoUpgradeSelected,
        InvalidUpgradeData,
        AlreadyPurchased,
        NotEnoughMoney
    }
}
