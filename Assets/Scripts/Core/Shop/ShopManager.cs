using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PandaCafe.Core.GameManagment;
using PandaCafe.UI;

// Controls shop screen state and selected upgrade.
// Fills item views from the upgrades database.
// Updates balance, item details, and purchase buttons.
// Handles navigation back to the level map.
namespace PandaCafe.Core.Shop
{
    public class ShopManager : MonoBehaviour
    {
        [SerializeField] private ShopItemView[] itemViews;
        [SerializeField] private ShopUpgradesDatabase upgradesDatabase;
         
        [SerializeField] private TMP_Text balance;

        [SerializeField] private TMP_Text itemName;
        [SerializeField] private TMP_Text itemDescription;
        [SerializeField] private TMP_Text itemPrice;

        [SerializeField] private Button buyBTN;
        [SerializeField] private Button backToLevelMapBTN;

        private ShopItemView currentSelected;

        // Initialize component references
        private void Awake()
        {
            if (itemViews != null && upgradesDatabase != null)
            {
                // Bind database entries to visible shop slots
                for (int i = 0; i < itemViews.Length; i++)
                {
                    ShopItemView itemView = itemViews[i];

                    if (itemView == null) continue;

                    if (upgradesDatabase.TryGetUpgradeData(i, out ShopUpgradeSO upgradeData))
                    {
                        itemView.Init(upgradeData, this);
                    }
                }
            }
        }

        // Subscribe events
        private void OnEnable()
        {
            ConfigureRaycastTargets();
            EnsureSelectedItem();

            SetButtonListener(buyBTN, Buy, true);
            SetButtonListener(backToLevelMapBTN, LoadLevelMap, true);

            RefreshUI();
        }

        // Unsubscribe events
        private void OnDisable()
        {
            SetButtonListener(buyBTN, Buy, false);
            SetButtonListener(backToLevelMapBTN, LoadLevelMap, false);
        }

        // Refresh shop UI
        private void RefreshUI()
        {
            EnsureSelectedItem();
            UpdateBalanceUI();
            UpdateSelectedItemUI(currentSelected);
        }

        // Configure UI raycast targets
        private void ConfigureRaycastTargets()
        {
            // Keep clicks on buttons, not their text or panel graphics
            ConfigureButtonRaycasts(buyBTN);
            ConfigureButtonRaycasts(backToLevelMapBTN);

            SetTextRaycastTarget(balance, false);
            SetTextRaycastTarget(itemName, false);
            SetTextRaycastTarget(itemDescription, false);
            SetTextRaycastTarget(itemPrice, false);
        }

        // Configure button raycasts
        private void ConfigureButtonRaycasts(Button button)
        {
            DisableParentPanelRaycasts(button);
            DisableChildTextRaycasts(button);
        }

        // Disable text raycasts
        private void DisableChildTextRaycasts(Button button)
        {
            if (button == null) return;

            TMP_Text[] texts = button.GetComponentsInChildren<TMP_Text>(true);
            foreach (TMP_Text text in texts)
            {
                SetTextRaycastTarget(text, false);
            }
        }

        // Disable panel raycasts
        private void DisableParentPanelRaycasts(Button button)
        {
            if (button == null) return;

            Transform current = button.transform.parent;
            while (current != null && current.GetComponent<Canvas>() == null)
            {
                if (current.GetComponent<Button>() == null && current.TryGetComponent(out Graphic graphic))
                {
                    graphic.raycastTarget = false;
                }

                current = current.parent;
            }
        }

        // Set text raycast flag
        private void SetTextRaycastTarget(TMP_Text text, bool value)
        {
            if (text != null)
            {
                text.raycastTarget = value;
            }
        }

        // Update balance text
        private void UpdateBalanceUI()
        {
            if (balance != null)
            {
                balance.text = GameManager.TotalBalance.ToString();
            }
        }

        // Buy selected upgrade
        private void Buy()
        {
            ShopUpgradeSO selectedUpgrade = GetSelectedUpgrade();
            if (selectedUpgrade == null)
            {
                RefreshUI();
                return;
            }

            UpgradeManager.Instance.TryPurchaseUpgrade(selectedUpgrade);
            RefreshUI();
        }

        // Buy from UI button
        public void BuyFromButton()
        {
            Buy();
        }

        // Load level map scene
        private void LoadLevelMap()
        {
            GameManager.LoadLevelMap();
        }

        // Select shop item
        public void SelectItem(ShopItemView itemView)
        {
            if (itemView == null) return;

            if (currentSelected != null)
            {
                currentSelected.SetSelected(false);
            }

            currentSelected = itemView;
            currentSelected.SetSelected(true);

            UpdateSelectedItemUI(itemView);
        }

        // Update selected item details
        private void UpdateSelectedItemUI(ShopItemView itemView)
        {
            if (itemView == null || itemView.itemData == null)
            {
                if (buyBTN != null)
                {
                    buyBTN.gameObject.SetActive(false);
                }

                return;
            }

            ShopUpgradeSO itemData = itemView.itemData;

            if (itemName != null)
            {
                itemName.text = itemData.itemName;
            }

            if (itemDescription != null)
            {
                itemDescription.text = itemData.description;
            }

            if (itemPrice != null)
            {
                itemPrice.text = itemData.price.ToString();
            }

            if (buyBTN != null)
            {
                bool isPurchased = UpgradeManager.Instance.IsPurchased(itemData);
                buyBTN.gameObject.SetActive(!isPurchased);
                buyBTN.interactable = !isPurchased && GameManager.CanAfford(itemData.price);
            }
        }

        // Ensure item is selected
        private void EnsureSelectedItem()
        {
            if (currentSelected != null && currentSelected.itemData != null) return;
            if (itemViews == null) return;

            ShopItemView fallbackItemView = null;

            // Prefer first unpurchased item, otherwise keep any valid item selected
            foreach (ShopItemView itemView in itemViews)
            {
                if (itemView == null || itemView.itemData == null) continue;

                if (fallbackItemView == null)
                {
                    fallbackItemView = itemView;
                }

                if (!UpgradeManager.Instance.IsPurchased(itemView.itemData))
                {
                    SelectItem(itemView);
                    return;
                }
            }

            if (fallbackItemView != null)
            {
                SelectItem(fallbackItemView);
            }
        }

        // Get selected upgrade data
        private ShopUpgradeSO GetSelectedUpgrade()
        {
            return currentSelected != null ? currentSelected.itemData : null;
        }

        // Add or remove button listener
        private void SetButtonListener(Button button, UnityEngine.Events.UnityAction action, bool enabled)
        {
            if (button == null) return;

            button.onClick.RemoveListener(action);

            if (enabled)
            {
                button.onClick.AddListener(action);
            }
        }
    }
}
