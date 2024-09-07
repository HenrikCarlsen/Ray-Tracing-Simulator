using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Transforms;
using System.Diagnostics;
using Unity.Entities.Serialization;
using Unity.Scenes;

namespace Particle
{
    public class SimpleParticleAuthoring : UnityEngine.MonoBehaviour
    {
        public float mass;
    }

    public class SimpleParticleBaker : Baker<SimpleParticleAuthoring>
    {
        public override void Bake(SimpleParticleAuthoring authoring)
        {
            AddComponent<ParticleTag>();
            AddComponent<Kinetic>();
            AddComponent<UniformForce>();
            AddComponent<Simulation>(new Simulation
            {
                time2intersection = double.MaxValue,
                state = Simulation.State.free
            });
            AddSharedComponent(
                new Properties
                {
                    mass = authoring.mass
                }
            );

            AddComponent<InBounds>();

            AddComponent(
                new particlePrefab()
            );

        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)]
    public partial struct NoTranslationSystem : ISystem
    {
        public readonly void OnCreate(ref SystemState state) { }
        public readonly void OnDestroy(ref SystemState state) { }

        public readonly void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (_, entity) in SystemAPI.Query<ParticleTag>().WithEntityAccess())
            {
                ecb.RemoveComponent<LocalToWorld>(entity);
            }
            ecb.Playback(state.EntityManager);
        }
    }
}