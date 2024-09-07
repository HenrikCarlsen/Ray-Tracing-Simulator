using Unity.Entities;
using Unity.Mathematics;
namespace Movement
{
    public struct Force : IComponentData
    {
        public double3 AccelerationSum;


    }



}