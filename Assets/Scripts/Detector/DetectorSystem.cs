using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Jobs;
using System.Runtime.InteropServices;

using UnityEngine;
using System.Linq;

namespace Detector
{

    [BurstCompile]
    public partial struct DetectorCounter : IJobEntity
    {
        [ReadOnly] public NativeArray<Particle.Kinetic> kinetics;
        [ReadOnly] public NativeArray<Particle.IntersectingGeometryTemp> geometry;

        public void Execute(
            // [ChunkIndexInQuery] int chunkIndex,
            Entity entity,
            in Plane plane,
            DynamicBuffer<DetectorPixel> pixels,
            ref DetectorGrid detector
        )
        {


            for (int i = 0; i < kinetics.Length; i++)
            {
                if (geometry[i].entity != entity) continue;

                double2 relativePoint = plane.Project2Surface(kinetics[i].position);
                int2 detectorPixel = (int2)math.round(relativePoint * detector.PixelCount);

                detector.Set(detectorPixel, detector.Get(detectorPixel, pixels) + 1, pixels);
            }
            // // update range
            // double max = 0;
            // for (int i = 0; i < detector.All().Length; i++)
            // {
            //     if (detector.All()[i].count > max)
            //     {
            //         max = detector.All()[i].count;
            //         //Debug.Log("max: " + max);
            //     }
            // }
            // if (detector.Range.y < max) detector.Range = new double2(0, max);
        }
    }

    [UpdateInGroup(typeof(InteractionOfParticles))]
    [StructLayout(LayoutKind.Auto)]
    [BurstCompile]
    partial struct DetectorSystem : ISystem
    {
        EntityQuery allDetectorsQuery;
        EntityQuery allParticlesQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            allParticlesQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<Particle.ParticleTag,
                    Particle.Kinetic,
                    Particle.Properties,
                    Particle.particlePrefab,
                    Particle.IntersectingGeometryTemp, // TODO change this later
                    Particle.Simulation,
                    Particle.InBounds>()
                .WithNone<Particle.FlowInteraction>()
                .Build(ref state);

            // allDetectorsQuery = new EntityQueryBuilder(Allocator.Persistent).WithAll<DetectorTag, DetectorGrid, Plane>().Build(ref state);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) { }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var allKinetics = allParticlesQuery.ToComponentDataArray<Particle.Kinetic>(Allocator.TempJob);
            var allGeometry = allParticlesQuery.ToComponentDataArray<Particle.IntersectingGeometryTemp>(Allocator.TempJob);

            var DetectorCounterJob = new DetectorCounter
            {
                kinetics = allKinetics,
                geometry = allGeometry
            };
            DetectorCounterJob.ScheduleParallel();
        }
    }
}