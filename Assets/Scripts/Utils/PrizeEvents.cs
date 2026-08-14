using System;
using Tools.PrizeManager.Models;

namespace Utils
{
    public static class PrizeEvents
    {
        public static event Action<Prize> OnPrizeAwarded;
        public static event Action OnPrizeReset;
        public static event Action OnPrizeQuantityUpdated;

        public static void TriggerPrizeAwarded(Prize prize)
        {
            OnPrizeAwarded?.Invoke(prize);
        }

        public static void TriggerPrizeReset()
        {
            OnPrizeReset?.Invoke();
        }

        public static void TriggerPrizeQuantityUpdated()
        {
            OnPrizeQuantityUpdated?.Invoke();
        }
    }
}