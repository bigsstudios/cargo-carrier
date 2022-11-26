using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Courier : MonoBehaviour
{
    [SerializeField] private Road road;
    private void Start()
    {
        MovePath();
    }

    public void MovePath()
    {
        var movePath = road.GetMovePath();
        var path = movePath.GetPath(out var startPosition);
        transform.position = startPosition;

        transform.DOPath(path, 6f, PathType.CubicBezier)
            .SetLookAt(0.01f).SetSpeedBased().SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }
}