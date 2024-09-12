using Zenject;
using UnityEngine;

public class GameInstaller : MonoInstaller
{
    public GameObject ShelfPrefab;
    public GameObject SlotPrefab;
    public GameObject ItemPrefab;

    public override void InstallBindings()
    {
        if (ShelfPrefab == null || SlotPrefab == null || ItemPrefab == null)
        {
            Debug.LogError("Prefabs are not assigned in GameInstaller!");
            return;
        }

        Container.Bind<GameObject>().WithId("ShelfPrefab").FromInstance(ShelfPrefab).AsCached();
        Container.Bind<GameObject>().WithId("SlotPrefab").FromInstance(SlotPrefab).AsCached();
        Container.Bind<GameObject>().WithId("ItemPrefab").FromInstance(ItemPrefab).AsCached();

        Container.Bind<LevelManager>().FromComponentInHierarchy().AsSingle();
    }

}
