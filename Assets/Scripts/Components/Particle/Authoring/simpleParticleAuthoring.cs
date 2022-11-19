using Unity.Entities;
using Unity.Mathematics;

// Authoring MonoBehaviours are regular GameObject components.
// They constitute the inputs for the baking systems which generates ECS data.
public class simpleParticleAuthoring : UnityEngine.MonoBehaviour
{
        public float mass;
}

// Bakers convert authoring MonoBehaviours into entities and components.
public class simpleParticleBaker : Baker<simpleParticleAuthoring>
{
    public override void Bake(simpleParticleAuthoring authoring)
    {
        AddComponent<ParticleTag>();
        AddComponent<Movement>();
        AddComponent<Force>();
        AddComponent<Mass>(new Mass{mass = authoring.mass});

        AddComponent<ParticleHistory>();
    }
}