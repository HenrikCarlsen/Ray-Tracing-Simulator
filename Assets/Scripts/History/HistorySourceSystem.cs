// after every collision a collisionHistory entity is made (at the start of the collision) and 
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Jobs;
using System.Runtime.InteropServices;

using UnityEngine;
using UnityEngine.Rendering;
using Particle;

namespace History
{

    [BurstCompile]
    [StructLayout(LayoutKind.Auto)]
    [UpdateInGroup(typeof(CreateParticlesEnd))]
    partial struct HistorySourceSystem : ISystem
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

            queryParticleWithHistory = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<Particle.ParticleTag, Particle.Kinetic, HistoryParticle>()
                .WithNone<Particle.FlowHistory>()
                .Build(ref state);
        }

        [BurstCompile]
        readonly public void OnDestroy(ref SystemState state) { }

        public void OnUpdate(ref SystemState state)
        {
            var queryParticleWithHistoryArray = queryParticleWithHistory.ToEntityArray(Allocator.Temp);


            for (int i = 0; i < queryParticleWithHistoryArray.Length; i++)
            {
                var particleEntity = queryParticleWithHistoryArray[i];
                var particleSimulation = state.EntityManager.GetComponentData<Simulation>(particleEntity);
                var pointEntity = state.EntityManager.GetComponentData<HistoryParticle>(particleEntity).point;

                if (particleSimulation.generation != Particle.Simulation.Generation.source) continue;
                if (pointEntity != Entity.Null) continue;


                state.EntityManager.AddComponent<Particle.FlowHistory>(particleEntity);


                //state.EntityManager.GetComponentData<HistoryParticlePrefab>(particleEntity).Value;

                // Create history point and make first point in it
                var historyPointEntity = state.EntityManager.Instantiate(state.EntityManager.GetComponentData<HistoryParticlePrefab>(particleEntity).Value);
                // Set no past
                // Connect to particle
                state.EntityManager.SetComponentData<HistoryPoint>(historyPointEntity,
                    new HistoryPoint { particle = particleEntity, position = state.EntityManager.GetComponentData<Kinetic>(particleEntity).position }
                );
                state.EntityManager.SetComponentData<HistoryParticle>(particleEntity,
                    new HistoryParticle { point = historyPointEntity }
                );
                // Set no future

                // connect to entire history
                var historyEntireEntity = state.EntityManager.CreateEntity(ArchetypeHistory);

                var historyEntire = state.EntityManager.GetBuffer<HistoryEntireChild>(historyEntireEntity);
                historyEntire.Add(new HistoryEntireChild { point = historyPointEntity });
                state.EntityManager.SetComponentData<HistoryEntireParent>(historyPointEntity,
                    new HistoryEntireParent { entire = historyEntireEntity }
                );

                // Update graphics position
                var transform = state.EntityManager.GetComponentData<LocalTransform>(historyPointEntity);
                transform.Position = (Unity.Mathematics.float3)state.EntityManager.GetComponentData<HistoryPoint>(historyPointEntity).position;
                transform.Scale = particleSimulation.weight * 0.25f;
                state.EntityManager.SetComponentData<LocalTransform>(historyPointEntity, transform);

            }
        }
    }
}