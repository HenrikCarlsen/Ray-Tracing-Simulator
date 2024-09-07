using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Transforms;
using System.Diagnostics;
using Unity.Entities.Serialization;
using UnityEngine;

namespace History
{
    public class HistoryAuthoring : UnityEngine.MonoBehaviour
    {
        public bool enableHistory;

        public GameObject HistoryPointPrefab;
    }

    public class HistoryBaker : Baker<HistoryAuthoring>
    {
        public override void Bake(HistoryAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);

            if (authoring.enableHistory)
            {
                AddComponent<HistoryParticle>(entity);
                AddComponent<HistoryParticlePrefab>(
                    entity,
                    new HistoryParticlePrefab { Value = GetEntity(authoring.HistoryPointPrefab) }
                );
            }
        }
    }
}