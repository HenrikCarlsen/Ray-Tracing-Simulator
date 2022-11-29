using Unity.Entities;
using Unity.Mathematics;

// Authoring MonoBehaviours are regular GameObject components.
// They constitute the inputs for the baking systems which generates ECS data.
public class HistoryPathAuthoring : UnityEngine.MonoBehaviour
{
}

// Bakers convert authoring MonoBehaviours into entities and components.
public class HistoryPathBaker : Baker<HistoryPathAuthoring>
{
    public override void Bake(HistoryPathAuthoring authoring)
    {
        AddComponent<Movement>();
        AddComponent<ParticleHistoryPath>();
    }
}