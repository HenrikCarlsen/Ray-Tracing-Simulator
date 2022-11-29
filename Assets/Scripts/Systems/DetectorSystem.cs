using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Jobs;

using UnityEngine;

[BurstCompile]
partial struct DetectorSystem : ISystem
{
    EntityQuery allMirrorsQuery;

    EntityQuery allParticleTag;
    
    ComponentLookup<MirrorTag> GetPlane;

    [BurstCompile]
    public void OnCreate(ref SystemState state){}

    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }

    [BurstCompile]
    public void OnUpdate(ref SystemState state){}
}

//