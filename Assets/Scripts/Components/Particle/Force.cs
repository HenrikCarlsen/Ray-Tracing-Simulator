using Unity.Entities;
using Unity.Mathematics;

// An empty component is called a "tag component".
public struct Force : IComponentData
{
    public float3 AccelerationSum;
}