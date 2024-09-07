using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Jobs;
using System.Runtime.InteropServices;

using UnityEngine;
using Particle;
using Unity.Scenes;

using Random = Unity.Mathematics.Random;

namespace Mirror
{


    [BurstCompile]
    [UpdateInGroup(typeof(InteractionOfParticles))]
    public partial struct ReflectJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter commandBuffer;

        public Plane plane;
        public SimpleMirror mirror;

        [BurstCompile]
        public readonly void Execute(
            [ChunkIndexInQuery] int chunkIndex,
            Entity entity,
            in Particle.Kinetic kinetic,
            in particlePrefab prefab,
            ref Particle.Simulation simulation
        )
        {
            // Find which side of the mirror is intersected
            double3 normal = plane.NormalOnPoint(kinetic.position);

            // The side is found using the sign of the determinant of the n and the velocity
            double det_n_v = math.dot(normal, kinetic.velocity);

            // If the determinant is very small this particle close to parallel with the mirror surface 
            // This is a state very sensitive to floating point errors and also unphysical 
            // (due to the surface always being a little uneven)
            if (math.abs(det_n_v) <= 1e-9)
            {
                simulation.state = Particle.Simulation.State.absorbNonPhysical;
                return;
            }

            // Side without mirror, normal points away from this side
            if (det_n_v < 0)
            {
                simulation.state = Particle.Simulation.State.absorb;
                return;
            }

            // Side with mirror, normal points towards this side
            // Reflect the velocity in the normal of the geometry
            // A particle hitting a perfect and smooth mirror is has its negative velocity mirrored in the normal of the point of intersection.
            // The outgoing velocity v_f is equal to the negative ingoing velocity v_i + 2 times the projection of v_i on the plane
            // v_f ​= 2*v_i' − v_i
            // Visual proof
            //
            //    v_i'
            //    ____     v_f
            //   \         /
            //    \   n   /
            //     \  ^  /
            //   v_i\ | /
            //   ____\|/____
            //   
            // To find v_i, we use that v_i is equal to to the ortogonal components
            // v_i = v_i' + v_i'_n, 
            // v_i'_n is the projection of v_i onto n and found by:
            // n * (v_i . n) /(n . n) 
            // see proof here https://en.wikipedia.org/wiki/Vector_projection
            // so v_i' = v_i - n * (v_i . n) / (n . n)
            double3 velocity_marked = kinetic.velocity - normal * math.dot(kinetic.velocity, normal) / math.dot(normal, normal);
            var newKinetic = new Particle.Kinetic
            {
                position = kinetic.position,
                velocity = 2 * velocity_marked - kinetic.velocity
            };

            var rng = new Random(123456 + (uint)entity.Index * 1453);

            int totalParticles = 5;


            if (simulation.weight < 0.01f) return; // TODO

            for (int i = 0; i < totalParticles; i++)
            {

                // Make particle
                var newParticleEntity = commandBuffer.Instantiate(chunkIndex, prefab.Value);
                commandBuffer.SetComponent<particlePrefab>(chunkIndex, newParticleEntity, prefab);


                var offset = rng.NextDouble3(-0.04, 0.04);

                newKinetic.velocity = newKinetic.velocity + offset;

                // Set the kinetics calcaluted above
                commandBuffer.SetComponent<Kinetic>(chunkIndex, newParticleEntity, newKinetic);

                // Copy the last particle simulation with the following exceptions:
                var newSimulation = simulation.Copy();
                newSimulation.time2intersection = double.MaxValue;
                newSimulation.generation = Simulation.Generation.mirror;
                newSimulation.weight = newSimulation.weight * 0.5f;
                newSimulation.lastParticle = entity;
                commandBuffer.SetComponent<Simulation>(chunkIndex, newParticleEntity, newSimulation);

                commandBuffer.AddComponent<Particle.FlowCreate>(chunkIndex, newParticleEntity);
                commandBuffer.AddComponent<Particle.FlowInteraction>(chunkIndex, entity);
            }
        }
    }

    [UpdateInGroup(typeof(InteractionOfParticles))]
    [StructLayout(LayoutKind.Auto)]
    [BurstCompile]
    partial struct MirrorSystem : ISystem
    {

        EntityQuery allParticlesQuery;

        EntityQuery allMirrorsQuery;


        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            allParticlesQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<Particle.ParticleTag,
                    Particle.Kinetic,
                    Particle.Properties,
                    Particle.particlePrefab,
                    Particle.IntersectingGeometry,
                    Particle.Simulation,
                    Particle.InBounds>()
                .WithNone<Particle.FlowInteraction>()
                .Build(ref state);


            allMirrorsQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<SimpleMirror, Plane>().Build(ref state);
        }

        [BurstCompile]
        public readonly void OnDestroy(ref SystemState state) { }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var allMirrorsEntities = allMirrorsQuery.ToEntityArray(Allocator.Temp);
            var allMirrorsMirror = allMirrorsQuery.ToComponentDataArray<SimpleMirror>(Allocator.Temp);
            var allMirrorsPlane = allMirrorsQuery.ToComponentDataArray<Plane>(Allocator.Temp);

            var CommandBuffer = new EntityCommandBuffer(Allocator.TempJob);

            for (int i = 0; i < allMirrorsEntities.Length; i++)
            {
                allParticlesQuery.ResetFilter();
                // Query only the particles intersecting the mirror
                allParticlesQuery.AddSharedComponentFilter<Particle.IntersectingGeometry>
                (
                    new Particle.IntersectingGeometry { entity = allMirrorsEntities[i] }
                );

                var reflectJob = new ReflectJob
                {
                    commandBuffer = CommandBuffer.AsParallelWriter(),
                    mirror = allMirrorsMirror[i],
                    plane = allMirrorsPlane[i]
                };
                var reflectHandler = reflectJob.ScheduleParallel(allParticlesQuery, state.Dependency);
                reflectHandler.Complete();

            }
            CommandBuffer.Playback(state.EntityManager);
            CommandBuffer.Dispose();
        }
    }
}