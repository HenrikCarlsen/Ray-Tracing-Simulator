using Unity.Entities;
using Unity.Mathematics;

namespace Particle
{
    public struct Kinetic : IComponentData
    {
        public double3 position;
        public double3 velocity;

        public override string ToString()
        {
            return "position: " + string.Format("{0:F3}", position) + ", velocity: " + string.Format("{0:F3}", velocity);
        }
    }
}