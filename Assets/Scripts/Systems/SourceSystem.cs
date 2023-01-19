using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Jobs;

using UnityEngine;



[BurstCompile]
public partial struct CreateParticleJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter CommandBuffer;

    public int generateCount;

    [BurstCompile]
    public void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, ref Source source)
    {
        if (source.random.state == 0)
            source.random = new Unity.Mathematics.Random(source.randomSeed + (uint)chunkIndex + 1);

        for (int i = 0; i < generateCount; i++)
        {
            //Debug.Log("source.random: " + source.random.NextDouble3(0.1,0.2));
            // Make particle
            var particle = CommandBuffer.Instantiate(chunkIndex, source.particle);
            // Set velocity in Movement
            CommandBuffer.SetComponent<Movement>(chunkIndex, particle,
                new Movement { position = source.startPosition, velocity = source.startVelocity + source.random.NextDouble(-0.1, 0.1)*new double3(0,1,0) }
            );
        }
    }
}

[UpdateBefore(typeof(MoveClassicallySystem))]
[BurstCompile]
partial struct SourceSystem : ISystem
{

    EntityQuery queryParticle;

    bool runOnce;
    bool shouldBeRunning;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        queryParticle = new EntityQueryBuilder(Allocator.Temp).WithAll<ParticleTag>().Build(ref state);
        runOnce = true;
        shouldBeRunning = true;
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    { }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Simple Check for testing
        if (!shouldBeRunning) return;
        if (runOnce) shouldBeRunning = false;

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        // Keep a limited number of particles in the simulation
        var currentParticleCount = queryParticle.ToEntityArray(Allocator.Temp);
        int currentParticleCountTarget = 1;
        if (currentParticleCount.Length < currentParticleCountTarget)
        {
            int targetGenerateRate = 1;
            if (currentParticleCount.Length - currentParticleCountTarget < targetGenerateRate)
                targetGenerateRate = currentParticleCountTarget - currentParticleCount.Length;

            // Create particles for each source
            var job = new CreateParticleJob
            {
                generateCount = targetGenerateRate,
                CommandBuffer = ecb.AsParallelWriter()
            };
            state.Dependency = job.ScheduleParallel(state.Dependency);
        }
    }
}