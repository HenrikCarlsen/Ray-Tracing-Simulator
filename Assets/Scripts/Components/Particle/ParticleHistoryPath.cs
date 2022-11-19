using Unity.Entities;
using Unity.Mathematics;


public struct ParticleHistoryPath : IComponentData
{
    public Entity lastInteraction;
    public Entity nextInteraction;

    public float weight;

}