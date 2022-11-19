using Unity.Entities;
using Unity.Mathematics;


// What is the last collision 
public struct ParticleHistoryPast : IComponentData
{
    public Entity point;
}

public struct ParticleHistoryFuture : IBufferElementData
{
    public Entity point;
}

public struct ParticleHistory : IComponentData
{
    public Entity endPoint;
}