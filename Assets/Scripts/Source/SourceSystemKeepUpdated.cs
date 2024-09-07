using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Jobs;
using System.Runtime.InteropServices;

using UnityEngine;
using System;
using UnityEngine.UIElements;

using System.Collections;
using System.Collections.Generic;

namespace Source
{
    [BurstCompile]
    [StructLayout(LayoutKind.Auto)]
    [UpdateBefore(typeof(SourceSystem))]
    [UpdateInGroup(typeof(CreateParticles))]
    partial struct SourceUpdateSystem : ISystem
    {
        EntityQuery ChangedSourceComponents;


        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            ChangedSourceComponents = SystemAPI.QueryBuilder()
                .WithAll<Parent, LocalToWorld>()
                .WithAny<SourceGeneratorTag, SourceTargetTag, Plane>()
                .Build();
            ChangedSourceComponents.AddChangedVersionFilter(typeof(LocalToWorld));

            state.RequireForUpdate(ChangedSourceComponents);
        }

        [BurstCompile]
        public readonly void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (ChangedSourceComponents.IsEmpty) return;


            // For each ChangedSourceComponents propergate information to their source
            var ChangedSourceComponentsEntities = ChangedSourceComponents.ToEntityArray(Allocator.Temp);
            for (int i = 0; i < ChangedSourceComponentsEntities.Length; i++)
            {
                // TODO update plane
                var entity = ChangedSourceComponentsEntities[i];
                var parent = state.EntityManager.GetComponentData<Parent>(entity).Value;
                Debug.Log(entity + ": " + state.EntityManager.GetComponentData<Plane>(entity).position);

                var source = state.EntityManager.GetComponentData<SourceSimple>(parent);
                if (state.EntityManager.HasComponent<SourceGeneratorTag>(entity))
                {
                    source.sourceSurface = state.EntityManager.GetComponentData<Plane>(entity);
                }
                if (state.EntityManager.HasComponent<SourceTargetTag>(entity))
                {
                    source.sourceTarget = state.EntityManager.GetComponentData<Plane>(entity);
                }
                state.EntityManager.SetComponentData<SourceSimple>(parent, source);

                // Debug.Log("source: " + source.sourceSurface);
                // Debug.Log("target: " + source.sourceTarget);


            }
        }
    }

}