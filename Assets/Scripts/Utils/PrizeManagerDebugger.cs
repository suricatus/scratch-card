using Tools.PrizeManager.Services;
using UnityEngine;
using Zenject;

namespace Utils
{
    public class PrizeManagerDebugger : MonoBehaviour
    {
        [Inject] private IPrizeManagerService _prizeManagerService;

        private void Start()
        {
            LogPrizeStatus();
        }

        [ContextMenu("Log Prize Status")]
        public void LogPrizeStatus()
        {
            if (_prizeManagerService == null)
            {
                Debug.LogError("[PrizeDebugger] PrizeManagerService is NULL!");
                return;
            }

            var allPrizes = _prizeManagerService.GetAllPrizes();
            Debug.Log($"[PrizeDebugger] Total Prize Types: {allPrizes.Count}");

            foreach (var prize in allPrizes)
            {
                Debug.Log($"[PrizeDebugger] {prize.PrizeName} - " +
                         $"Total: {prize.TotalQuantity}, " +
                         $"Awarded: {prize.AwardedQuantity}, " +
                         $"Remaining: {prize.RemainingQuantity}");
            }

            CheckPlayerPrefs();
        }

        [ContextMenu("Check PlayerPrefs")]
        private void CheckPlayerPrefs()
        {
            var prizeData = PlayerPrefs.GetString("PrizeManager_Data", "NOT FOUND");
            Debug.Log($"[PrizeDebugger] PlayerPrefs Data: {prizeData}");
        }

        [ContextMenu("Clear Prize Data")]
        public void ClearPrizeData()
        {
            PlayerPrefs.DeleteKey("PrizeManager_Data");
            PlayerPrefs.Save();
            Debug.Log("[PrizeDebugger] Cleared prize data from PlayerPrefs");
        }
    }
}
