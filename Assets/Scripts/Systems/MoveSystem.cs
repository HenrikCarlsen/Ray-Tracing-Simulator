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

    public static double3 ReflectionInPlane(double3 x, Plane plane)
    {
        // https://math.stackexchange.com/questions/952092/reflect-a-point-about-a-plane-using-matrix-transformation
        // x_r = x-2*nbar*dot(x*nbar)

        double3 normal = plane.normal;
        
        double3 nbar = normal / math.length(normal);
        Debug.Log("x: " + x + ",\t n: " + normal + ",\t r: " + ( x - 2 * nbar * math.dot(x, nbar)));
        return x - 2 * nbar * math.dot(x, nbar);
    }

    public static Movement moveInUniformField(Movement movement, double3 acceleration, double time)
    {
        movement.position += movement.velocity * time + acceleration * time * time;
        movement.velocity += acceleration * time;
        return movement;
    }

    public void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, ref Movement movement)
    {
        //Debug.Log("allMirrors.Length: " + allMirrors.Length);

        double tMin = double.MaxValue;
        int i_tMin = -1;
        for (int i = 0; i < allMirrors.Length; i++)
        {
            // Debug.Log("i: " + i);

            Plane dummy = allMirrors[i];
            double tTemp = GeometryHelper.timeOfCollision(movement, acceleration, dummy);

            var atemp = moveInUniformField(movement, acceleration, tTemp);
            // if( GeometryHelper.planeCollision(atemp.position, dummy) ){
                
            // }



            // Enforce mininum t avoid collision with the surface the particle just exited 
            if (1e-9 < tTemp && tTemp < tMin && GeometryHelper.planeCollision(atemp.position, dummy))
            {
                tMin = tTemp;
                i_tMin = i;
            }
        }

        if (i_tMin == -1) return;
        // Debug.Log("FROM movement: " + movement.ToString());

        // Move particle to point of collision and reflect it's velocity
        movement = moveInUniformField(movement, acceleration, tMin);
        movement.velocity = ReflectionInPlane(movement.velocity, allMirrors[i_tMin]);

        // Debug.Log("TO   movement: " + movement.ToString());


        // TODO make new particle
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

        allParticleTag = new EntityQueryBuilder(Allocator.Persistent).WithAll<ParticleTag>().Build(ref state);


        state.RequireForUpdate<Gravity>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {

        var gravity = SystemAPI.GetSingleton<Gravity>();

        var job = new PlaneMirrorParticleInteractionJob
        {
            allMirrorsEntity = allMirrorsQuery.ToEntityArray(Allocator.TempJob),
            allMirrors = allMirrorsQuery.ToComponentDataArray<Plane>(Allocator.TempJob),
            acceleration = gravity.acceleration
        };

        job.ScheduleParallel(allParticleTag);


        state.CompleteDependency();
    }
}