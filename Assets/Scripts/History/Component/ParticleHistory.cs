using Unity.Entities;
using Unity.Mathematics;


namespace History
{
    public struct HistoryParticle : IComponentData
    {
        public Entity point;
    }

    public struct HistoryPoint : IComponentData
    {
        public Entity particle;

        public double3 position; // for testing only
    }
    public struct HistoryPast : IComponentData
    {
        public Entity point;
    }

    public struct HistoryFuture : IBufferElementData
    {
        public Entity point;
    }


    public struct HistoryEntireParent : IComponentData
    {
        public Entity entire;
    }


    public struct HistoryEntireChild : IBufferElementData
    {
        public Entity point;
    }

    public struct HistoryParticlePrefab : IComponentData
    {
        public Entity Value;
    }


}