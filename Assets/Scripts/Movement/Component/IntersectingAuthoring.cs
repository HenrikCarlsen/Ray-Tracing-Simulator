using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Transforms;

namespace Particle
{
    public class IntersectingAuthoring : UnityEngine.MonoBehaviour
    {

    }

    public class IntersectingBaker : Baker<IntersectingAuthoring>
    {
        public override void Bake(IntersectingAuthoring authoring)
        {
            AddComponent<IntersectingGeometryTemp>();
            AddSharedComponent<Particle.IntersectingGeometry>(
                new Particle.IntersectingGeometry()
            );
        }
    }
}