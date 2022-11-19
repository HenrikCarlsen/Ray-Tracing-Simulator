using Unity.Entities;
using Unity.Mathematics;

// An empty component is called a "tag component".
public struct Movement : IComponentData
{
    public double3 position;
    public double3 velocity;

    public override string ToString(){
        return "pos: " + position + ", vel: " + velocity;
    }
}