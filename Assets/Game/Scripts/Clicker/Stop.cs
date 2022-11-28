using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Clicker
{
    public class Stop : MonoBehaviour
    {
        [SerializeField] private List<Transform> points;
        [SerializeField] private Transform lockIcon;
        [SerializeField] private Material originalMaterial;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private bool unlocked;

        public void Unlock(bool animate)
        {
            unlocked = true;
            
            if (animate)
            {
                lockIcon.DOScale(Vector3.zero, .3f);
            }
            else
            {
                lockIcon.gameObject.SetActive(false);
            }

            meshRenderer.material = originalMaterial;
        }

        public Transform GetRandomPoint()
        {
            return points[Random.Range(0, points.Count)];
        }

        public bool IsUnlocked()
        {
            return unlocked;
        }
    }
}