using Unity.Entities;
using Unity.Mathematics;


public struct HistoryFuturePoint : IBufferElementData
{
    public Entity point;
    public Entity path;
}