using Unity.Entities;
using UnityEngine;

using Unity.Mathematics;

namespace Mirror
{

    // Authoring MonoBehaviours are regular GameObject components.
    // They constitute the inputs for the baking systems which generates ECS data.
    public class MirrorAuthoring : UnityEngine.MonoBehaviour
    {
        public GameObject mirrorType;
    }

    // Bakers convert authoring MonoBehaviours into entities and components.
    public class MirrorBaker : Baker<MirrorAuthoring>
    {
        public override void Bake(MirrorAuthoring authoring)
        {
            AddComponent<MirrorTag>();
            AddComponent<SimpleMirror>();
            //AddComponent<GeneratorHolder>(new GeneratorHolder{generator = GetEntity(authoring.mirrorType)});
            AddComponent<Plane>(new Plane(authoring.gameObject));
        }
    }
}