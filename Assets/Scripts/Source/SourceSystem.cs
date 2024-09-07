using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Jobs;
using System.Runtime.InteropServices;

using UnityEngine;
using System;
using UnityEngine.UIElements;
using Particle;


namespace Source
{
    [BurstCompile]
    [StructLayout(LayoutKind.Auto)]
    [UpdateInGroup(typeof(CreateParticles))]
    partial struct SourceSystem : ISystem
    {

        EntityQuery queryParticle;
        public EntityCommandBuffer.ParallelWriter CommandBuffer;

        [ReadOnly] public ComponentLookup<Plane> planeLookup;

        public int particleLimit;

        public bool oneParticle;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            queryParticle = new EntityQueryBuilder(Allocator.Temp).WithAll<Particle.ParticleTag>().Build(ref state);
            planeLookup = state.GetComponentLookup<Plane>(true);

            particleLimit = 1;
            oneParticle = true;
        }

        [BurstCompile] public readonly void OnDestroy(ref SystemState state) { }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            int particlesNeeded = particleLimit - queryParticle.CalculateEntityCount();

            if (oneParticle == false) return;
            if (oneParticle == true) oneParticle = false;

            if (particlesNeeded <= 0) return;

            var CommandBuffer = new EntityCommandBuffer(Allocator.TempJob);

            planeLookup.Update(ref state);

            var spawnParticlesJob = new SpawnParticlesJob
            {
                CommandBuffer = CommandBuffer.AsParallelWriter(),
                numberOfParticle2create = particlesNeeded,
                planeLookup = planeLookup
            };
            var spawnParticlesHandler = spawnParticlesJob.ScheduleParallel(state.Dependency);
            spawnParticlesHandler.Complete();


            CommandBuffer.Playback(state.EntityManager);
            CommandBuffer.Dispose();
        }
    }

    [BurstCompile]
    [StructLayout(LayoutKind.Auto)]
    partial struct SpawnParticlesJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter CommandBuffer;

        public int numberOfParticle2create;

        [ReadOnly] public ComponentLookup<Plane> planeLookup;

        public void Execute(
            Entity entity,
            [ChunkIndexInQuery] int chunkIndex,
            ref SourceSimple source
        )
        {
            if (source.random.state == 0)
                source.random = new Unity.Mathematics.Random(source.randomSeed + (uint)chunkIndex + 1);

            for (int i = 0; i < numberOfParticle2create; i++)
            {
                // Make RandomPointOnSurface algorithm
                var pointOnSource = source.sourceSurface.position; // new double3(0, 0, 0); //source.sourceSurface.RandomPointOnSurface();
                var pointOnTarget = source.sourceTarget.position; //new double3(2, 0, 0); // source.sourceTarget.RandomPointOnSurface();

                var velocityNorm = (pointOnTarget - pointOnSource) / math.length(pointOnTarget - pointOnSource);
                var randomNumber = source.random.NextDouble(source.velocityRange.x, source.velocityRange.y);
                Debug.Log("-------------------------:");

                // Debug.Log("pointOnSource: " + pointOnSource);
                // Debug.Log("pointOnTarget: " + pointOnTarget);
                // Debug.Log("velocityNorm: " + velocityNorm);
                // Debug.Log("randomNumber: " + randomNumber);

                var velocity = velocityNorm * randomNumber;



                // Make particle
                var newParticleEntity = CommandBuffer.Instantiate(chunkIndex, source.particle);
                CommandBuffer.SetComponent<particlePrefab>(chunkIndex, newParticleEntity, new Particle.particlePrefab { Value = source.particle });

                // Set the kinetics calcaluted above
                CommandBuffer.SetComponent<Kinetic>(chunkIndex, newParticleEntity,
                    new Particle.Kinetic
                    {
                        position = pointOnSource,
                        velocity = velocity
                    }
                );

                // Copy the last particle simulation with the following exceptions:
                CommandBuffer.SetComponent<Simulation>(chunkIndex, newParticleEntity,
                    new Simulation
                    {
                        time2intersection = double.MaxValue,
                        generation = Simulation.Generation.source,
                        lastParticle = Entity.Null,
                        weight = 1
                    }
                );


                CommandBuffer.AddComponent<Particle.FlowCreate>(chunkIndex, newParticleEntity);
            }
        }
    }
}