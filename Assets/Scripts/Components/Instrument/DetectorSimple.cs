using Unity.Entities;

using Unity.Mathematics;
using Unity.Collections;

public struct DetectorSimple : IComponentData
{
    public float2 size;
    public int2 pixel;
    public FixedString32Bytes filename;
}