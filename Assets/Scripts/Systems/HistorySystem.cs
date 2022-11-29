// after every collision a collisionHistory entity is made (at the start of the collision) and 
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Jobs;

using UnityEngine;

public partial struct MakeParticleHistory : IJobEntity
{

    public EntityCommandBuffer CommandBuffer;
    //[ReadOnly] public Entity historyPathEntity;
    [ReadOnly] public Entity historyPointEntity;

    public void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, in Movement movement, ref ParticleHistory particleHistory)
    {

        Entity historyPoint = CommandBuffer.Instantiate(historyPointEntity);
        CommandBuffer.SetComponent<Movement>(historyPoint, movement);

        // Connect this historyPoint to the last historyPoint for the current particle
        CommandBuffer.SetComponent<ParticleHistoryPast>(historyPoint, new ParticleHistoryPast { point = particleHistory.lastHistoryPoint });
        // Update lastHistoryPoint to the current historyPoint
        CommandBuffer.SetComponent<ParticleHistory>(entity, new ParticleHistory { lastHistoryPoint = historyPoint });
    }
}

public partial struct MakeParticlePathHistory : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter CommandBuffer;
    [ReadOnly] public Entity historyPathEntity;

    [ReadOnly] public ComponentLookup<Movement> HistoryPointMovement;


    public void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity,
                        in ParticleHistoryPast particleHistoryPast,
                        ref ParticleHistoryGraphic particleHistoryGraphic)
    {

        //Debug.Log("DEBUG!!!!");


        if (particleHistoryGraphic.graphicPath != Entity.Null) return;



        Entity historyPath = CommandBuffer.Instantiate(chunkIndex, historyPathEntity);
        particleHistoryGraphic.graphicPath = historyPath;

        // get current and last particleHistoryPast position from movement
        //
        if (particleHistoryPast.point == Entity.Null) return;

        var P0 = HistoryPointMovement[particleHistoryPast.point].position;
        var P1 = HistoryPointMovement[entity].position;

        var Pmiddle = P0 + (P1 - P0) / 2.0;

        //Debug.Log("P1: " + P1 + ", P0: " + P0);

        // Find rotation
        // make rotation component on path and sort it all out in the transform


        CommandBuffer.SetComponent<Movement>(chunkIndex, historyPath, new Movement { position = Pmiddle });
        CommandBuffer.SetComponent<ParticleHistoryPath>(chunkIndex, historyPath, new ParticleHistoryPath { before = P0, after = P1 });





        //Debug.Log("chunkIndex: " + chunkIndex);
    }
}

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
        float3 p =  (float3)(path.after) - (float3)movement.position;


        transform.Value.Rotation = quaternion.LookRotationSafe(p,math.right());
        transform.Value.Scale = math.length( (float3)(path.after-path.before) ) / 2.0f;
    }
}

[BurstCompile]
partial struct HistorySystem : ISystem
{
    EntityQuery queryMovedParticles;
    EntityQuery queryChangedHistoryPoints;

    EntityQuery gueryHistoryPointMovement;

    ComponentLookup<Movement> GetMovement;



    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        EntityManager entityManager = state.EntityManager;

        var types2 = new NativeArray<ComponentType>(2, Allocator.Temp);
        types2[0] = ComponentType.ReadWrite<Movement>();
        types2[1] = ComponentType.ReadOnly<ParticleTag>();
        queryMovedParticles = state.GetEntityQuery(types2);
        queryMovedParticles.SetChangedVersionFilter(ComponentType.ReadWrite<Movement>());

        var types3 = new NativeArray<ComponentType>(1, Allocator.Temp);
        types3[0] = ComponentType.ReadWrite<ParticleHistoryPast>();
        queryChangedHistoryPoints = state.GetEntityQuery(types3);
        queryChangedHistoryPoints.SetChangedVersionFilter(ComponentType.ReadWrite<ParticleHistoryPast>());


        gueryHistoryPointMovement = new EntityQueryBuilder(Allocator.TempJob).WithAll<Movement>().Build(ref state);
        GetMovement = state.GetComponentLookup<Movement>(true);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var HistorySettings = SystemAPI.GetSingleton<SingletonHistorySetting>();

        var jobMakeParticleHistory = new MakeParticleHistory
        {
            historyPointEntity = HistorySettings.point,
            CommandBuffer = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged)
        };
        jobMakeParticleHistory.Run(queryMovedParticles);

        GetMovement.Update(ref state);
        var jobMakeParticlePathHistory = new MakeParticlePathHistory
        {
            historyPathEntity = HistorySettings.path,
            CommandBuffer = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
            HistoryPointMovement = GetMovement
        };
        jobMakeParticlePathHistory.Run(queryChangedHistoryPoints);

        // Align unity the transform with movement component (used for graphic only)
        var jobAlign = new AlignTransformWithMovement();
        jobAlign.ScheduleParallel(state.Dependency);

        var jobAlignPath = new AlignTransformWithMovementForPath();
        jobAlignPath.ScheduleParallel(state.Dependency);
    }
}