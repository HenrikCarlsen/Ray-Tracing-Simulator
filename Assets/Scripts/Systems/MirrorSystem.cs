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

    public static double3 ReflectionInPlane(double3 x, Plane plane)
    {
        // https://math.stackexchange.com/questions/952092/reflect-a-point-about-a-plane-using-matrix-transformation
        // x_r = x-2*nbar*dot(x*nbar)

        double3 normal = plane.normal;

        double3 nbar = normal / math.length(normal);
        //Debug.Log("x: " + x + ",\t n: " + normal + ",\t r: " + (x - 2 * nbar * math.dot(x, nbar)));
        return x - 2 * nbar * math.dot(x, nbar);
    }


    [BurstCompile]
    public void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, 
        in ParticleGenerator generator, in Movement movement, in Plane plane, in MirrorSimple mirror,
        in ParticleHistory particleHistory)
    {

        // Reflect ParticleGenerator in the mirror:
        // TODO handle on plane mirrors
        var velocity = ReflectionInPlane(movement.velocity, plane);
        Movement newMovement = new Movement { position = movement.position, velocity = velocity };

        // create new particle
        var particle = CommandBuffer.Instantiate(chunkIndex, generator.particle);

        CommandBuffer.SetComponent<Movement>(chunkIndex, particle, newMovement);

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

    [BurstCompile]
    public void OnCreate(ref SystemState state) { 

        // Find all particle types
        // Find all surface types

        //state.EntityManager.GetAllArchetypes()
        //state.EntityManager.CreateArchetype()


    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        // Create particles for each source

        var job = new ReflectJob
        {
            CommandBuffer = ecb.AsParallelWriter()
        };
        state.Dependency = job.ScheduleParallel(state.Dependency);

    }
}