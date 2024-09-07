using Unity.Entities;
using Unity.Mathematics;

namespace Particle
{
    public struct UniformForce : IComponentData
    {
        public double3 AccelerationSum;

        public readonly Particle.Kinetic Move(in Particle.Kinetic movement, in double time)
        {
            return new Particle.Kinetic
            {
                position = movement.position + movement.velocity * time + AccelerationSum * time * time,
                velocity = movement.velocity + AccelerationSum * time
            };
        }
    }
}