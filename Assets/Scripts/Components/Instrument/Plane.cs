
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using System;
// using UnityEngine.Random;
public struct Plane : IComponentData
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

    public double2 project2Surface(double3 point){
        // https://stackoverflow.com/questions/23472048/projecting-3d-points-to-2d-plane
        double t_1 = math.dot(normalX, point-position)/(scale.x/2.0); 
        double t_2 = math.dot(normalY, point-position)/(scale.y/2.0); 

        // change interval from [-1,1] to [0,1]
        double s_1 = t_1*0.5+0.5;
        double s_2 = t_2*0.5+0.5;

        return new double2(s_1,s_2);
    }

    public void randomPointOnSurface(){}

}
