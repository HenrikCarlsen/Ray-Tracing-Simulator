// using Unity.Burst;
// using Unity.Entities;
// using Unity.Mathematics;
// using Unity.Transforms;
// using Unity.Collections;
// using Unity.Jobs;

// using UnityEngine;

// [UpdateAfter(typeof(HistorySystem))]
// [BurstCompile]
// partial struct DetectorSystem : ISystem
// {
//     EntityQuery allMirrorsQuery;

//     EntityQuery allParticleTag;
    
//     ComponentLookup<MirrorTag> GetPlane;

//     [BurstCompile]
//     public void OnCreate(ref SystemState state){}

//     [BurstCompile]
//     public void OnDestroy(ref SystemState state) { }

//     [BurstCompile]
//     public void OnUpdate(ref SystemState state){}
// }

// //


// D

// Detector:

// -DetectorTag
// -SimpleDetector
// 	Size float2
// 	Pixel count int2
// 	Filename
// -Movement
// -Plane

// Show result in unity

// Set pixel value on texture

