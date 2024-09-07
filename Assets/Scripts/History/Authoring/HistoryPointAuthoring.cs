using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Transforms;
using System.Diagnostics;
using Unity.Entities.Serialization;

namespace History
{
    public class HistoryPointAuthoring : UnityEngine.MonoBehaviour
    { }

    public class HistoryPointBaker : Baker<HistoryPointAuthoring>
    {
        public override void Bake(HistoryPointAuthoring authoring)
        {
            AddComponent<HistoryPoint>();
            AddComponent<HistoryPast>();
            AddBuffer<HistoryFuture>();
            AddComponent<HistoryEntireParent>();
        }
    }
}