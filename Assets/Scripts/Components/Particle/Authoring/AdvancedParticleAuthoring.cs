using Unity.Entities;
using Unity.Mathematics;

// Authoring MonoBehaviours are regular GameObject components.
// They constitute the inputs for the baking systems which generates ECS data.
public class AdvancedParticleAuthoring : UnityEngine.MonoBehaviour
{
        public float mass;
}

// Bakers convert authoring MonoBehaviours into entities and components.
public class AdvancedParticleBaker : Baker<AdvancedParticleAuthoring>
{
    public override void Bake(AdvancedParticleAuthoring authoring)
    {
        
        AddComponent<ParticleTag>();
        AddComponent<Movement>();
        AddComponent<Force>();
        AddComponent<Mass>(new Mass{mass = authoring.mass});

        AddComponent<ParticleHistory>();
    }
}