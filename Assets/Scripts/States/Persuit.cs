using UnityEngine;

public class Persuit : ISteering
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

    public Persuit(Transform target, Transform npcTransform, float maxSpeed)
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

        var dirToPoint = futureTargetPosition - npcTransform.position; // Dirección = PosiciónFinal - PosiciónInicial
        var dirToTarget = target.position - npcTransform.position; // Dirección = PosiciónFinal - PosiciónInicial

        //if(Vector3.Dot(dirToPoint, dirToTarget) < 0)
        //{
        //    dirToPoint = dirToTarget;
        //}
        Debug.DrawRay(npcTransform.position, dirToPoint, Color.darkRed);

        var dotRemaped = (Vector3.Dot(dirToPoint, dirToTarget) + 1) / 2;
        dirToPoint = Vector3.Lerp(dirToTarget, dirToPoint, dotRemaped);

        var desiredVelocity = dirToPoint.normalized * maxSpeed; // velocidad deseada = dirección normalizada * velocidad máxima
        Vector3 steering = desiredVelocity - currentVelocity; // corrección de velocidad = velocidad deseada - actual


        Debug.DrawRay(npcTransform.position, dirToPoint, Color.orange);
        Debug.DrawRay(npcTransform.position, dirToTarget, Color.yellow);
        return currentVelocity += steering * Time.deltaTime; // a la velocidad actual se le suma la corrección (aceleración) * tiempo (Time.deltaTime)


    }
}
