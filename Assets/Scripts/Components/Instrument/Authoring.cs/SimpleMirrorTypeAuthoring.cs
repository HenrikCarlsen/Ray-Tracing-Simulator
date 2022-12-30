using Unity.Entities;
using UnityEngine;

using Unity.Mathematics;


// Authoring MonoBehaviours are regular GameObject components.
// They constitute the inputs for the baking systems which generates ECS data.
public class SimpleMirrorTypeAuthoring : UnityEngine.MonoBehaviour
{
    //public bool simple;
    //public double2 waveness;

    public SimpleMirrorTypeAuthoring()
    {

    }
}

// Bakers convert authoring MonoBehaviours into entities and components.
public class SimpleMirrorTypeBaker : Baker<SimpleMirrorTypeAuthoring>
{
    public override void Bake(SimpleMirrorTypeAuthoring authoring)
    {
        // Generator tags
        AddComponent<GeneratorTag>();
        AddComponent<MirrorTag>();

        // Generator properties
        AddComponent<Plane>(new Plane(authoring.gameObject));
        //AddSharedComponent<MirrorWaveness>(new MirrorWaveness());
        AddComponent<GeneratorOwner>(new GeneratorOwner{owner = Entity.Null});


        // Particle properties
        AddComponent<Movement>();
        AddSharedComponent<ParticleProperties>(new ParticleProperties());

        // History
        AddComponent<ParticleHistory>();
    }
}
