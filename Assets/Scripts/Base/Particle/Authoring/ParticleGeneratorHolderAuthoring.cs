// using Unity.Entities;

// using UnityEngine;


// // Authoring MonoBehaviours are regular GameObject components.
// // They constitute the inputs for the baking systems which generates ECS data.
// public class ParticleGeneratorHolderAuthoring : UnityEngine.MonoBehaviour
// {
//     public GameObject particlePrefab;
// }

// public struct ParticleGeneratorHolder : IComponentData
// {
//     public Entity particlePrefab;
// }

// // Bakers convert authoring MonoBehaviours into entities and components.
// public class PParticleGeneratorHolderBaker : Baker<ParticleGeneratorHolderAuthoring>
// {
//     public override void Bake(ParticleGeneratorHolderAuthoring authoring)
//     {
//         AddComponent<ParticleGeneratorHolder>(
//             new ParticleGeneratorHolder{
//                 particlePrefab = GetEntity(authoring.particlePrefab)
//             }
//         );
//     }
// }