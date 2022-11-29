using Unity.Entities;
using Unity.Mathematics;

// Authoring MonoBehaviours are regular GameObject components.
// They constitute the inputs for the baking systems which generates ECS data.
public class HistoryPointAuthoring : UnityEngine.MonoBehaviour
{
}

// Bakers convert authoring MonoBehaviours into entities and components.
public class HistoryPointBaker : Baker<HistoryPointAuthoring>
{
    public override void Bake(HistoryPointAuthoring authoring)
    {
        AddComponent<Movement>();
        AddComponent<ParticleHistoryPast>();
        AddComponent<ParticleHistoryFuture>();
        AddComponent<ParticleHistoryGraphic>();
        //AddBuffer<ParticleHistoryFuture>();
    }
}