using Unity.Entities;
using Unity.Mathematics;


using UnityEngine;

namespace Graphics
{
    // Authoring MonoBehaviours are regular GameObject components.
    // They constitute the inputs for the baking systems which generates ECS data.
    public class HistoryPathHolderAuthoring : UnityEngine.MonoBehaviour
    {
        public GameObject pipePrefab;
    }

    // Bakers convert authoring MonoBehaviours into entities and components.
    public class HistoryPathHolderAuthoringBaker : Baker<HistoryPathHolderAuthoring>
    {
        public override void Bake(HistoryPathHolderAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            var pipeEntity = GetEntity(authoring.pipePrefab, TransformUsageFlags.NonUniformScale);

            AddComponent<HistoryPathGraphics>(entity, new HistoryPathGraphics { prefab = GetEntity(authoring.pipePrefab, TransformUsageFlags.NonUniformScale) });
        }
    }

}