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
    public class SourceComponentAuthoring : UnityEngine.MonoBehaviour
    {
        public enum SourceComponent { generator, target }

        public SourceComponent sourceComponent;
    }

    // Bakers convert authoring MonoBehaviours into entities and components.
    public class SourceComponentBaker : Baker<SourceComponentAuthoring>
    {
        public override void Bake(SourceComponentAuthoring authoring)
        {
            switch (authoring.sourceComponent)
            {
                case SourceComponentAuthoring.SourceComponent.generator:
                    AddComponent<SourceGeneratorTag>();
                    break;
                case SourceComponentAuthoring.SourceComponent.target:
                    AddComponent<SourceTargetTag>();
                    break;
                default:
                    break;
            }
        }
    }
}
