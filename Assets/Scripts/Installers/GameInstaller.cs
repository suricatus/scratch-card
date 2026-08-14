using Tools.PrizeManager.Installers;
using Tools.PrizeManager.Models;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private PrizeConfig prizeConfig;

        public override void InstallBindings()
        {
            var prizeInstaller = gameObject.AddComponent<PrizeManagerInstaller>();
            prizeInstaller.GetType()
                .GetField("config", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(prizeInstaller, prizeConfig);
            
            prizeInstaller.InstallBindings();
        }
    }
}