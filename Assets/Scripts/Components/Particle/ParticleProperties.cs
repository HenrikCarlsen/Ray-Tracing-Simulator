using Unity.Entities;
using Unity.Mathematics;


// What is the last collision 
public struct ParticleProperties : ISharedComponentData
{
    public double mass;
    public double charge;
    public double spin; 
    public double lifetime;
}