using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPoint : MonoBehaviour
{
    [SerializeField] private Transform inPoint;
    [SerializeField] private Transform outPoint;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, .1f);
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public Vector3 GetInPosition()
    {
        return inPoint.position;
    }

    public Vector3 GetOutPosition()
    {
        return outPoint.position;
    }
}
