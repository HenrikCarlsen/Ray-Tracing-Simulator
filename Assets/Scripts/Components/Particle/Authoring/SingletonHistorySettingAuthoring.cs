using Unity.Entities;

using UnityEngine;

public class SingletonHistorySettingAuthoring : UnityEngine.MonoBehaviour
{
    public GameObject path;
    public GameObject point;
}

public struct SingletonHistorySetting : IComponentData
{
    public Entity path;
    public Entity point;
}

public class SingletonHistorySettingBaker : Baker<SingletonHistorySettingAuthoring>
{
    public override void Bake(SingletonHistorySettingAuthoring authoring)
    {
        AddComponent<SingletonHistorySetting>(
            new SingletonHistorySetting
            {
                path = GetEntity(authoring.path),
                point = GetEntity(authoring.point)
            }
        );
    }
}