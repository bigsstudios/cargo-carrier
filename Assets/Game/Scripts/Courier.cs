using Clicker;
using DG.Tweening;
using UnityEngine;

namespace Game.Scripts
{
    public class Courier : ClickerObject
    {
        private Road _road;
        private float _pathPercentage;
        private Tween _pathTween;
        private Tween _dummyTween;

        [SerializeField] private Renderer colorPart1;
        [SerializeField] private Color color1;
        [SerializeField] private Color color2;

        private bool _colorChanged;

        private void Awake()
        {
            _road = RoadManager.Instance.GetRoad(GetLevel());
        }

        public void ChangeColor()
        {
            colorPart1.material.color = _colorChanged ? color1 : color2;
            _colorChanged = !_colorChanged;
        }

        public override void SpawnPercentageReceived(float spawnPercentage)
        {
            _pathPercentage = spawnPercentage;
            MovePath(true, spawnPercentage);
        }

        private void OnTriggerEnter(Collider other)
        {
            var house = other.GetComponent<House>();
            house.Earned();
            ClickerManager.Instance.Earned(GetEarnPrice(), house.GetColor(), house.GetEarnUiPosition());
        }

        private void MovePath(bool isNew, float startPercentage)
        {
            var movePath = _road.GetMovePath();
            var path = movePath.GetPath(out var startPosition);
            var courierTransform = transform;
            // courierTransform.position = startPosition;

            var dummy = new GameObject
            {
                transform =
                {
                    position = startPosition
                }
            };

            _dummyTween = dummy.transform.DOPath(path, 6f, PathType.CubicBezier);
            _dummyTween.Pause();
            _dummyTween.ForceInit();

            courierTransform.position = _dummyTween.PathGetPoint(isNew ? startPercentage : 0f);

            _pathTween = DOVirtual.Float(isNew ? startPercentage : 0f, 1f, .1f, v =>
                {
                    _pathPercentage = v;
                    var currentPos = _dummyTween.PathGetPoint(v);
                    var nextPos = _dummyTween.PathGetPoint(v + 0.01f);
                    courierTransform.position = currentPos;
                    courierTransform.LookAt(nextPos);
                }).SetEase(Ease.Linear)
                .SetSpeedBased().OnComplete(() =>
                {
                    _dummyTween.Kill();
                    Destroy(dummy);
                    MovePath(false, 0f);
                });

            // var tween = transform.DOPath(path, 6f, PathType.CubicBezier)
            //     .SetLookAt(0.01f).SetSpeedBased().SetEase(Ease.Linear).OnComplete(MovePath);
        }

        public float GetPathPercentage()
        {
            return _pathPercentage;
        }

        public override string ToString()
        {
            return _pathPercentage.ToString("F2");
        }

        private void OnDestroy()
        {
            _pathTween?.Kill();
            _dummyTween?.Kill();
        }
    }
}