using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Jobs;
using System.Runtime.InteropServices;
using UnityEngine;
using Particle;

namespace Movement
{
    [BurstCompile]
    [StructLayout(LayoutKind.Auto)]
    [UpdateInGroup(typeof(MoveParticles))]
    public partial struct MoveClassicallySystem : ISystem
    {

        EntityQuery allGeometriesQuery;
        EntityQuery allParticlesQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            allGeometriesQuery = new EntityQueryBuilder(Allocator.Persistent)
                .WithAll<Plane>()
                .WithNone<VirtualTag>()
                .Build(ref state);

            allParticlesQuery = new EntityQueryBuilder(Allocator.Persistent)
                .WithAll<Particle.Kinetic,
                Particle.UniformForce,
                Particle.Simulation,
                Particle.IntersectingGeometryTemp,
                Particle.IntersectingGeometry,
                Particle.InBounds>()
                .WithNone<Particle.FlowMove>()
                .Build(ref state);
        }


        [BurstCompile]
        public readonly void OnUpdate(ref SystemState state)
        {
            // Find the first intersections between particles' path and geometries
            var allGeometriesHandlers = new NativeArray<JobHandle>(1, Allocator.TempJob);

            var planeEntities = allGeometriesQuery.ToEntityArray(Allocator.TempJob);
            var planeData = allGeometriesQuery.ToComponentDataArray<Plane>(Allocator.TempJob);
            var planeGeometryJob = new PlaneGeometryJob
            {
                allPlaneEntities = planeEntities,
                allPlanes = planeData,
                timeMinimumBuffer = 1e-9 // TODO get from singleton
            };

            var planeGeometryHandler = planeGeometryJob.ScheduleParallel(allParticlesQuery, state.Dependency);
            planeEntities.Dispose(planeGeometryHandler);
            planeData.Dispose(planeGeometryHandler);

            allGeometriesHandlers[0] = planeGeometryHandler;

            // Move particles to their first insecting geometry to intersection for all particles and tag
            var allGeometriesHandlersCombined = JobHandle.CombineDependencies(allGeometriesHandlers);

            var CommandBuffer = new EntityCommandBuffer(Allocator.TempJob);

            var intersectionEndJob = new IntersectionEndJob
            {
                CommandBuffer = CommandBuffer.AsParallelWriter()
            };
            var intersectionEndHandler = intersectionEndJob.ScheduleParallel(allParticlesQuery, allGeometriesHandlersCombined);
            intersectionEndHandler.Complete();

            CommandBuffer.Playback(state.EntityManager);
            CommandBuffer.Dispose();


            allGeometriesHandlers.Dispose(intersectionEndHandler);
        }

        [BurstCompile]
        public readonly void OnDestroy(ref SystemState state) { }
    }

    [StructLayout(LayoutKind.Auto)]
    partial struct PlaneGeometryJob : IJobEntity
    {
        [ReadOnly] public NativeArray<Entity> allPlaneEntities;
        [ReadOnly] public NativeArray<Plane> allPlanes;
        [ReadOnly] public double timeMinimumBuffer;

        public void Execute(
            in Particle.Kinetic kinetic,
            in Particle.UniformForce force,
            ref Particle.Simulation simulation,
            ref Particle.IntersectingGeometryTemp intersectingGeometry,
            Particle.IntersectingGeometry lastIntersectingGeometry
        )
        {
            // Find all intersections with all Planes
            for (int i = 0; i < allPlaneEntities.Length; i++)
            {
                // Only look for new intersection
                if (lastIntersectingGeometry.entity != Entity.Null) continue; // TODO REMOVE THIS; NOT GENERAL; BUT GOOD FOR DEBUGGING


                //Debug.Log("kinetic: " + kinetic);
                double timeCandidate = allPlanes[i].TimeOfIntersection(kinetic, force);
                //Debug.Log("allPlanes[i]: " + allPlanes[i].position);
                //Debug.Log("timeCandidate: " + timeCandidate);

                if (timeCandidate < timeMinimumBuffer) continue; // only look forward, buffer needed in case of intersecting last geometry

                if (simulation.time2intersection < timeCandidate) continue; // first intersection only



                // Check if particle is colliding with the limited plane (non-infinite)
                var movedParticleTemp = force.Move(kinetic, timeCandidate);
                if (!GeometryHelper.planeCollision(movedParticleTemp.position, allPlanes[i])) continue;
                // Debug.Log("timeCandidate 3: " + timeCandidate);

                // Debug.Log("passed 3");
                // Debug.Log("kinetic: " + kinetic);
                // Debug.Log("force: " + force.AccelerationSum);

                // Debug.Log("movedParticleTemp: " + movedParticleTemp);


                // Update particle intersection information


                simulation.time2intersection = timeCandidate;
                intersectingGeometry.entity = allPlaneEntities[i];
            }
        }
    }

    [StructLayout(LayoutKind.Auto)]
    partial struct IntersectionEndJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter CommandBuffer;

        public void Execute(
            [ChunkIndexInQuery] int chunkIndex, Entity entity,
            ref Particle.Kinetic kinetic,
            in Particle.UniformForce force,
            in Particle.Simulation simulation,
            in Particle.IntersectingGeometryTemp intersectingGeometryTemp
        )
        {
            CommandBuffer.AddComponent<FlowMove>(chunkIndex, entity);

            // Check if any intersection was found
            if (intersectingGeometryTemp.entity == Entity.Null || simulation.time2intersection == double.MaxValue)
            {
                CommandBuffer.AddComponent<Particle.OutOfBounds>(chunkIndex, entity);
                CommandBuffer.RemoveComponent<Particle.InBounds>(chunkIndex, entity);
                return;
            }
            // Debug.Log("B kinetic: " + kinetic);
            // Debug.Log("time2Intersection.time2intersection " + simulation.time2intersection);

            // Move particle
            kinetic = force.Move(kinetic, simulation.time2intersection);
            // Debug.Log("A kinetic: " + kinetic);

            // Move intersecting geometry to shared component, so that it can be sorted later
            CommandBuffer.AddComponent<Particle.Intersecting>(chunkIndex, entity);
            CommandBuffer.SetSharedComponent<Particle.IntersectingGeometry>(chunkIndex, entity,
                new Particle.IntersectingGeometry
                {
                    entity = intersectingGeometryTemp.entity
                }
            );
        }
    }
}