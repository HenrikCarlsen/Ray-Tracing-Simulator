
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using System;

public struct Plane : IComponentData, IGeometry
{
    public double3 position;
    public double3 normal;
    public double3 normalX;
    public double3 normalY;

    public double2 scale;

    public Plane(GameObject go)
    {
        var transform = go.GetComponent<Transform>();

        position = new double3(transform.position);
        normal = new double3(transform.rotation * Vector3.forward);

        normalX = new double3(transform.rotation * Vector3.right);
        normalY = new double3(transform.rotation * Vector3.up);

        scale = new double2(transform.lossyScale.x, transform.lossyScale.y);
    }

    public readonly override string ToString()
    {
        return "position: " + string.Format("{0:F3}", position) + ", normal: " + string.Format("{0:F3}", normal) + ", scale: " + string.Format("{0:F3}", scale);
    }


    public double2 Project2Surface(double3 point)
    {
        // https://stackoverflow.com/questions/23472048/projecting-3d-points-to-2d-plane
        double t_1 = math.dot(normalX, point - position) / (scale.x / 2.0);
        double t_2 = math.dot(normalY, point - position) / (scale.y / 2.0);

        // change interval from [-1,1] to [0,1]
        double s_1 = t_1 * 0.5 + 0.5;
        double s_2 = t_2 * 0.5 + 0.5;

        return new double2(s_1, s_2);
    }

    public readonly double TimeOfIntersection(Particle.Kinetic kinetic)
    {
        // TODO, this is alittle too silly, rewrite to first order polynomial
        return TimeOfIntersection(kinetic, new Particle.UniformForce { AccelerationSum = new double3(0, 0, 0) });
    }
    public readonly double TimeOfIntersection(Particle.Kinetic kinetic, Particle.UniformForce force)
    {
        //     // plane normal: N, point in space: P0, points on plane X=(x,y,z)
        //     // Nx*x +Ny*y + Nz*z - (Nx*x0 + Ny*y0 + Nz*z0)
        //     // parabola X = A*t^2+V*t+X0
        //     // Insert X in plane equation
        //     // This gives a second order equation:

        double a = math.dot(normal, force.AccelerationSum);
        double b = math.dot(normal, kinetic.velocity);
        double c = math.dot(normal, kinetic.position) - math.dot(normal, position);
        double t = Polynomial.SecondOrderEquationFirstPositive(a, b, c);

        // Debug.Log("normal: " + normal);
        // Debug.Log("position: " + position);
        // Debug.Log("kinetic: " + kinetic);
        //Debug.Log("\tt: " + t + ", a:" + a + ", b:" + b + ", c:" + c);

        return t;
    }

    public readonly bool Intersecting(double3 point, double error)
    {
        double x = LinePointDistance(position, position + normalY, point);
        double y = LinePointDistance(position, position + normalX, point);

        if (x <= scale.x / 2.0 && y <= scale.y / 2.0) return true;
        else return false;
    }

    private readonly double LinePointDistance(double3 L1, double3 L2, double3 P)
    {
        // https://en.wikipedia.org/wiki/Distance_from_a_point_to_a_line
        // https://onlinemschool.com/math/library/vector/multiply1/#h1
        double3 M1 = L1;
        double3 V = L2 - L1;
        double3 M0 = P;

        double3 M0M1 = M1 - M0;

        double D = math.sqrt(
            (M0M1.y * V.z - M0M1.z * V.y) * (M0M1.y * V.z - M0M1.z * V.y) +
            (M0M1.x * V.z - M0M1.z * V.x) * (M0M1.x * V.z - M0M1.z * V.x) +
            (M0M1.x * V.y - M0M1.y * V.x) * (M0M1.x * V.y - M0M1.y * V.x)
        );

        double N = Unity.Mathematics.math.length(V);
        return D / N;
    }

    public readonly double3 RandomPointOnSurface() { return new double3(); }
    public readonly double3 NormalOnPoint(double3 point)
    {
        return normal;
    }

}
