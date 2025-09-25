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
        var dir = target.position - npcTransform.position; // Dirección = PosiciónFinal - PosiciónInicial
        var desiredVelocity = dir.normalized * maxSpeed; // velocidad deseada = dirección normalizada * velocidad máxima
        Vector3 steering = desiredVelocity - currentVelocity; // corrección de velocidad = velocidad deseada - actual
        return currentVelocity += steering * Time.deltaTime; // a la velocidad actual se le suma la corrección (aceleración) * tiempo (Time.deltaTime)
    }
}