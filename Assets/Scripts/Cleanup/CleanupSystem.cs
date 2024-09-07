// using Unity.Burst;
// using Unity.Entities;
// using Unity.Mathematics;
// using Unity.Transforms;
// using Unity.Collections;
// using Unity.Jobs;
// using System.Runtime.InteropServices;

// using UnityEngine;
// using System;
// using UnityEngine.UIElements;
// using System.Collections.Generic;


// namespace Cleanup
// {
//     [BurstCompile]
//     [StructLayout(LayoutKind.Auto)]
//     [UpdateInGroup(typeof(CleanupParticles))]
//     partial struct CleanupSystem : ISystem
//     {
//         EntityQuery allOutOfBoundsParticlesQuery;
//         EntityQuery allInBoundsAndIntersectingParticlesQuery;

//         [BurstCompile]
//         public void OnCreate(ref SystemState state)
//         {
//             allOutOfBoundsParticlesQuery = new EntityQueryBuilder(Allocator.Persistent)
//                 .WithAll<Particle.ParticleTag, Particle.OutOfBounds>()
//                 .Build(ref state);

//             allInBoundsAndIntersectingParticlesQuery = new EntityQueryBuilder(Allocator.Persistent)
//                 .WithAll<Particle.ParticleTag, Particle.IntersectingGeometry>()
//                 .WithNone<Particle.OutOfBounds>()
//                 .Build(ref state);
//         }

//         [BurstCompile]
//         public readonly void OnDestroy(ref SystemState state)
//         { }

//         [BurstCompile]
//         public void OnUpdate(ref SystemState state)
//         {
//             // Get all particles that didn't intersect anything
//             state.EntityManager.DestroyEntity(allOutOfBoundsParticlesQuery);

//             var uniqueGeometries = new List<Particle.IntersectingGeometry>();
//             state.EntityManager.GetAllUniqueSharedComponentsManaged<Particle.IntersectingGeometry>(uniqueGeometries);

//             // Remove all that found intersections this frame
//             for (int i = 0; i < uniqueGeometries.Count; i++)
//             {
//                 if (uniqueGeometries[i].entity == Entity.Null) continue;
//                 allInBoundsAndIntersectingParticlesQuery.SetSharedComponentFilter<Particle.IntersectingGeometry>
//                 (
//                     uniqueGeometries[i]
//                 );
//                 state.EntityManager.DestroyEntity(allInBoundsAndIntersectingParticlesQuery);
//             }
//         }
//     }
// }