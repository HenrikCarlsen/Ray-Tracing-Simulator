using Unity.Entities;
using UnityEngine;

using Unity.Mathematics;
using Unity.Collections;

namespace Detector
{

    // Authoring MonoBehaviours are regular GameObject components.
    // They constitute the inputs for the baking systems which generates ECS data.
    public class SimpleDetectorTypeAuthoring : UnityEngine.MonoBehaviour
    {
        public float2 size;
        public int2 pixel;
        public FixedString32Bytes filename;

    }

    // Bakers convert authoring MonoBehaviours into entities and components.
    public class SimpleDetectorTypeBaker : Baker<SimpleDetectorTypeAuthoring>
    {
        public override void Bake(SimpleDetectorTypeAuthoring authoring)
        {
            // Generator tags
            AddComponent<DetectorTag>();

            // Generator properties

            // Particle properties
            AddComponent<DetectorGrid>();
            var buffer = AddBuffer<DetectorPixel>();

            int totalPixelCount = authoring.pixel.x * authoring.pixel.y;

            for (int i = 0; i < totalPixelCount; i++)
            {
                buffer.Add(new DetectorPixel { });
            }
            AddComponent<Plane>(new Plane(authoring.gameObject));


            // History
        }
    }

}