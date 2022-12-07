using Unity.Entities;

using UnityEngine;


// Authoring MonoBehaviours are regular GameObject components.
// They constitute the inputs for the baking systems which generates ECS data.
public class ParticleGeneratorAuthoring : UnityEngine.MonoBehaviour
{
    public GameObject particle;
}

public struct ParticleGenerator : IComponentData
{
    public Entity particle;
}


// Bakers convert authoring MonoBehaviours into entities and components.
public class ParticleGeneratorBaker : Baker<ParticleGeneratorAuthoring>
{
    public override void Bake(ParticleGeneratorAuthoring authoring)
    {
        AddComponent<Movement>();
        AddComponent<Plane>();
        AddComponent<GeneratorTag>();
        AddComponent<ParticleGenerator>(new ParticleGenerator{particle = GetEntity(authoring.particle)});
        AddComponent<MirrorSimple>();
        AddComponent<ParticleHistory>();


    }
}