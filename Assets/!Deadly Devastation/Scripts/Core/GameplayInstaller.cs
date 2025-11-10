using UnityEngine;
using Zenject;

namespace RKS.DD.Gameplay.Installers
{
    public class GameplayInstaller : MonoInstaller
    {
        [Header("Prefabs")]
        [SerializeField] private GameObject playerPrefab;

        public override void InstallBindings()
        {
            Container.BindFactory<PlayerContext, PlayerFactory>()
                     .FromComponentInNewPrefab(playerPrefab)
                     .AsSingle();
            Debug.Log("[GameplayInstaller] Scene dependencies installed.");
        }
    }
    public class PlayerFactory : PlaceholderFactory<PlayerContext> { }
}
