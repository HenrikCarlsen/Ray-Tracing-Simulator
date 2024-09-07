using Unity.Entities;
using Unity.Mathematics;

namespace Particle
{
    public struct FlowCreate : IComponentData { }
    public struct FlowMove : IComponentData { }
    public struct FlowInteraction : IComponentData { }
    public struct FlowHistory : IComponentData { }
    public struct FlowCleanup : IComponentData { }


}