using Unity.Entities;
using Unity.Mathematics;

namespace Particle
{
    [ChunkSerializable]
    public struct IntersectingGeometry : ISharedComponentData
    {
        public Entity entity;
    }
}