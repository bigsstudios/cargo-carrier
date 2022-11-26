using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePath : MonoBehaviour
{
    private WayPoint[] _waypoints;
    
            private void Awake()
            {
                _waypoints = transform.GetComponentsInChildren<WayPoint>();
            }
    
    
            public Vector3[] GetPath(out Vector3 startPosition)
            {
                var path = new List<Vector3>();
                startPosition = _waypoints[0].GetPosition();
    
                for (var i = 0; i < _waypoints.Length - 1; i++)
                {
                    var current = _waypoints[i];
                    var next = _waypoints[i + 1];
    
                    path.Add(next.GetPosition());
                    path.Add(current.GetOutPosition());
                    path.Add(next.GetInPosition());
                }
    
                return path.ToArray();
            }
}
