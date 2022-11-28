using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts
{
    public class MultiImageTargetGraphics : MonoBehaviour
    {
        [SerializeField] private Graphic[] targetGraphics;

        public Graphic[] GetTargetGraphics => targetGraphics;
    }
}