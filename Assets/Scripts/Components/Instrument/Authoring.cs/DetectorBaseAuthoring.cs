using Unity.Entities;
using UnityEngine;

using Unity.Mathematics;
using Unity.Collections;

using Unity.Burst;
using Unity.Rendering;

// Authoring MonoBehaviours are regular GameObject components.
// They constitute the inputs for the baking systems which generates ECS data.
public class DetectorBaseAuthoring : MonoBehaviour
{
    public int2 pixelCount;
    public DetectorGrid.Scale scale;
    public FixedString32Bytes filename;
    public GameObject detectorType;

    public enum DetectorShape { plane, sphere };
    public DetectorShape detectorShape;

    class Baker : Baker<DetectorBaseAuthoring>
    {
        public override void Bake(DetectorBaseAuthoring authoring)
        {
            AddComponent<GeneratorBaseTag>();
            AddComponent<DetectorTag>();
            AddComponent<DetectorGrid>(
                new DetectorGrid(authoring.pixelCount, authoring.scale)
            );
            AddComponent<GeneratorHolder>(new GeneratorHolder { generator = GetEntity(authoring.detectorType) });

            switch (authoring.detectorShape)
            {
                case DetectorShape.plane:
                    AddComponent<Plane>(new Plane(authoring.gameObject)); break;
                default:
                    throw new System.Exception();
            }
        }
    }
}

[BurstCompile]
[WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)]
partial struct DetectorBaseBakerSystem : ISystem
{
    EntityQuery allDetectors;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ParticleGeneratorHolder>();
        allDetectors = new EntityQueryBuilder(Allocator.Persistent).WithAll<DetectorTag, GeneratorBaseTag, DetectorGrid, GeneratorHolder>().Build(ref state);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var generatorsEntity = allDetectors.ToEntityArray(Allocator.TempJob);

        for (int i = 0; i < generatorsEntity.Length; i++)
        {
            // Create local version of the material
            Material mat = new Material(Shader.Find("Shader Graphs/Detector"));

            var renderMesh = state.EntityManager.GetSharedComponentManaged<RenderMesh>(generatorsEntity[i]);
            renderMesh.material = mat;
            state.EntityManager.SetSharedComponentManaged<RenderMesh>(generatorsEntity[i], renderMesh);

            // The detector needs the ID of the material to connect to it later
            var detectorGrid = state.EntityManager.GetComponentData<DetectorGrid>(generatorsEntity[i]);
            detectorGrid.MaterialID = mat.GetInstanceID();
            state.EntityManager.SetComponentData<DetectorGrid>(generatorsEntity[i], detectorGrid);
        }
    }
}