using Tools.PrizeManager.Models;
using Tools.PrizeManager.Services;
using UnityEngine;
using Utils;
using Zenject;

namespace Gameplay
{
    public class PrizeAwardController : MonoBehaviour
    {
        [Inject] private IPrizeManagerService _prizeManagerService;

        public async void AwardRandomPrize()
        {
            var availablePrizes = _prizeManagerService.GetAllPrizes();
            var validPrizes = availablePrizes.FindAll(p => p.IsAvailable);

            if (validPrizes.Count == 0)
                return;
            
            var randomPrize = validPrizes[Random.Range(0, validPrizes.Count)];
            var result = await _prizeManagerService.AwardPrizeAsync(randomPrize.Id);

            if (result.Success)
            {
                OnPrizeAwared(result.Prize);
                PrizeEvents.TriggerPrizeAwarded(result.Prize);
            }
        }

        public async void AwardSpecificPrize(string prizeId)
        {
            var result = await _prizeManagerService.AwardPrizeAsync(prizeId);

            if (result.Success)
            {
                OnPrizeAwared(result.Prize);
                PrizeEvents.TriggerPrizeAwarded(result.Prize);
            }
        }

        private void OnPrizeAwared(Prize prize)
        {
            PlayerPrefs.SetString("LastPrizeId", prize.Id);
            PlayerPrefs.SetString("LastPrizeName", prize.PrizeName);
            PlayerPrefs.Save();
        }

        public Prize GetLastAwardedPrize()
        {
            var prizeId = PlayerPrefs.GetString("LastPrizeId");
            if (string.IsNullOrEmpty(prizeId))
                return null;
            
            return _prizeManagerService.GetPrize(prizeId);
        }
    }
}