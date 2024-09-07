using Unity.Entities;
using UnityEngine;

using Unity.Mathematics;
using System;
using UnityEngine.UIElements;
using Unity.Collections;
using Unity.Transforms;
namespace Source
{

    // Authoring MonoBehaviours are regular GameObject components.
    // They constitute the inputs for the baking systems which generates ECS data.
    public class SourceAuthoring : UnityEngine.MonoBehaviour
    {
        public GameObject sourceSurface;
        public GameObject sourceTarget;
        public double2 velocityRange;

        public GameObject particle;

        public int randomSeed;
    }

    // Bakers convert authoring MonoBehaviours into entities and components.
    public class SourceBaker : Baker<SourceAuthoring>
    {
        public override void Bake(SourceAuthoring authoring)
        {
            AddComponent<SourceSimple>(
            new SourceSimple
            {
                particle = GetEntity(authoring.particle),
                velocityRange = authoring.velocityRange,
                randomSeed = (uint)authoring.randomSeed,
            }
            );
        }
    }

    // [WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)]
    // partial struct SourceBakingSystem : ISystem
    // {
    //     public readonly void OnUpdate(ref SystemState state)
    //     {
    //         var allSourcesQuery = SystemAPI.QueryBuilder()
    //             .WithAll<SourceSimple>()
    //             .Build();

    //         var allSources = allSourcesQuery.ToEntityArray(Allocator.Temp);
    //         Debug.Log("allSources: " + allSources.Length);

    //         for (int sourceIndex = 0; sourceIndex < allSources.Length; sourceIndex++)
    //         {
    //             var children = state.EntityManager.GetBuffer<Child>(allSources[sourceIndex]);

    //             for (int childIndex = 0; childIndex < children.Length; childIndex++)
    //             {
    //                 Debug.Log("children[childIndex].Value: " + children[childIndex].Value);
    //                 if (state.EntityManager.HasComponent<SourceGeneratorTag>(children[childIndex].Value))
    //                 {
    //                     Debug.Log("got in here");
    //                     var currentSource = state.EntityManager.GetComponentData<SourceSimple>(allSources[sourceIndex]);
    //                     currentSource.sourceSurface = state.EntityManager.GetComponentData<Plane>(children[childIndex].Value);
    //                     state.EntityManager.SetComponentData<SourceSimple>(allSources[sourceIndex], currentSource);
    //                 }
    //                 if (state.EntityManager.HasComponent<SourceTargetTag>(children[childIndex].Value))
    //                 {
    //                     Debug.Log("got in here2");

    //                     var currentSource = state.EntityManager.GetComponentData<SourceSimple>(allSources[sourceIndex]);
    //                     currentSource.sourceTarget = state.EntityManager.GetComponentData<Plane>(children[childIndex].Value);
    //                     state.EntityManager.SetComponentData<SourceSimple>(allSources[sourceIndex], currentSource);
    //                 }
    //             }

    //         }
    //     }

    //     public readonly void OnCreate(ref SystemState state) { }

    //     public readonly void OnDestroy(ref SystemState state) { }
    // }
}