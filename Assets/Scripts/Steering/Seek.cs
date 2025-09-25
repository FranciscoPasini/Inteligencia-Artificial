using UnityEngine;

public class Seek : ISteering
{
    private Transform npcTransform;
    private Transform target;
    private float maxSpeed;

    public Transform SetTarget
    {
        set
        {
            target = value;
        }
    }

    public Seek(Transform target, Transform npcTransform, float maxSpeed)
    {
        this.target = target;
        this.npcTransform = npcTransform;
        this.maxSpeed = maxSpeed;
    }

    public Vector3 GetSteerDir(Vector3 currentVelocity)
    {
        var dir = target.position - npcTransform.position; // Direcci�n = Posici�nFinal - Posici�nInicial
        var desiredVelocity = dir.normalized * maxSpeed; // velocidad deseada = direcci�n normalizada * velocidad m�xima
        Vector3 steering = desiredVelocity - currentVelocity; // correcci�n de velocidad = velocidad deseada - actual
        return currentVelocity += steering * Time.deltaTime; // a la velocidad actual se le suma la correcci�n (aceleraci�n) * tiempo (Time.deltaTime)
    }
}