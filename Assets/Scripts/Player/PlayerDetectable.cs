using UnityEngine;

public class PlayerDetectable : MonoBehaviour, IDetectable
{
    [SerializeField] private Transform[] detectablePoints;

    public Transform Transform => transform;

    // Si no seteás nada en el inspector, devuelve al menos el transform del jugador
    public Transform[] DetectablePositions
        => (detectablePoints != null && detectablePoints.Length > 0)
            ? detectablePoints
            : new Transform[] { transform };
}
