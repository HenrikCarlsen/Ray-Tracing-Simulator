using Unity.Entities;
using UnityEngine;

using Unity.Mathematics;


// Authoring MonoBehaviours are regular GameObject components.
// They constitute the inputs for the baking systems which generates ECS data.
public class SourceAuthoring : UnityEngine.MonoBehaviour
{
    public float3 startVelocity;

    public GameObject particle;
}

// Bakers convert authoring MonoBehaviours into entities and components.
public class SourceBaker : Baker<SourceAuthoring>
{
    public override void Bake(SourceAuthoring authoring)
    {
        AddComponent<Source>(new Source
            {
                particle = GetEntity(authoring.particle),
                startVelocity = authoring.startVelocity,
                startPosition = (float3)authoring.transform.position
            }
        );
        AddComponent<Plane>(new Plane(authoring.gameObject));
    }
}