using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace Clicker
{
    public class CameraController : MonoBehaviour
    {
        public static CameraController Instance { get; private set; }

        [SerializeField] private Camera mainCam;
        [SerializeField] private Camera uiCam;
        [SerializeField] private List<CameraStateData> stateData;
        [SerializeField] private Transform handlePivot;

        public float _targetRotY;
        public float _startRotY;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        private void Start()
        {
            _targetRotY = handlePivot.localEulerAngles.y;
        }

        public Camera GetMainCam()
        {
            return mainCam;
        }

        public Camera GetUiCam()
        {
            return uiCam;
        }

        public void ScreenRotateStarted()
        {
            _startRotY = handlePivot.localEulerAngles.y;
        }

        public void ScreenRotated(float diff)
        {
            _targetRotY = _startRotY + diff;
        }

        private void FixedUpdate()
        {
            var oldAngles = handlePivot.localEulerAngles;
            oldAngles.y = Mathf.LerpAngle(oldAngles.y, _targetRotY, Time.deltaTime * 5f) % 360f;
            handlePivot.localEulerAngles = oldAngles;
        }

        public void RefreshZoom(int level, bool animate)
        {
            var data = stateData[level];
            var aspect = mainCam.aspect;
            print("aspect: " + aspect);

            foreach (var aspectPosition in data.aspects)
            {
                if (aspectPosition.aspect < aspect) continue;
                var targetPos = aspectPosition.pos;
                if (animate)
                {
                    transform.DOMove(targetPos, 1f);
                }
                else
                {
                    transform.position = targetPos;
                }

                return;
            }

            var last = data.aspects.Last();
            var lastTargetPos = last.pos;
            if (animate)
            {
                transform.DOMove(lastTargetPos, 1f);
            }
            else
            {
                transform.position = lastTargetPos;
            }
        }

        [Serializable]
        private class CameraStateData
        {
            public List<CameraPosAspect> aspects;
        }

        [Serializable]
        public class CameraPosAspect
        {
            public float aspect;
            public Vector3 pos;
        }
    }
}