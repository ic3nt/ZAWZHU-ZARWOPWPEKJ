using RKS.DD.Game.Elevator;
using UnityEngine;
using Zenject;

namespace RKS.DD.Core.Installers
{
    public class RoundInstaller : MonoInstaller
    {
        [Header("Managers")]
        [SerializeField] private ElevatorController elevatorController;
        public override void InstallBindings()
        {
            Debug.Log("[RoundInstaller] Installing round managers...");

            Container.Bind<ElevatorController>().FromComponentInNewPrefab(elevatorController).AsSingle().NonLazy();
        }

    }
}
