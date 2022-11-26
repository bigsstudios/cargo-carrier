using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : MonoBehaviour
{
    [SerializeField] private MovePath[] _paths;
    private void Awake()
    {
        _paths = transform.GetComponentsInChildren<MovePath>();
    }

    public MovePath GetMovePath()
    {
        return _paths[Random.Range(0, _paths.Length)];
    }
}
