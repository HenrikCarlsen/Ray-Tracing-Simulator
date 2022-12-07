using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

// Aspects must be declared as a readonly partial struct
readonly partial struct SimpleMirrorAspect : IAspect
{
    // An Entity field in an Aspect gives access to the Entity itself.
    // This is required for registering commands in an EntityCommandBuffer for example.
    public readonly Entity Self;


    public readonly RefRO<Plane> plane;
}