using UnityEngine;

public class Arrive : ISteering
{
    private Transform npcTransform;
    private Transform target;
    private float maxSpeed;
    private float slowingRange;
    public Transform SetTarget
    {
        set
        {
            target = value;
        }
    }

    public Arrive(Transform target, Transform npcTransform, float maxSpeed, float slowingRange)
    {
        this.target = target;
        this.npcTransform = npcTransform;
        this.maxSpeed = maxSpeed;
        this.slowingRange = slowingRange;
    }

    public Vector3 GetSteerDir(Vector3 currentVelocity)
    {

        var dir = target.position - npcTransform.position; // Direcci�n = Posici�nFinal - Posici�nInicial
        var dist = dir.magnitude;
        var rampedSpeed = maxSpeed * (dist / slowingRange); //La velocidad se hace proporcional a la distancia sobre el rango de desaceleracion
        var clippedSpeed = Mathf.Min(rampedSpeed, maxSpeed); // limitamos la velocidad al maximo

        var desiredVelocity = clippedSpeed * dir / dist; // a la velocidad calculada la multipliucamos por la direcci�n sin magnitud
        Vector3 steering = desiredVelocity - currentVelocity; // correcci�n de velocidad = velocidad deseada - actual
        return currentVelocity += steering * Time.deltaTime; // a la velocidad actual se le suma la correcci�n (aceleraci�n) * tiempo (Time.deltaTime)
    }
}