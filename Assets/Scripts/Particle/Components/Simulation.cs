using Unity.Entities;
using Unity.Mathematics;

namespace Particle
{
    public struct Simulation : IComponentData
    {
        public double time2intersection;

        public enum State
        {
            free, // Moving without a field
            absorb, // by a normal physical behavior
            absorbNonPhysical, // by another behavior, used by edge cases
            leftSimulation // detected by time being double max
        }
        public State state;

        public enum Generation
        {
            source, // made from a source component
            mirror, // made from a mirror component
        }
        public Generation generation;

        public float weight;

        public Entity lastParticle;

        public Simulation Copy()
        {
            return new Simulation
            {
                time2intersection = time2intersection,
                state = state,
                generation = generation,
                weight = weight,
                lastParticle = lastParticle
            };
        }
    }
}