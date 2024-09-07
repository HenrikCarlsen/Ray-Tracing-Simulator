// after every collision a collisionHistory entity is made (at the start of the collision) and 
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using System.Runtime.InteropServices;

using Unity.Rendering;
using System;
using History;
using Particle;

namespace Graphics
{

    [BurstCompile]
    partial struct GraphicSystem : ISystem
    {
        [ReadOnly] public ComponentLookup<LocalTransform> localTransformLookup;
        [ReadOnly] public ComponentLookup<Simulation> simulationLookup;

        EntityQuery HistoryPoints;
        EntityQuery HistoryPaths;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            HistoryPoints = new EntityQueryBuilder(Allocator.Persistent)
                .WithAll<HistoryPoint, HistoryPast, HistoryPathGraphics>()
                .Build(ref state);
            HistoryPoints.SetChangedVersionFilter(typeof(HistoryPoint));

            HistoryPaths = new EntityQueryBuilder(Allocator.Persistent)
                .WithAll<HistoryPath>()
                .Build(ref state);

            localTransformLookup = state.GetComponentLookup<LocalTransform>(true);
            simulationLookup = state.GetComponentLookup<Simulation>(true);

        }

        [BurstCompile]
        readonly public void OnDestroy(ref SystemState state) { }

        //[BurstCompile]
        public void OnUpdate(ref SystemState state)
        {

            // Find all updated history points
            // Run SpawnPipesJob
            var CommandBuffer = new EntityCommandBuffer(Allocator.TempJob);

            localTransformLookup.Update(ref state);
            simulationLookup.Update(ref state);

            var spawnPipesJob = new SpawnPipesJob
            {
                CommandBuffer = CommandBuffer,
                HistoryPathEntities = HistoryPaths.ToEntityArray(Allocator.TempJob),
                HistoryPaths = HistoryPaths.ToComponentDataArray<HistoryPath>(Allocator.TempJob),
                localTransformLookup = localTransformLookup,
                simulationLookup = simulationLookup
            };
            var spawnPipesHandler = spawnPipesJob.Schedule(HistoryPoints, state.Dependency);
            spawnPipesHandler.Complete();

            CommandBuffer.Playback(state.EntityManager);
            CommandBuffer.Dispose();

        }
    }

    // Make pipe between each history point
    // Rescale alle pipes based on the distance between the point
    // global scale
    // global transparenty

    [BurstCompile]
    [StructLayout(LayoutKind.Auto)]
    partial struct SpawnPipesJob : IJobEntity
    {
        public EntityCommandBuffer CommandBuffer;

        public NativeArray<Entity> HistoryPathEntities;
        public NativeArray<HistoryPath> HistoryPaths;

        [ReadOnly] public ComponentLookup<Simulation> simulationLookup;

        [ReadOnly] public ComponentLookup<LocalTransform> localTransformLookup;

        public void Execute(
            Entity entity,
            in HistoryPoint point,
            in HistoryPathGraphics historyGraphics,
            in HistoryPast pointPast
        )
        {

            // if they don't have a past point, continue
            if (pointPast.point == Entity.Null) return;

            // Get transform information from the history points TODO use world transform?!?!


            // Remove any existing pipe using this point
            for (int i = 0; i < HistoryPaths.Length; i++)
            {
                if (HistoryPaths[i].point == entity) CommandBuffer.DestroyEntity(HistoryPathEntities[i]);
            }
            // Debug.Log("historyGraphics.prefab: " + historyGraphics.prefab);

            // // Make pipe
            var newPipe = CommandBuffer.Instantiate(historyGraphics.prefab);
            CommandBuffer.SetComponent<HistoryPath>(newPipe, new HistoryPath { point = entity });


            var transformPast = localTransformLookup[pointPast.point];
            var transformCurrent = localTransformLookup[entity];
            var middlePoint = transformPast.Position + (transformCurrent.Position - transformPast.Position) / 2.0f;




            var rotation = TransformHelpers.LookAtRotation(transformPast.Position, transformCurrent.Position, math.up());

            var a = quaternion.RotateX(math.radians(90.0f));
            rotation = math.mul(rotation, a);
            //rotation = TransformHelpers.TransformRotation()


            CommandBuffer.SetComponent<LocalTransform>(newPipe,
                new LocalTransform
                {
                    Position = middlePoint,
                    Rotation = rotation,
                    Scale = 1.0f
                }
            );


            var distrance = math.distance(transformPast.Position, transformCurrent.Position);


            var thickness = simulationLookup[point.particle].weight * 0.25f;

            // Debug.Log("thickness: " + thickness);

            CommandBuffer.SetComponent<PostTransformMatrix>(newPipe,
                new PostTransformMatrix
                { Value = float4x4.Scale(thickness, distrance * 0.5f, thickness) } // TODO set magic numbers from particle simulation weight
            );


            // Debug.Log("newPipe: " + newPipe);


            // var middlePoint = transformPast.Position + (transformCurrent.Position - transformPast.Position) / 2.0f;
            // Debug.Log("middlePoint: " + middlePoint);


            // Find the scale of the pipe
            // Find the postScale


        }
    }
}

