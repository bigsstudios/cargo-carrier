using System.Collections.Generic;
using System.Linq;
using Clicker;
using UnityEngine;

namespace Game.Scripts
{
    public class HouseManager : MonoBehaviour
    {
        public static HouseManager Instance { get; private set; }

        [SerializeField] private List<House> houses;

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

        public void InitializeHouses(int incomeLevel)
        {
            for (var i = 0; i < incomeLevel + 1; i++)
            {
                houses[i].gameObject.SetActive(true);
            }
        }

        public void HouseUnlocked(int houseLevel)
        {
            houses[houseLevel].gameObject.SetActive(true);
        }

        public bool CanUnlockHouse(int incomeLevel, int stopLevel)
        {
            return houses.Count > incomeLevel + 1 && houses[incomeLevel + 1].GetRoadLevel() <= stopLevel;
        }

        public bool IsAllHousesUnlocked(int stopLevel)
        {
            return houses.Where(h => h.GetRoadLevel() == stopLevel).All(h => h.gameObject.activeSelf);
        }

        public void RandomColorClicked()
        {
            houses.Where(h => h.GetRoadLevel() == 0).ForEach(h => h.ChangeColor());
        }
    }
}