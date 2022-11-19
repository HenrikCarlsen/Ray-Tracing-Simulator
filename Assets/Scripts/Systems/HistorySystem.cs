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

    public EntityCommandBuffer.ParallelWriter CommandBuffer;
    public Entity historyPathEntity;
    public Entity historyPointEntity;


    public void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, in Movement movement, ref ParticleHistory particleHistory)
    {
        Entity history = CommandBuffer.Instantiate(chunkIndex, historyPointEntity);

        CommandBuffer.SetComponent<Movement>(chunkIndex,history,movement);

        // make path between the last interaction and the last interaction
        //Entity historyPath = Ecb.CreateEntity(chunkIndex, historyPathArchetype);

        // var path = new ParticleHistoryPath{
        //     lastInteraction = particleHistory.lastInteraction,
        //     nextInteraction = history
        // };
        // Ecb.SetComponent<ParticleHistoryPath>(chunkIndex,historyPath,path);

        // particleHistory.lastInteraction = history;
    }
}

public partial struct AlignTransformWithMovement : IJobEntity
{
    public void Execute(ref LocalToWorldTransform transform, in Movement movement)
    {
        transform.Value.Position = (float3)movement.position;
    }
}


[BurstCompile]
partial struct HistorySystem : ISystem
{
    EntityQuery queryChangedParticles;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // create art

        EntityManager entityManager = state.EntityManager;


        var types2 = new NativeArray<ComponentType>(2, Allocator.Temp);
        types2[0] = ComponentType.ReadWrite<Movement>();
        types2[1] = ComponentType.ReadOnly<ParticleTag>();
        queryChangedParticles = state.GetEntityQuery(types2);
        queryChangedParticles.SetChangedVersionFilter(ComponentType.ReadWrite<Movement>());
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // var historyPathEntity = HistorySettings.path;
        // var historyPointEntity = HistorySettings.point;

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        // Get history singletons
        var HistorySettings = SystemAPI.GetSingleton<SingletonHistorySetting>();

        var jobMakeParticleHistory = new MakeParticleHistory
        {
            historyPathEntity = HistorySettings.path,
            historyPointEntity = HistorySettings.point,
            CommandBuffer = ecb.AsParallelWriter()
        };
        jobMakeParticleHistory.ScheduleParallel(queryChangedParticles);

        // Align unity the transform with movement component (used for graphic only)
        var jobAlign = new AlignTransformWithMovement();
        jobAlign.ScheduleParallel(state.Dependency);
    }
}