using Unity.Entities;
using UnityEngine;

using Unity.Mathematics;
using System;
using UnityEngine.UIElements;
using System.ComponentModel;
namespace Source
{

    // Authoring MonoBehaviours are regular GameObject components.
    // They constitute the inputs for the baking systems which generates ECS data.
    public class SurfaceAuthoring : UnityEngine.MonoBehaviour
    {
        public bool Virtual;
    }

    // Bakers convert authoring MonoBehaviours into entities and components.
    public class SurfaceBaker : Baker<SurfaceAuthoring>
    {
        public override void Bake(SurfaceAuthoring authoring)
        {
            if (authoring.Virtual) AddComponent<VirtualTag>();

            var mesh = authoring.gameObject.GetComponents<MeshFilter>() ?? throw new Exception("Surface object must have a MeshFilter attached");

            if (mesh[0].sharedMesh.name.Contains("Quad") || mesh[0].sharedMesh.name.Contains("Plane"))
            {
                AddComponent<Plane>(
                    new Plane(authoring.gameObject)
                );
            }

        }
    }
}
