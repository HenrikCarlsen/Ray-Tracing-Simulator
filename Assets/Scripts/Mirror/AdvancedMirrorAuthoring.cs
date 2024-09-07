using Unity.Entities;
using UnityEngine;

using Unity.Mathematics;


// Authoring MonoBehaviours are regular GameObject components.
// They constitute the inputs for the baking systems which generates ECS data.
public class AdvancedMirrorAuthoring : UnityEngine.MonoBehaviour{}

// Bakers convert authoring MonoBehaviours into entities and components.
public class AdvancedMirrorBaker : Baker<AdvancedMirrorAuthoring>
{
    public override void Bake(AdvancedMirrorAuthoring authoring)
    {
        AddComponent<MirrorTag>();
        AddComponent<Plane>(new Plane(authoring.gameObject));

        // Make GeneratorType based on the mesh and settings on this mirror

        // How to handle the data transfer in moveSystem?
        

        // Do the same for the detector
        //

        // 

        // Use mesh to make generatorType
    }
}