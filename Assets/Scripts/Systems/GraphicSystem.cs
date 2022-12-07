// after every collision a collisionHistory entity is made (at the start of the collision) and 
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Jobs;

using UnityEngine;

public partial struct AlignTransformWithMovement : IJobEntity
{
    public void Execute(ref LocalToWorldTransform transform, in Movement movement)
    {
        transform.Value.Position = (float3)movement.position;
        //transform.Value.Rotation = quaternion.LookRotation()
    }
}

public partial struct AlignTransformWithMovementForPath : IJobEntity
{
    public void Execute(ref LocalToWorldTransform transform, in Movement movement, in ParticleHistoryPath path)
    {
        float3 p = (float3)(path.after) - (float3)movement.position;

        transform.Value.Rotation = quaternion.LookRotationSafe(p, math.right());
        transform.Value.Scale = math.length((float3)(path.after - path.before)) / 2.0f;
    }
}

[UpdateAfter(typeof(HistorySystem))]

[BurstCompile]
partial struct GraphicSystem : ISystem
{

    [BurstCompile]
    public void OnCreate(ref SystemState state) { }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var jobAlign = new AlignTransformWithMovement();
        jobAlign.ScheduleParallel(state.Dependency);

        var jobAlignPath = new AlignTransformWithMovementForPath();
        jobAlignPath.ScheduleParallel(state.Dependency);

        state.CompleteDependency();
    }
}