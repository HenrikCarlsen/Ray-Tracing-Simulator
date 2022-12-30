using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Jobs;

using UnityEngine;

[BurstCompile]
public partial struct DetectorDeleteGeneratorJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter CommandBuffer;

    public void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity,
        in Plane plane, in GeneratorOwner owner2,
        in Movement movement, in ParticleProperties particleProperties, in ParticleHistory particleHistory)
    {
        //detectorSimple.count += 1;
        CommandBuffer.DestroyEntity(chunkIndex, entity);
    }
}

[BurstCompile]
public partial struct DetectorCounter : IJobEntity
{

    [ReadOnly] public NativeArray<GeneratorOwner> allDetectorOwner;
    //[ReadOnly] public NativeArray<DetectorOwner> allDetectorGenerators;
    [ReadOnly] public NativeArray<Movement> allDetectorGeneratorsMovement;

    public void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, in Plane plane, ref DetectorGrid grid) // In detector
    {

        for (int i = 0; i < allDetectorOwner.Length; i++)
        {
            if (allDetectorOwner[i].owner != entity) break;

            // Update grid
            double2 relativePoint = plane.relativeSurfacePosition(allDetectorGeneratorsMovement[i].position);

            int2 detectorPixel = (int2)math.round(relativePoint * grid.pixelCount);
            grid.set(detectorPixel, grid.get(detectorPixel) + 1);

        }
        // update range
        double max = 0;
        for (int i = 0; i < grid.all().Length; i++)
        {
            if (grid.all()[i].count > max)
            {
                max = grid.all()[i].count;
                //Debug.Log("max: " + max);
            }
        }
        if (grid.range.y < max) grid.range = new double2(0, max);
    }
}

[UpdateAfter(typeof(HistorySystem))]
[BurstCompile]
partial struct DetectorSystem : ISystem
{
    EntityQuery allDetectorGenerators;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

        state.RequireForUpdate<ParticleGeneratorHolder>();
        allDetectorGenerators = new EntityQueryBuilder(Allocator.Persistent).WithAll<DetectorTag, GeneratorTag, GeneratorOwner, Movement>().Build(ref state);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var particlePrefab = SystemAPI.GetSingleton<ParticleGeneratorHolder>().particlePrefab;

        var jobCounter = new DetectorCounter
        {
            allDetectorOwner = allDetectorGenerators.ToComponentDataArray<GeneratorOwner>(Allocator.TempJob),
            //allDetectorGenerators = allDetectorGenerators.ToComponentDataArray<DetectorOwner>(Allocator.TempJob),
            allDetectorGeneratorsMovement = allDetectorGenerators.ToComponentDataArray<Movement>(Allocator.TempJob)
        };
        state.Dependency = jobCounter.ScheduleParallel(state.Dependency);

        var jobDelete = new DetectorDeleteGeneratorJob(){
            CommandBuffer = ecb.AsParallelWriter()
        };
        state.Dependency = jobDelete.ScheduleParallel(state.Dependency);
    }
}