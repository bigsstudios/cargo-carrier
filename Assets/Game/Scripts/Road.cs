using UnityEngine;

namespace Game.Scripts
{
    public class Road : MonoBehaviour
    {
        [SerializeField] private MovePath path;

        public MovePath GetMovePath()
        {
            return path;
        }
    }
}