using Unity.Entities;
using Unity.Mathematics;

public struct Source : IComponentData
{
    public float3 startVelocity;

    public float3 startPosition;

    public Entity particle;
}