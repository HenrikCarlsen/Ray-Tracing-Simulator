using Unity.Entities;

using UnityEngine;


// Authoring MonoBehaviours are regular GameObject components.
// They constitute the inputs for the baking systems which generates ECS data.
public class ParticleGeneratorHolderAuthoring : UnityEngine.MonoBehaviour
{
    public GameObject generatorPrefab;
}

public struct ParticleGeneratorHolder : IComponentData
{
    public Entity generatorPrefab;
}


// Bakers convert authoring MonoBehaviours into entities and components.
public class PParticleGeneratorHolderBaker : Baker<ParticleGeneratorHolderAuthoring>
{
    public override void Bake(ParticleGeneratorHolderAuthoring authoring)
    {
        AddComponent<ParticleGeneratorHolder>(
            new ParticleGeneratorHolder{
                generatorPrefab = GetEntity(authoring.generatorPrefab)
            }
        );
        AddComponent<GeneratorHolderTag>();
        //generatorPrefab = GetEntity(authoring.generatorPrefab);
    }
}