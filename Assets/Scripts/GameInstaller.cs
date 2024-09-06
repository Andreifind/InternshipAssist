using Zenject;
using UnityEngine;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        // Install LevelManager as a Singleton, available across all scenes
        Container.Bind<LevelManager>().FromComponentInHierarchy().AsSingle();
    }
}
