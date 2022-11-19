using Unity.Entities;
using Unity.Mathematics;

// An empty component is called a "tag component".
public struct Charge : IComponentData
{
    public float charge;
}