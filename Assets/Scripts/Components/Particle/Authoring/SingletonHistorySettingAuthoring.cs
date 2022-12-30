using Unity.Entities;

using UnityEngine;

public class SingletonHistorySettingAuthoring : UnityEngine.MonoBehaviour
{
    public GameObject path;
    public GameObject point;

    public bool historyTrackingOn;
}

public struct SingletonHistorySetting : IComponentData
{
    public Entity path;
    public Entity point;
    public bool historyTrackingOn;
}

public class SingletonHistorySettingBaker : Baker<SingletonHistorySettingAuthoring>
{
    public override void Bake(SingletonHistorySettingAuthoring authoring)
    {
        AddComponent<SingletonHistorySetting>(
            new SingletonHistorySetting
            {
                path = GetEntity(authoring.path),
                point = GetEntity(authoring.point),
                historyTrackingOn = authoring.historyTrackingOn
            }
        );
    }
}