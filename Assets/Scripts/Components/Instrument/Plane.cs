
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

}
