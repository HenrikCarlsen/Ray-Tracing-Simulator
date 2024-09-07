// after every collision a collisionHistory entity is made (at the start of the collision) and 
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Jobs;
using System.Runtime.InteropServices;

using UnityEngine;
using Particle;

namespace History
{

    [BurstCompile]
    [StructLayout(LayoutKind.Auto)]
    [UpdateInGroup(typeof(HistoryOfParticles))]
    partial struct HistorySystem : ISystem
    {
        EntityArchetype ArchetypeHistory;
        EntityArchetype ArchetypeHistoryPoint;

        EntityQuery queryParticleWithHistory;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            ArchetypeHistory = state.EntityManager.CreateArchetype(
                typeof(HistoryEntireChild)
            );
            ArchetypeHistoryPoint = state.EntityManager.CreateArchetype(
                typeof(HistoryPoint),
                typeof(HistoryPast),
                typeof(HistoryFuture),
                typeof(HistoryEntireParent)
            );
            // TODO catch only new particles
            queryParticleWithHistory = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<Particle.ParticleTag, HistoryParticle>()
                .WithNone<Particle.FlowHistory>()
                .Build(ref state);

        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) { }

        public void OnUpdate(ref SystemState state)
        {

            var queryParticleWithHistoryArray = queryParticleWithHistory.ToEntityArray(Allocator.Temp);

            // CANT BE DONE IN PARALLEL AS WE NEED TO WRITE TO HistoryPointFuture
            // For each particle with history
            for (int i = 0; i < queryParticleWithHistoryArray.Length; i++)
            {
                var particleEntity = queryParticleWithHistoryArray[i];
                var particleSimulation = state.EntityManager.GetComponentData<Simulation>(particleEntity);

                if (particleSimulation.generation == Particle.Simulation.Generation.source) continue;
                if (state.EntityManager.GetComponentData<HistoryParticle>(particleEntity).point != Entity.Null) continue;


                state.EntityManager.AddComponent<Particle.FlowHistory>(particleEntity);

                // Make new point and connect to particle
                var newPointEntity = state.EntityManager.Instantiate(state.EntityManager.GetComponentData<HistoryParticlePrefab>(particleEntity).Value);
                state.EntityManager.SetComponentData<HistoryParticle>(particleEntity,
                    new HistoryParticle { point = newPointEntity }
                );
                state.EntityManager.SetComponentData<HistoryPoint>(newPointEntity,
                    new HistoryPoint { particle = particleEntity, position = state.EntityManager.GetComponentData<Kinetic>(particleEntity).position }
                );
                // Update old Future
                var lastParticleEntity = state.EntityManager.GetComponentData<Simulation>(particleEntity).lastParticle;
                var lastPointEntity = state.EntityManager.GetComponentData<HistoryParticle>(lastParticleEntity).point;

                var oldHistoryPointFutures = state.EntityManager.GetBuffer<HistoryFuture>(lastPointEntity);
                oldHistoryPointFutures.Add(new HistoryFuture { point = newPointEntity });
                // Set Past
                state.EntityManager.SetComponentData<HistoryPast>(newPointEntity, new HistoryPast { point = lastPointEntity });
                // Add to entire History
                var historyEntire = state.EntityManager.GetComponentData<HistoryEntireParent>(lastPointEntity).entire;
                var historyEntireBuffer = state.EntityManager.GetBuffer<HistoryEntireChild>(state.EntityManager.GetComponentData<HistoryEntireParent>(lastPointEntity).entire);
                historyEntireBuffer.Add(new HistoryEntireChild { point = newPointEntity });
                state.EntityManager.SetComponentData<HistoryEntireParent>(newPointEntity,
                    new HistoryEntireParent { entire = historyEntire }
                );
                // Update graphics position
                var transform = state.EntityManager.GetComponentData<LocalTransform>(newPointEntity);
                transform.Position = (Unity.Mathematics.float3)state.EntityManager.GetComponentData<HistoryPoint>(newPointEntity).position;
                transform.Scale = particleSimulation.weight * 0.25f;
                state.EntityManager.SetComponentData<LocalTransform>(newPointEntity, transform);

            }
        }
    }
}