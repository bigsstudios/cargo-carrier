using DG.Tweening;
using UnityEngine;

namespace Game.Scripts
{
    public class House : MonoBehaviour
    {
        [SerializeField] private int level;
        [SerializeField] private int roadLevel;
        [SerializeField] private Transform earnUiPoint;
        [SerializeField] private Color color;
        
        [SerializeField] private MeshRenderer colorPart1;
        [SerializeField] private Color color1;
        [SerializeField] private Color color2;

        private bool _colorChanged;

        public int GetLevel()
        {
            return level;
        }

        public int GetRoadLevel()
        {
            return roadLevel;
        }
        
        public void ChangeColor()
        {
            colorPart1.material.color = _colorChanged ? color1 : color2;
            _colorChanged = !_colorChanged;
        }

        public void Earned()
        {
            transform.DORewind();
            var sequence = DOTween.Sequence();
            sequence.Insert(0f, transform.DOScale(Vector3.one * 1.1f, 0.2f));
            sequence.Insert(0.2f, transform.DOScale(Vector3.one, 0.2f));
        }

        public Vector3 GetEarnUiPosition()
        {
            return earnUiPoint.position;
        }

        public Color GetColor()
        {
            return color;
        }
    }
}