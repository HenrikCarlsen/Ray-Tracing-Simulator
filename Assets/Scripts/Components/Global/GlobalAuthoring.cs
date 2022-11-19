using Unity.Entities;
using Unity.Mathematics;

// Authoring MonoBehaviours are regular GameObject components.
// They constitute the inputs for the baking systems which generates ECS data.
public class GlobalAuthoring : UnityEngine.MonoBehaviour
{
    public double3 gravity;
}

// Bakers convert authoring MonoBehaviours into entities and components.
public class GlobalBaker : Baker<GlobalAuthoring>
{
    public override void Bake(GlobalAuthoring authoring)
    {
        AddComponent<Gravity>( new Gravity{acceleration = authoring.gravity} );
        AddComponent<GlobalPhysicsTag>();
    }
}