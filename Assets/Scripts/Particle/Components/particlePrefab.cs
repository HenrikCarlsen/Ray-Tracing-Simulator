using Unity.Entities;
using Unity.Mathematics;
using Unity.Entities.Serialization;

namespace Particle
{
    public struct particlePrefab : IComponentData
    {
        public Entity Value;
    }
}