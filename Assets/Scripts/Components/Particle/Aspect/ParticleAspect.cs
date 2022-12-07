using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

// Aspects must be declared as a readonly partial struct
readonly partial struct ParticleAspect : IAspect
{
    // An Entity field in an Aspect gives access to the Entity itself.
    // This is required for registering commands in an EntityCommandBuffer for example.
    public readonly Entity Self;

    readonly RefRO<Spin> Spin;
    public float spin { get => Spin.ValueRO.spin; }

    readonly RefRO<Charge> Charge;
    public float charge { get => Charge.ValueRO.charge; }

    readonly RefRO<Mass> Mass;
    public float mass { get => Mass.ValueRO.mass; }


    readonly RefRW<Movement> Movement;
    public double3 position
    {
        get => Movement.ValueRO.position;
        set => Movement.ValueRW.position = value;
    }
    public double3 velocity
    {
        get => Movement.ValueRO.velocity;
        set => Movement.ValueRW.velocity = value;
    }
}