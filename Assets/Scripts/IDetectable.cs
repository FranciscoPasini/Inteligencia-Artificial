using System.Collections.Specialized;
using UnityEngine;

public interface IDetectable
{
    public Transform Transform { get; }
    public Transform[] DetectablePositions { get; }
}
