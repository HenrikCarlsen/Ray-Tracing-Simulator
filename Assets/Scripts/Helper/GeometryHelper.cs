using Unity.Mathematics;

using UnityEngine;


class GeometryHelper
{
    public static double timeOfCollision(in Movement movement, in double3 acceleration, in Plane plane)
    {
        //     // plane normal: N, point in space: P0, points on plane X=(x,y,z)
        //     // Nx*x +Ny*y + Nz*z - (Nx*x0 + Ny*y0 + Nz*z0)
        //     // parabola X = A*t^2+V*t+X0
        //     // Insert X in plane equation
        //     // This gives a second order equation:
        double a = math.dot(plane.normal, acceleration);
        double b = math.dot(plane.normal, movement.velocity);
        double c = math.dot(plane.normal, movement.position) - math.dot(plane.normal, plane.position);
        double t = MathExtra.SecondOrderEquationFirstPositive(a, b, c);
        Debug.Log("t: " + t + ", a:" + a + ", b:" + b + ", c:" + c );

        return t;
    }
}