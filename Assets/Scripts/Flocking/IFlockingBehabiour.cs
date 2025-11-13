using UnityEngine;
using System.Collections.Generic;
public interface IFlockingBehabiour
{
    public Vector3 GetDir(List<Boid> boids);
}
