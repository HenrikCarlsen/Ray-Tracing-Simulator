using Unity.Entities;
using Unity.Mathematics;

namespace Particle
{
    public struct IntersectingGeometryTemp : IComponentData
    {
        public Entity entity;
    }
}