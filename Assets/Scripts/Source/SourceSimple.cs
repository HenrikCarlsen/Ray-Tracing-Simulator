using Unity.Entities;
using Unity.Mathematics;

namespace Source
{
    public struct SourceSimple : IComponentData
    {
        public uint randomSeed;
        public Random random;

        public Plane sourceSurface;
        public Plane sourceTarget;

        public double2 velocityRange;

        public Entity particle;
    }
}