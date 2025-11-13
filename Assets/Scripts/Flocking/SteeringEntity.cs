using System;
using UnityEngine;
using System.Collections.Generic;

public class SteeringEntity : MonoBehaviour
{
    [SerializeField] protected float maxSpeed;
    [SerializeField] protected float maxForce;
    protected Vector3 velocity;

    public Vector3 Seek(Vector3 position)
    {
        var dir = position - transform.position;
        return Steer(dir.normalized * maxSpeed);
    }
    public Vector3 Steer(Vector3 desired)
    {
        var steering = desired - velocity;
        return Vector3.ClampMagnitude(steering, maxForce * Time.deltaTime);
    }
    public void AddForce(Vector3 force)
    {
        velocity = Vector3.ClampMagnitude(velocity + force, maxForce);
    }
    public void Move()
    {
        if (velocity == Vector3.zero) return;
        transform.forward = velocity;
        transform.position += velocity * Time.deltaTime;
    }

}
