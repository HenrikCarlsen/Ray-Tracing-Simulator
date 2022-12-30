using Unity.Entities;
using UnityEngine;

using Unity.Mathematics;
using Unity.Collections;

// Authoring MonoBehaviours are regular GameObject components.
// They constitute the inputs for the baking systems which generates ECS data.
public class SimpleDetectorTypeAuthoring : UnityEngine.MonoBehaviour
{
    public float2 size;
    public int2 pixel;
    public FixedString32Bytes filename;

}

// Bakers convert authoring MonoBehaviours into entities and components.
public class SimpleDetectorTypeBaker : Baker<SimpleDetectorTypeAuthoring>
{
    public override void Bake(SimpleDetectorTypeAuthoring authoring)
    {
        // Generator tags
        AddComponent<GeneratorTag>();
        AddComponent<DetectorTag>();

        // Generator properties
        AddComponent<Plane>(new Plane(authoring.gameObject));
        //AddComponent<DetectorOwner>(new DetectorOwner{owner = Entity.Null } );

        AddComponent<GeneratorOwner>(new GeneratorOwner{owner = Entity.Null});

        // Particle properties
        AddComponent<Movement>();
        AddSharedComponent<ParticleProperties>(new ParticleProperties());

        // History
        AddComponent<ParticleHistory>();
    }
}

