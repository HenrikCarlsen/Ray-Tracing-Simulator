using Unity.Mathematics;

using UnityEngine;

namespace Movement
{

    class GeometryHelper
    {
        public static double timeOfCollision(in Particle.Kinetic kinetic, in double3 acceleration, in Plane plane)
        {
            //     // plane normal: N, point in space: P0, points on plane X=(x,y,z)
            //     // Nx*x +Ny*y + Nz*z - (Nx*x0 + Ny*y0 + Nz*z0)
            //     // parabola X = A*t^2+V*t+X0
            //     // Insert X in plane equation
            //     // This gives a second order equation:

            double3 normal = plane.normal;

            double a = math.dot(normal, acceleration);
            double b = math.dot(normal, kinetic.velocity);
            double c = math.dot(normal, kinetic.position) - math.dot(normal, plane.position);
            double t = Polynomial.SecondOrderEquationFirstPositive(a, b, c);
            //Debug.Log("t: " + t + ", a:" + a + ", b:" + b + ", c:" + c );

            return t;
        }

        public static bool planeCollision(double3 movedPosition, Plane plane)
        {
            double x = linePointDistance(plane.position, plane.position + plane.normalY, movedPosition);
            double y = linePointDistance(plane.position, plane.position + plane.normalX, movedPosition);

            if (x <= plane.scale.x / 2.0 && y <= plane.scale.y / 2.0) return true;
            else return false;
        }

        public static double linePointDistance(double3 L1, double3 L2, double3 P)
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

    }
}