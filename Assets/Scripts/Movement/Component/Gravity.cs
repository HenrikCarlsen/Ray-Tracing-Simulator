using Unity.Entities;
using Unity.Mathematics;
namespace Movement
{
    public struct Gravity : IComponentData
    {
        public double3 acceleration;
    }
}