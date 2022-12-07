using Unity.Entities;
using UnityEngine;

using Unity.Mathematics;
using Unity.Collections;


// Authoring MonoBehaviours are regular GameObject components.
// They constitute the inputs for the baking systems which generates ECS data.
public class DetectorAuthoring : UnityEngine.MonoBehaviour{

    public float2 size;
    public int2 pixel;
    public FixedString32Bytes filename;
}

// Bakers convert authoring MonoBehaviours into entities and components.
public class DetectorBaker : Baker<DetectorAuthoring>
{
    public override void Bake(DetectorAuthoring authoring)
    {
        AddComponent<DetectorTag>();
        AddComponent<DetectorSimple>(
            new DetectorSimple{
                size = authoring.size,
                pixel = authoring.pixel,
                filename = authoring.filename
            }
        );
        AddComponent<Plane>();
    }
}