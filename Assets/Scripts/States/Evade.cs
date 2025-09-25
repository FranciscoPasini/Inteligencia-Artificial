using UnityEngine;

public class Evade : ISteering
{
    private Transform npcTransform;
    private Transform target;
    private float maxSpeed;
    private float timePrediction = 0.5f;
    private Rigidbody rb;
    public Transform SetTarget
    {
        set
        {
            target = value;
        }
    }

    public Evade(Transform target, Transform npcTransform, float maxSpeed)
    {
        this.target = target;
        rb = target.GetComponent<Rigidbody>();
        this.npcTransform = npcTransform;
        this.maxSpeed = maxSpeed;
    }

    public Vector3 GetSteerDir(Vector3 currentVelocity)
    {
        var dist = (target.position - npcTransform.position).magnitude;
        var futureTargetPosition = target.position + rb.linearVelocity * timePrediction * dist;

        var dirFromPoint = npcTransform.position - futureTargetPosition; // Dirección = PosiciónInicial - PosiciónFinal 
        var dirFromTarget = npcTransform.position - target.position; // Dirección = PosiciónInicial - PosiciónFinal

        //if(Vector3.Dot(dirToPoint, dirToTarget) < 0)
        //{
        //    dirToPoint = dirToTarget;
        //}
        Debug.DrawRay(npcTransform.position, dirFromPoint, Color.darkRed);

        var dotRemaped = (Vector3.Dot(dirFromPoint, dirFromTarget) + 1) / 2;
        dirFromPoint = Vector3.Lerp(dirFromPoint, dirFromTarget, dotRemaped);

        var desiredVelocity = dirFromPoint.normalized * maxSpeed; // velocidad deseada = dirección normalizada * velocidad máxima
        Vector3 steering = desiredVelocity - currentVelocity; // corrección de velocidad = velocidad deseada - actual


        Debug.DrawRay(npcTransform.position, dirFromPoint, Color.orange);
        Debug.DrawRay(npcTransform.position, dirFromTarget, Color.yellow);
        return currentVelocity += steering * Time.deltaTime; // a la velocidad actual se le suma la corrección (aceleración) * tiempo (Time.deltaTime)


    }
}
