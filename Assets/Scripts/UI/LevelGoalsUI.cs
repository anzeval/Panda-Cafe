using TMPro;
using UnityEngine;

// Shows level goals intro panel.
// Displays target coins, guests, and losses.
// Can be toggled by level UI controller.
namespace PandaCafe.UI
{
   public class LevelGoalsUI : MonoBehaviour
    {
        [SerializeField] private GameObject childContainer;

        [SerializeField] private TMP_Text coinGoal;
        [SerializeField] private TMP_Text guestGoal;
        [SerializeField] private TMP_Text maxLostedGuests;

        // Set initial goal values
        public void SetInitialValues(int _coinGoal, int _servedGuestsGoal, int _maxLostGuests)
        {
            coinGoal.text = _coinGoal.ToString();
            guestGoal.text = _servedGuestsGoal.ToString();
            maxLostedGuests.text = _maxLostGuests.ToString();
        }

        // Toggle UI visibility
        public void SetActive(bool isActive)
        {
            childContainer.SetActive(isActive);
        }
    }
}
