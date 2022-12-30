using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Jobs;

using UnityEngine;


public partial struct PlaneMirrorParticleInteractionJob : IJobEntity
{
    [ReadOnly] public NativeArray<Entity> allMirrorsEntity;
    [ReadOnly] public NativeArray<Plane> allMirrorsPlane;
    [ReadOnly] public NativeArray<GeneratorHolder> allGenerators;

    public double3 acceleration;

    public EntityCommandBuffer.ParallelWriter CommandBuffer;

    public static Movement moveInUniformField(Movement movement, double3 acceleration, double time)
    {
        movement.position += movement.velocity * time + acceleration * time * time;
        movement.velocity += acceleration * time;
        return movement;
    }

    public void Execute(
        [ChunkIndexInQuery] int chunkIndex, Entity entity,
        ref Movement movement, in ParticleHistory particleHistory, in ParticleProperties particleProperties
        )
    {

        // Find all intersections with all planes
        double tMin = double.MaxValue;
        int i_tMin = -1;
        for (int i = 0; i < allGenerators.Length; i++)
        {
            Plane dummy = allMirrorsPlane[i];
            double tTemp = GeometryHelper.timeOfCollision(movement, acceleration, dummy);

            var movedParticleTemp = moveInUniformField(movement, acceleration, tTemp);

            // Enforce mininum t avoid collision with the surface the particle just exited 
            if (1e-9 < tTemp && tTemp < tMin && GeometryHelper.planeCollision(movedParticleTemp.position, dummy))
            {
                tMin = tTemp;
                i_tMin = i;
            }
        }
        if (i_tMin == -1)
        {
            CommandBuffer.DestroyEntity(chunkIndex, entity);
            return;
        }

        // Move particle to collided part
        movement = moveInUniformField(movement, acceleration, tMin);

        // Make a generator based on the intersected thing


        // Make new particle generator
        var particleGenerator = CommandBuffer.Instantiate(chunkIndex, allGenerators[i_tMin].generator);
        CommandBuffer.SetComponent<GeneratorOwner>(chunkIndex, particleGenerator, new GeneratorOwner { owner = allMirrorsEntity[i_tMin] });

        // Particle fill:
        CommandBuffer.SetComponent<Movement>(chunkIndex, particleGenerator, movement);
        CommandBuffer.SetSharedComponent<ParticleProperties>(chunkIndex, particleGenerator, particleProperties);

        // Set Plane
        CommandBuffer.SetComponent<Plane>(chunkIndex, particleGenerator, allMirrorsPlane[i_tMin]);

        // Set History
        CommandBuffer.SetComponent<ParticleHistory>(chunkIndex, particleGenerator, particleHistory);

        // Destroy current particle
        CommandBuffer.DestroyEntity(chunkIndex, entity);
    }
}

[BurstCompile]
partial struct MoveClassicallySystem : ISystem
{
    EntityQuery allObjectsQuery;

    EntityQuery allObjectsTag;


    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // allMirrorsQuery = new EntityQueryBuilder(Allocator.Persistent).WithAll<MirrorTag, Plane, GeneratorHolder>().Build(ref state);
        // allParticleTag = new EntityQueryBuilder(Allocator.Persistent).WithAll<ParticleTag>().WithAllRW<Movement>().Build(ref state);
        allObjectsQuery = new EntityQueryBuilder(Allocator.Persistent).WithAll<Plane, GeneratorHolder>().Build(ref state);
        allObjectsTag = new EntityQueryBuilder(Allocator.Persistent).WithAll<ParticleTag>().WithAllRW<Movement>().Build(ref state);


        state.RequireForUpdate<Gravity>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();

        var gravity = SystemAPI.GetSingleton<Gravity>();
        var generatorHolder = SystemAPI.GetSingleton<ParticleGeneratorHolder>();

        var job = new PlaneMirrorParticleInteractionJob
        {
            allMirrorsEntity = allObjectsQuery.ToEntityArray(Allocator.TempJob),
            allMirrorsPlane = allObjectsQuery.ToComponentDataArray<Plane>(Allocator.TempJob),
            allGenerators = allObjectsQuery.ToComponentDataArray<GeneratorHolder>(Allocator.TempJob),
            acceleration = gravity.acceleration,
            CommandBuffer = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
        };
        job.ScheduleParallel(allObjectsTag);

        state.CompleteDependency();

    }
}