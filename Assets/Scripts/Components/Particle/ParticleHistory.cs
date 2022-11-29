using Unity.Entities;
using Unity.Mathematics;


// What is the last collision 
public struct ParticleHistoryPast : IComponentData
{
    public Entity point;
}

public struct ParticleHistoryFuture : IComponentData
{
    public Entity point;
}

public struct ParticleHistory : IComponentData
{
    public Entity lastHistoryPoint;
}


public struct ParticleHistoryGraphic : IComponentData
{
    public Entity graphicPath;
}

public struct ParticleHistoryPath : IComponentData
{
    public double3 before;
    public double3 after;
}