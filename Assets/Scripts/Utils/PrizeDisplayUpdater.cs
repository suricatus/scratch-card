using TMPro;
using Tools.PrizeManager.Models;
using Tools.PrizeManager.Services;
using UnityEngine;
using Zenject;

namespace Utils
{
    public class PrizeDisplayUpdater : MonoBehaviour
    {
        [SerializeField] private TMP_Text prizeCountText;
        
        [Inject] private IPrizeManagerService _prizeManagerService;

        private void OnEnable()
        {
            PrizeEvents.OnPrizeAwarded += UpdateDisplay;
            PrizeEvents.OnPrizeReset += UpdateDisplay;
            UpdateDisplay();
        }

        private void OnDisable()
        {
            PrizeEvents.OnPrizeAwarded -= UpdateDisplay;
            PrizeEvents.OnPrizeReset -= UpdateDisplay;
        }

        private void UpdateDisplay(Prize prize = null)
        {
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            if (prizeCountText == null || _prizeManagerService == null)
                return;

            var allPrizes = _prizeManagerService.GetAllPrizes();
            var totalPrizes = 0;
            var totalRemaining = 0;
            
            foreach (var prize in allPrizes)
            {
                totalPrizes += prize.TotalQuantity;
                totalRemaining += prize.RemainingQuantity;
            }
            
            prizeCountText.text = $"Prêmios Disponíveis: {totalRemaining}/{totalPrizes}";
        }
    }
}
