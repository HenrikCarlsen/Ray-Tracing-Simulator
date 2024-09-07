// // after every collision a collisionHistory entity is made (at the start of the collision) and 
// using Unity.Burst;
// using Unity.Entities;
// using Unity.Mathematics;
// using Unity.Transforms;
// using Unity.Collections;
// using Unity.Jobs;
// using System.Runtime.InteropServices;

// using UnityEngine;
// using UnityEngine.Rendering;
// using Particle;

// namespace History
// {

//     [BurstCompile]
//     [StructLayout(LayoutKind.Auto)]
//     [UpdateInGroup(typeof(CreateParticlesEnd))]
//     partial struct HistorySourceSystem : ISystem
//     {
//         EntityArchetype ArchetypeHistory;
//         EntityArchetype ArchetypeHistoryPoint;

//         EntityQuery queryParticleWithHistory;

//         [BurstCompile]
//         public void OnCreate(ref SystemState state)
//         {
//             ArchetypeHistory = state.EntityManager.CreateArchetype(
//                 typeof(HistoryEntireChild)
//             );
//             ArchetypeHistoryPoint = state.EntityManager.CreateArchetype(
//                 typeof(HistoryPoint),
//                 typeof(HistoryPast),
//                 typeof(HistoryFuture),
//                 typeof(HistoryEntireParent)
//             );

//             queryParticleWithHistory = new EntityQueryBuilder(Allocator.Temp)
//                 .WithAll<Particle.ParticleTag, Particle.Kinetic, HistoryParticle>()
//                 .WithNone<Particle.FlowHistory>()
//                 .Build(ref state);
//         }

//         [BurstCompile]
//         readonly public void OnDestroy(ref SystemState state) { }

//         public void OnUpdate(ref SystemState state)
//         {
//             // For each particle with the history tag. Make new history and add point to it

//             var CommandBuffer = new EntityCommandBuffer(Allocator.TempJob);

//             var makeHistoryJob = new MakeHistory
//             {
//                 ArchetypeHistory = ArchetypeHistory,
//                 ArchetypeHistoryPoint = ArchetypeHistoryPoint,
//                 CommandBuffer = CommandBuffer.AsParallelWriter()
//             };
//             var makeHistoryHandler = makeHistoryJob.ScheduleParallel(queryParticleWithHistory, state.Dependency);
//             makeHistoryHandler.Complete();

//             CommandBuffer.Playback(state.EntityManager);
//             CommandBuffer.Dispose();
//         }
//     }
//     [BurstCompile]
//     [StructLayout(LayoutKind.Auto)]
//     partial struct MakeHistory : IJobEntity
//     {
//         public EntityArchetype ArchetypeHistory;
//         public EntityArchetype ArchetypeHistoryPoint;
//         public EntityCommandBuffer.ParallelWriter CommandBuffer;

//         public void Execute(
//             Entity entity,
//             ref HistoryParticle historyParticle,
//             Particle.Kinetic kinetic,
//             Particle.Simulation simulation,
//             [ChunkIndexInQuery] int chunkIndex
//         )
//         {
//             if (simulation.generation != Particle.Simulation.Generation.source) return;
//             if (historyParticle.point != Entity.Null) return;

//             CommandBuffer.AddComponent<FlowHistory>(chunkIndex, entity);

//             Debug.Log("HERE!!!!!!! start of source history!!!");

//             // Create history and make first point in it
//             // var historyEntireEntity = CommandBuffer.CreateEntity(chunkIndex, ArchetypeHistory);
//             var historyPointEntity = CommandBuffer.CreateEntity(chunkIndex, ArchetypeHistoryPoint);

//             // Set no past
//             // Connect to particle
//             historyParticle.point = historyPointEntity;
//             // CommandBuffer.SetComponent<HistoryPoint>(chunkIndex, historyPointEntity,
//             //     new HistoryPoint { particle = historyPointEntity, position = kinetic.position }
//             // );
//             // // Set no future

//             // // connect to entire history
//             // var historyEntire = CommandBuffer.SetBuffer<HistoryEntireChild>(chunkIndex, historyEntireEntity);
//             // historyEntire.Add(new HistoryEntireChild { point = historyPointEntity });
//             // CommandBuffer.SetComponent<HistoryEntireParent>(chunkIndex, historyPointEntity,
//             //     new HistoryEntireParent { entire = historyEntireEntity }
//             // );
//         }
//     }
// }