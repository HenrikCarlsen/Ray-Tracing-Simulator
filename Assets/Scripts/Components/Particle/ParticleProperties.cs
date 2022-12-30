using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;

// What is the last collision 
public struct ParticleProperties : ISharedComponentData
{
    public FixedString64Bytes name; 
    public double mass; // [MeV/c^2]
    public double lifetime; // [s] Mean lifetime of the free particle

    public double charge; // [e]
    public double eletricDipoleMoment;
    public double electricPolarizability;

    public double magneticMoment;
    public double magneticPolarizability;

    public double spin; // [1]
    public double isoSpin; // [1]
    public double parity; // [1]
}