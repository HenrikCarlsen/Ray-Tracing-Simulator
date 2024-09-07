// // after every collision a collisionHistory entity is made (at the start of the collision) and 
// using Unity.Burst;
// using Unity.Entities;
// using Unity.Mathematics;
// using Unity.Transforms;
// using Unity.Collections;
// using Unity.Jobs;
// using UnityEngine;

// using Unity.Rendering;
// using System;

// public partial struct AlignTransformWithMovement : IJobEntity
// {
//     public void Execute(ref LocalToWorldTransform transform, in Particle.Kinetic movement)
//     {
//         transform.Value.Position = (float3)movement.position;
//         //transform.Value.Rotation = quaternion.LookRotation()
//     }
// }

// public partial struct AlignTransformWithMovementForPath : IJobEntity
// {
//     public void Execute(ref LocalToWorldTransform transform, in Particle.Kinetic movement, in History.ParticleHistoryPath path)
//     {
//         float3 p = (float3)(path.after) - (float3)movement.position;

//         transform.Value.Rotation = quaternion.LookRotationSafe(p, math.right());
//         transform.Value.Scale = math.length((float3)(path.after - path.before)) / 2.0f;
//     }
// }

// //[UpdateAfter(typeof(History.HistorySystem))]

// [BurstCompile]
// partial struct GraphicSystem : ISystem
// {
//     EntityQuery allGraphicEntities;
//     EntityQuery allGraphicWithPathEntities;

//     EntityQuery allDetectors;

//     [BurstCompile]
//     public void OnCreate(ref SystemState state)
//     {
//         allGraphicEntities = new EntityQueryBuilder(Allocator.Persistent).WithAll<LocalToWorldTransform, Particle.Kinetic>().Build(ref state);
//         allGraphicEntities.SetChangedVersionFilter(typeof(Particle.Kinetic));

//         allGraphicWithPathEntities = new EntityQueryBuilder(Allocator.Persistent).WithAll<LocalToWorldTransform, Particle.Kinetic, History.ParticleHistoryPath>().Build(ref state);
//         allGraphicWithPathEntities.SetChangedVersionFilter(typeof(Particle.Kinetic));

//         allDetectors = new EntityQueryBuilder(Allocator.Persistent).WithAll<Detector.DetectorTag, GeneratorBaseTag, RenderMeshArray>().Build(ref state);
//     }

//     [BurstCompile]
//     public void OnDestroy(ref SystemState state) { }

//     [BurstCompile]
//     public void OnUpdate(ref SystemState state)
//     {
//         var jobAlign = new AlignTransformWithMovement();
//         jobAlign.ScheduleParallel(allGraphicEntities, state.Dependency);

//         var jobAlignPath = new AlignTransformWithMovementForPath();
//         jobAlignPath.ScheduleParallel(allGraphicWithPathEntities, state.Dependency);

//         var entities = allDetectors.ToEntityArray(Allocator.Temp);
//         for (int i = 0; i < entities.Length; i++)
//         {
//             // MaterialMeshInfo
//             var RMA = state.EntityManager.GetSharedComponentManaged<RenderMeshArray>(entities[i]);
//             var grid = state.EntityManager.GetComponentData<Detector.DetectorGrid>(entities[i]);

//             // Find the detector's material
//             Material detectorMaterial = null;

//             int testIndex = 0;
//             for (int j = 0; j < RMA.Materials.Length; j++)
//             {
//                 if (grid.MaterialID == RMA.Materials[j].GetInstanceID())
//                 {
//                     detectorMaterial = RMA.Materials[j];
//                     testIndex = j;
//                     //break;
//                 }
//             }
//             if (detectorMaterial == null) continue;

//             int xLength = grid.pixelCount.x;
//             int yLength = grid.pixelCount.y;

//             var texture = new Texture2D(xLength, yLength, TextureFormat.ARGB32, false);
//             texture.filterMode = FilterMode.Point;

//             double textureRangeMax = 1;
//             double rangeMax = grid.range.y;

//             detectorMaterial.SetVector("_GraphicScale", new Vector4(grid.pixelCount.x, grid.pixelCount.y, 0, 0));

//             for (int x = 0; x < xLength; x++)
//             {
//                 for (int y = 0; y < yLength; y++)
//                 {
//                     double value = grid.get(new int2(x, y));

//                     if (rangeMax == 0 || value == 0)
//                     {
//                         texture.SetPixel(x, y, new Color(0, 0, 0, 0));
//                         continue;
//                     }

//                     // Rescale to texture range: 
//                     if (grid.scale == Detector.DetectorGrid.Scale.linear)
//                     {
//                         value = value * (textureRangeMax / rangeMax);
//                         texture.SetPixel(x, y, new Color((float)value, (float)value, (float)value, 1));
//                     }
//                     if (grid.scale == Detector.DetectorGrid.Scale.log)
//                     {
//                         value = math.log(1 + value);
//                         value = value * (textureRangeMax / math.log(1 + rangeMax));
//                         texture.SetPixel(x, y, new Color((float)value, (float)value, (float)value, 1));
//                     }
//                 }
//             }
//             // Send the texture and material to the graphics card.
//             texture.Apply();
//             detectorMaterial.SetTexture("_ParticleDetectedImage", texture);
//         }
//         state.CompleteDependency();
//     }
// }