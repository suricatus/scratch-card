using Gameplay;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class GameplayInstaller : MonoInstaller
    {
        [SerializeField] private PrizeAwardController prizeAwardController;

        public override void InstallBindings()
        {
            if (prizeAwardController != null)
            {
                Container.Bind<PrizeAwardController>()
                    .FromInstance(prizeAwardController)
                    .AsSingle();
            }
        }
    }
}
