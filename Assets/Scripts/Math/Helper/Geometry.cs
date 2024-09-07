using Unity.Entities;
using Unity.Mathematics;

interface IGeometry
{
    // void intersect(Particle.Kinetic kinetic);
    // void intersect(Particle.Kinetic kinetic, Particle.Force force);

    double TimeOfIntersection(Particle.Kinetic kinetic);
    double TimeOfIntersection(Particle.Kinetic kinetic, Particle.UniformForce force);
    double2 Project2Surface(double3 point);
    //public void randomPointOnSurface() { }
    bool Intersecting(double3 point, double error);

    double3 NormalOnPoint(double3 point);


}
