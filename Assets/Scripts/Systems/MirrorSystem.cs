using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Jobs;

using UnityEngine;

[BurstCompile]
public partial struct ReflectJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter CommandBuffer;

    public int generateCount;

    public Entity ParticlePrefab;

    public static double3 ReflectionInPlane(double3 x, Plane plane)
    {
        double3 nbar = plane.normal / math.length(plane.normal);
        // https://math.stackexchange.com/questions/952092/reflect-a-point-about-a-plane-using-matrix-transformation
        // x_r = x-2*nbar*dot(x*nbar)
        return x - 2 * nbar * math.dot(x, nbar);
    }

    [BurstCompile]
    public void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity,
        in Plane plane,
        in Movement movement, in ParticleProperties particleProperties, in ParticleHistory particleHistory)
    {

        // Reflect ParticleGenerator in the mirror:
        var velocity = ReflectionInPlane(movement.velocity, plane);
        Movement newMovement = new Movement { position = movement.position, velocity = velocity };

        // create new particle
        var particle = CommandBuffer.Instantiate(chunkIndex, ParticlePrefab);

        CommandBuffer.SetComponent<Movement>(chunkIndex, particle, newMovement);
        CommandBuffer.SetSharedComponent<ParticleProperties>(chunkIndex, particle, particleProperties);

        //Debug.Log("particleHistory 1: " + particleHistory.lastHistoryPoint);
        CommandBuffer.SetComponent<ParticleHistory>(chunkIndex, particle, particleHistory);

        // destroy this generator
        CommandBuffer.DestroyEntity(chunkIndex, entity);
    }
}


[UpdateBefore(typeof(MoveClassicallySystem))]
[BurstCompile]
partial struct MirrorSystem : ISystem
{

    EntityQuery allMirrorGenerators;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ParticleGeneratorHolder>();
        allMirrorGenerators = new EntityQueryBuilder(Allocator.Persistent).WithAll<MirrorTag, GeneratorTag>().Build(ref state);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var particlePrefab = SystemAPI.GetSingleton<ParticleGeneratorHolder>().particlePrefab;

        // Create particles for each source
        var job = new ReflectJob
        {
            CommandBuffer = ecb.AsParallelWriter(),
            ParticlePrefab = particlePrefab
        };
        state.Dependency = job.ScheduleParallel(allMirrorGenerators, state.Dependency);
    }
}