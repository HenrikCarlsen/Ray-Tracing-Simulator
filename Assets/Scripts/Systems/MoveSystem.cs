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
    [ReadOnly] public NativeArray<Plane> allMirrors;
    public double3 acceleration;

    public EntityCommandBuffer.ParallelWriter CommandBuffer;

    [ReadOnly] public Entity particleGeneratorPrefab;

    public static Movement moveInUniformField(Movement movement, double3 acceleration, double time)
    {
        movement.position += movement.velocity * time + acceleration * time * time;
        movement.velocity += acceleration * time;
        return movement;
    }

    public void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, ref Movement movement, in ParticleHistory particleHistory)
    {


        double tMin = double.MaxValue;
        int i_tMin = -1;
        for (int i = 0; i < allMirrors.Length; i++)
        {
            Plane dummy = allMirrors[i];
            double tTemp = GeometryHelper.timeOfCollision(movement, acceleration, dummy);

            var movedParticleTemp = moveInUniformField(movement, acceleration, tTemp);

            // Enforce mininum t avoid collision with the surface the particle just exited 
            if (1e-9 < tTemp && tTemp < tMin && GeometryHelper.planeCollision(movedParticleTemp.position, dummy))
            {
                tMin = tTemp;
                i_tMin = i;
            }
        }

        if (i_tMin == -1) return;

        // Move particle to collided part
        movement = moveInUniformField(movement, acceleration, tMin);

        // Make new particle generator
        var particleGenerator = CommandBuffer.Instantiate(chunkIndex,particleGeneratorPrefab);
        CommandBuffer.SetComponent<Movement>(chunkIndex,particleGenerator,movement);
        CommandBuffer.SetComponent<Plane>(chunkIndex,particleGenerator,allMirrors[i_tMin]);

        //Debug.Log("particleHistory 2: " + particleHistory.lastHistoryPoint);
        CommandBuffer.SetComponent<ParticleHistory>(chunkIndex,particleGenerator,particleHistory);

        // Destroy current particle
        CommandBuffer.DestroyEntity(chunkIndex,entity);


        ///////////////////// ALTERNATIVE MODEL (DYNAMIC)

        // Make entity from archetype on plane
        // Fill out the information on this archtype
            // Plane and particle 
            // TODO handle case where it is a not plane


        // Get archetype from prefab and particle prefab
        // Create entity from archetype
        // Make mirror and detector archetype in the component authoring




        /////////////////////
    }
}

[BurstCompile]
partial struct MoveClassicallySystem : ISystem
{
    EntityQuery allMirrorsQuery;

    EntityQuery allParticleTag;
    
    ComponentLookup<MirrorTag> GetPlane;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        allMirrorsQuery = new EntityQueryBuilder(Allocator.Persistent).WithAll<MirrorTag, Plane>().Build(ref state);
        GetPlane = state.GetComponentLookup<MirrorTag>(true);

        allParticleTag = new EntityQueryBuilder(Allocator.Persistent).WithAll<ParticleTag>().WithAllRW<Movement>().Build(ref state);


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
            allMirrorsEntity = allMirrorsQuery.ToEntityArray(Allocator.TempJob),
            allMirrors = allMirrorsQuery.ToComponentDataArray<Plane>(Allocator.TempJob),
            acceleration = gravity.acceleration,
            CommandBuffer = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
            particleGeneratorPrefab = generatorHolder.generatorPrefab
        };

        job.ScheduleParallel(allParticleTag);

        state.CompleteDependency();

    }
}