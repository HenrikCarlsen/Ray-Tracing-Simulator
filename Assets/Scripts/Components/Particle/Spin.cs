using Unity.Entities;
using Unity.Mathematics;

// An empty component is called a "tag component".
public struct Spin : IComponentData
{
    public float spin;
}