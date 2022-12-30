// after every collision a collisionHistory entity is made (at the start of the collision) and 
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Jobs;

using UnityEngine;


[UpdateBefore(typeof(MoveClassicallySystem))]
[UpdateBefore(typeof(SourceSystem))]
[UpdateBefore(typeof(MirrorSystem))]

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
        state.RequireForUpdate<SingletonHistorySetting>();

        EntityManager entityManager = state.EntityManager;

        var types2 = new NativeArray<ComponentType>(3, Allocator.Temp);
        types2[0] = ComponentType.ReadOnly<Movement>();
        types2[1] = ComponentType.ReadOnly<ParticleTag>();
        types2[2] = ComponentType.ReadWrite<ParticleHistory>();

        queryMovedParticles = state.GetEntityQuery(types2);
        queryMovedParticles.SetChangedVersionFilter(ComponentType.ReadOnly<Movement>());

        var types3 = new NativeArray<ComponentType>(2, Allocator.TempJob);
        types3[0] = ComponentType.ReadWrite<Movement>();
        types3[1] = ComponentType.ReadWrite<ParticleHistoryPath>();
        queryChangedHistoryPoints = state.GetEntityQuery(types3);
        queryChangedHistoryPoints.SetChangedVersionFilter(ComponentType.ReadWrite<ParticleHistoryPath>());
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }

    public void OnUpdate(ref SystemState state)
    {

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var HistorySettings = SystemAPI.GetSingleton<SingletonHistorySetting>();

        if(!HistorySettings.historyTrackingOn) return;

        var entityManager = state.EntityManager;
        foreach (var movedParticle in queryMovedParticles.ToEntityArray(Allocator.Temp))
        {
            Entity historyPoint = entityManager.Instantiate(HistorySettings.point);
            entityManager.SetComponentData<Movement>(historyPoint, entityManager.GetComponentData<Movement>(movedParticle));

            // Connect this historyPoint to the last historyPoint for the current particle
            entityManager.SetComponentData<ParticleHistoryPast>(historyPoint,
                new ParticleHistoryPast { point = entityManager.GetComponentData<ParticleHistory>(movedParticle).lastHistoryPoint }
            );

            bool firstPointInHistory = entityManager.GetComponentData<ParticleHistory>(movedParticle).lastHistoryPoint == Entity.Null;

            entityManager.SetComponentData<ParticleHistory>(movedParticle, new ParticleHistory { lastHistoryPoint = historyPoint });

            if (!firstPointInHistory)
            {
                // Make path
                var P0 = entityManager.GetComponentData<Movement>(
                    entityManager.GetComponentData<ParticleHistoryPast>(historyPoint).point
                    ).position;
                var P1 = entityManager.GetComponentData<Movement>(movedParticle).position;

                var Pmiddle = P0 + (P1 - P0) / 2.0;

                Entity historyPath = entityManager.Instantiate(HistorySettings.path);
                entityManager.SetComponentData<Movement>(historyPath, new Movement { position = Pmiddle });
                entityManager.SetComponentData<ParticleHistoryPath>(historyPath, new ParticleHistoryPath { before = P0, after = P1 });


                entityManager.SetComponentData<ParticleHistoryGraphic>(historyPoint, new ParticleHistoryGraphic { graphicPath = historyPath });

            }
            // Update movedParticle history
            entityManager.SetComponentData<ParticleHistory>(movedParticle, new ParticleHistory { lastHistoryPoint = historyPoint });
        }

        state.CompleteDependency();
    }
}