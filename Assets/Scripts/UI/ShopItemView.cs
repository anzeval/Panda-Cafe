using UnityEngine;
using UnityEngine.UI;
using PandaCafe.Core.Shop;

// Controls one shop item button.
// Displays upgrade icon and selection state.
// Notifies shop manager when clicked.
namespace PandaCafe.UI
{
    public class ShopItemView : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Image selectionFrame;

        [SerializeField] private Button shopItemButton;

        public ShopUpgradeSO itemData { get; private set; }

        private ShopManager shopManager;

        // Initialize dependencies
        public void Init(ShopUpgradeSO itemData, ShopManager shopManager)
        {
            this.itemData = itemData;
            this.shopManager = shopManager;

            if (icon != null)
            {
                icon.sprite = itemData.icon;
            }

            SetSelected(false);

            if (shopItemButton != null)
            {
                shopItemButton.onClick.RemoveListener(OnClick);
                shopItemButton.onClick.AddListener(OnClick);
            }
        }

        // Clean up event subscriptions
        private void OnDestroy()
        {
            if (shopItemButton != null)
            {
                shopItemButton.onClick.RemoveListener(OnClick);
            }
        }

        // Handle item click
        private void OnClick()
        {
            shopManager?.SelectItem(this);
        }

        // Update selected visual
        public void SetSelected(bool selected)
        {
            if (selectionFrame != null)
            {
                selectionFrame.enabled = !selected;
            }
        }
    }
}
