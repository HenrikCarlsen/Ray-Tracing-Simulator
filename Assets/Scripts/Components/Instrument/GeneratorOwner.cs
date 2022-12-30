using Unity.Entities;

using Unity.Mathematics;
using Unity.Collections;

public struct GeneratorOwner : IComponentData
{
    public Entity owner;
}