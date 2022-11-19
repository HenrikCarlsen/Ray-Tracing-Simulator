// using Unity.Burst;
// using Unity.Entities;
// using Unity.Mathematics;
// using Unity.Transforms;
// using Unity.Collections;
// using Unity.Jobs;

// using UnityEngine;

// [BurstCompile]
// public partial struct CalculateForcesJob : IJobEntity
// {

//     [BurstCompile]
//     public void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, ref Force force)
//     {
//         force.AccelerationSum = new float3(0, -0, 0);
//     }

// }



// [BurstCompile]
// partial struct ForceSystem : ISystem
// {


//     [BurstCompile]
//     public void OnCreate(ref SystemState state) { }

//     [BurstCompile]
//     public void OnDestroy(ref SystemState state) { }

//     [BurstCompile]
//     public void OnUpdate(ref SystemState state)
//     {
//         var job = new CalculateForcesJob { };
//         state.Dependency = job.ScheduleParallel(state.Dependency);
//     }
// }