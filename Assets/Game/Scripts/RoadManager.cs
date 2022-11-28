using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    public class RoadManager : MonoBehaviour
    {
        public static RoadManager Instance { get; private set; }

        [SerializeField] private List<Road> roads;

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

        public Road GetRoad(int level)
        {
            return roads[level];
        }

        public void InitializeRoads(int roadLevel)
        {
            for (var i = 0; i < roadLevel + 1; i++)
            {
                roads[i].gameObject.SetActive(true);
            }
        }

        public void RoadUnlocked(int roadLevel)
        {
            roads[roadLevel].gameObject.SetActive(true);
        }
    }
}