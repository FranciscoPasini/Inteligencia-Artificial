using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Video;
using static UnityEngine.UI.Image;

public class FOV : MonoBehaviour
{
    [SerializeField] private GameObject _target;
    [SerializeField] private float _angle;
    [SerializeField] private float _distance;
    [SerializeField] private LayerMask _obstacleMask;
    private Vector3 Origin => transform.position;
    private Vector3 Forward => transform.forward;
    private IDetectable detectable;
    void Start()
    {
        detectable = _target.GetComponent<IDetectable>();
    }

    public bool CheckDetection()
    {
        bool canSee = false;

        for (int i = 0; i < detectable.DetectablePositions.Length; i++)
        {
            var currentPoint = detectable.DetectablePositions[i];
            if (IsInRange(currentPoint.position) && IsInAngle(currentPoint.position) && IsInSight(currentPoint.position))
            {
                canSee = true;
                break;
            }
        }
        return canSee;
    }

    public bool IsInRange(Vector3 target)
    {
        var sqrDistance = (Origin - target).sqrMagnitude;

        return sqrDistance <= _distance * _distance;
    }
    public bool IsInAngle(Vector3 target)
    {
        var dir = target - Origin;
        return Vector3.Angle(Forward, dir) <= _angle / 2;
    }
    public bool IsInSight(Vector3 target)
    {
        return !Physics.Linecast(Origin, target, _obstacleMask);
    }

    private void OnDrawGizmos()
    {
        Color myColor = Color.blue;
        myColor.a = 0.5f;
        Gizmos.color = myColor;
        Gizmos.DrawWireSphere(Origin, _distance);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(Origin, Quaternion.Euler(0, _angle / 2, 0) * Forward * _distance);
        Gizmos.DrawRay(Origin, Quaternion.Euler(0, -_angle / 2, 0) * Forward * _distance);

        Gizmos.color = Color.green;
    }
}
