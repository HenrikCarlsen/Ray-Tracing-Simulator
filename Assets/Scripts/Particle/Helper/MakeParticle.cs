using Unity.Entities;
using Unity.Mathematics;

namespace Particle
{
    public struct Helper
    {

        // public static Entity MakeParticle(
        //     EntityCommandBuffer.ParallelWriter commandBuffer,
        //     int chunkIndex,
        //     in particlePrefab prefab,
        //     in Simulation simulation,
        //     in Kinetic kinetic,
        //     in Simulation.Generation generation,
        //     Entity lastParticleEntity
        // )
        // {

        //     var newParticleEntity = commandBuffer.Instantiate(chunkIndex, prefab.Value);

        //     // 



        //     return newParticleEntity;

        // }



        public static Entity MakeParticle(
            EntityCommandBuffer.ParallelWriter commandBuffer,
            int chunkIndex,
            in particlePrefab prefab,
            in Simulation simulation,
            in Kinetic kinetic,
            in Simulation.Generation generation,
            Entity lastParticleEntity
        )
        {
            var newParticleEntity = commandBuffer.Instantiate(chunkIndex, prefab.Value);
            commandBuffer.SetComponent<particlePrefab>(chunkIndex, newParticleEntity, prefab);


            var a = new Simulation();

            // a.Copy();


            commandBuffer.SetComponent<Simulation>(chunkIndex, newParticleEntity,
                new Simulation
                {
                    time2intersection = double.MaxValue,
                    generation = generation,
                    lastParticle = lastParticleEntity
                }
            );

            return newParticleEntity;
        }
    }
}