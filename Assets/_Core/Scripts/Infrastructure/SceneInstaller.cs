using PokeGo.World;
using UnityEngine.XR.ARFoundation;
using Zenject;

namespace PokeGo.Infrastructure
{
    public class SceneInstaller : MonoInstaller
    {
        public ARPlaneManager PlaneManager { get; set; }
        public PokeSpawner PokeSpawner { get; set; }

        public override void InstallBindings()
        {
            InstallPlaneManager();
            InstallPokeSpawner();
        }

        private void InstallPlaneManager()
        {
            Container.Bind<ARPlaneManager>()
                .FromComponentInHierarchy()
                .AsSingle();
        }

        private void InstallPokeSpawner()
        {
            Container.Bind<PokeSpawner>()
                .FromComponentInHierarchy()
                .AsSingle();
        }
    }
}