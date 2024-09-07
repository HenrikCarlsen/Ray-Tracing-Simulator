using Unity.Entities;
using Unity.Mathematics;


namespace Graphics
{
    public struct HistoryPath : IComponentData
    {
        public Entity point;
    }
}