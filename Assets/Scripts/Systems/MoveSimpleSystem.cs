// using Unity.Burst;
// using Unity.Entities;
// using Unity.Mathematics;
// using Unity.Transforms;
// using Unity.Collections;
// using Unity.Jobs;

// using UnityEngine;

// [BurstCompile]
// public partial struct MoveParticleClassicallyJob : IJobEntity
// {
//     public float time;

//     [BurstCompile]
//     public void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, ref Movement movement, in Force force)
//     {
//         movement.position += movement.velocity*time + force.AccelerationSum*time*time;
//         movement.velocity += force.AccelerationSum*time;
//     }

// }

// // public partial struct UpdateParticlePath : IJobEntity
// // {
// //     //public DynamicBuffer<ParticlePath> buffer;

// //     public void Execute(DynamicBuffer<ParticlePath> buffer, in Movement movement)
// //     {
// //         buffer.Add(new ParticlePath{position = movement.position});
// //     }
// // }



// public partial struct AlignTransformWithMovement : IJobEntity
// {
//     public void Execute(ref LocalToWorldTransform transform, in Movement movement)
//     {
//         transform.Value.Position = movement.position;
//     }
// }

// [BurstCompile]
// partial struct MoveClassicallySystem : ISystem
// {

//     [BurstCompile]
//     public void OnCreate(ref SystemState state){}

//     [BurstCompile]
//     public void OnDestroy(ref SystemState state){}

//     [BurstCompile]
//     public void OnUpdate(ref SystemState state)
//     {
//         float deltaTime = Time.deltaTime; //TODO is this the correct way to get time?
//         var job = new MoveParticleClassicallyJob{
//             time = SystemAPI.Time.DeltaTime
//         };
//         job.ScheduleParallel(state.Dependency);

//         // Update particle path with the movement information
//         // var jobPath = new UpdateParticlePath();
//         // jobPath.Schedule(state.Dependency);

//         // Align unity the transform with movement component (used for graphic only)
//         var jobAlign = new AlignTransformWithMovement();
//         jobAlign.ScheduleParallel(state.Dependency);
//     }
// }