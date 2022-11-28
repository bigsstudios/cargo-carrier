using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Game.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Clicker
{
    public class ClickerManager : MonoBehaviour
    {
        public static ClickerManager Instance { get; private set; }

        [SerializeField] private List<ClickerObject> prefabs;
        [SerializeField] private MoneyUI moneySpritePrefab;
        [SerializeField] private List<int> addStopPriceList;
        [SerializeField] private ParticleSystem mergeEffectPrefab;

        private List<int> _objectCountList;
        private Dictionary<int, List<ClickerObject>> _clickerObjects;
        private int MaxObjectCount => 4 + GetStopLevel() * 4;

        private float _gameSpeed;
        private float _lastClickTime;

        [SerializeField] private Image cursor;
        [SerializeField] private Canvas cursorCanvas;
        private Tween _cursorTween;

        private int _stopLevel;

        private const int incomePrice = 100;
        private const int speedPrice = 250;

        private Vector3 _clickPos;

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

        public void RandomCourierColorClicked()
        {
            _clickerObjects[0].Cast<Courier>().ForEach(c => c.ChangeColor());
        }

        private void Start()
        {
            Application.targetFrameRate = 60;
            _gameSpeed = 1f;
            CheckFirstStart();

            _objectCountList = new List<int>();
            _clickerObjects = new Dictionary<int, List<ClickerObject>>();

            for (var i = 0; i <= prefabs.Count - 1; i++)
            {
                var key = "level_" + i + "_count";
                _clickerObjects[i] = new List<ClickerObject>();
                _objectCountList.Add(PlayerPrefs.GetInt(key));
            }

            Started();
        }

        private void Update()
        {
            RefreshRevenuePerSecond();

            if (Input.GetMouseButtonDown(0) && !IsPointerOverUIObject())
            {
                _clickPos = Input.mousePosition;
                CameraController.Instance.ScreenRotateStarted();
            }
            else if (Input.GetMouseButton(0) && !IsPointerOverUIObject())
            {
                var mousePos = Input.mousePosition;
                var horizontalDiff = (mousePos.x - _clickPos.x) / 4f;
                CameraController.Instance.ScreenRotated(horizontalDiff);
            }
            else if (Input.GetMouseButtonUp(0) && !IsPointerOverUIObject())
            {
                var mousePos = Input.mousePosition;
                if (_clickPos != mousePos) return;
                
                MoveCursor();
                // UiManager.Instance.ClickedScreenToSpeedUp();
                var s = _gameSpeed + Time.deltaTime * 20f;
                _gameSpeed = Mathf.Min(2.5f, s);
                _lastClickTime = Time.time;
            }
            else if (_gameSpeed > 1f && Time.time - _lastClickTime > 1f)
            {
                var s = _gameSpeed - Time.deltaTime * 5f;
                _gameSpeed = Mathf.Max(1f, s);
            }

            Time.timeScale = _gameSpeed;
        }

        private void MoveCursor()
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                cursorCanvas.transform as RectTransform,
                Input.mousePosition, cursorCanvas.worldCamera,
                out var movePos);

            cursor.transform.position = cursorCanvas.transform.TransformPoint(movePos);

            _cursorTween?.Kill();
            cursor.DOFade(1f, 0.01f);
            _cursorTween = cursor.DOFade(0f, 1f);
        }

        private static bool IsPointerOverUIObject()
        {
            var eventDataCurrentPosition = new PointerEventData(EventSystem.current)
            {
                position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
            };
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }

        public void Earned(int price, Color color, Vector3 endPos)
        {
            ShowMoneyUI(color, endPos, price);
            SetBalance(GetBalance() + price);
            RefreshUI();
        }

        private void ShowMoneyUI(Color color, Vector3 endPos, int price)
        {
            var rot = CameraController.Instance.GetMainCam().transform.rotation;
            var moneyUI = Instantiate(moneySpritePrefab, endPos, rot);
            moneyUI.Animate(color, price, GetStopLevel());
        }

        private void CheckFirstStart()
        {
            var firstStart = PlayerPrefs.GetInt("launchedBefore", 0) == 0;
            if (!firstStart) return;

            PlayerPrefs.SetInt("launchedBefore", 1);

            PlayerPrefs.SetInt("add_price", 5);
            PlayerPrefs.SetInt("add_level", 0);

            PlayerPrefs.SetInt("merge_price", 10);
            PlayerPrefs.SetInt("merge_level", 0);

            PlayerPrefs.SetInt("stop_price", addStopPriceList[0]);
            PlayerPrefs.SetInt("stop_level", 0);

            PlayerPrefs.SetInt("income_price", 100);
            PlayerPrefs.SetInt("income_level", 0);

            PlayerPrefs.SetInt("level_0_count", 1);
        }

        private void Started()
        {
            var stopLevel = GetStopLevel();
            var incomeLevel = GetIncomeLevel();
            _stopLevel = stopLevel;
            RoadManager.Instance.InitializeRoads(stopLevel);
            HouseManager.Instance.InitializeHouses(incomeLevel);
            CameraController.Instance.RefreshZoom(stopLevel, false);

            for (var level = 0; level < _objectCountList.Count; level++)
            {
                var count = _objectCountList[level];
                var list = _clickerObjects[level];
                var prefab = prefabs[level];

                for (var i = 0; i < count; i++)
                {
                    var obj = Instantiate(prefab);
                    var percentage = GetSpawnPercentage(list, out var spawnIndex);
                    obj.SpawnPercentageReceived(percentage);
                    // list.Add(obj);
                    list.Insert(spawnIndex, obj);
                    obj.SetSpawned(false);
                }
            }

            RefreshUI();
            RefreshRevenuePerSecond();
        }

        private static float GetSpawnPercentage(ICollection objects, out int spawnIndex)
        {
            var couriers = objects.Cast<Courier>().ToList();
            // print(string.Join("-", couriers));

            switch (couriers.Count)
            {
                case 0:
                    spawnIndex = 0;
                    return 0f;
                case 1:
                    spawnIndex = 1;
                    return GetRealPercentage(couriers[0].GetPathPercentage() + 0.5f);
            }

            var nextIndex = -1;
            var prevPercentage = -1f;
            var maxDiff = -1f;

            for (var i = 0; i < couriers.Count; i++)
            {
                var tempNextIndex = i + 1 == couriers.Count ? 0 : i + 1;
                var tempNextPercentage = couriers[tempNextIndex].GetPathPercentage();
                var tempPrevPercentage = couriers[i].GetPathPercentage();
                if (tempNextPercentage < tempPrevPercentage) tempNextPercentage += 1f;
                var diff = Math.Round(Mathf.Abs(tempNextPercentage - tempPrevPercentage), 2);

                // print("diff between " + i + " and " + tempNextIndex + " is " + diff);

                if (diff <= maxDiff) continue;

                // print("checked " + i + "(" + tempPrevPercentage + ") and " + tempNextIndex + "(" + tempNextPercentage + ") .found " + diff + " is bigger than " + maxDiff);
                maxDiff = (float)diff;
                prevPercentage = tempPrevPercentage;
                nextIndex = tempNextIndex == 0 ? couriers.Count : tempNextIndex;
            }

            var centerPercentage = prevPercentage + (maxDiff / 2f);
            var realCenterPercentage = GetRealPercentage(centerPercentage);
            spawnIndex = nextIndex;
            // print("prev: " + prevPercentage + ", next: " + nextPercentage + ", center: " + centerPercentage + ", real: " + realCenterPercentage + ", index: " + spawnIndex);

            return realCenterPercentage;
        }

        private static float GetRealPercentage(float percentage)
        {
            return percentage switch
            {
                < 0f => percentage + 1f,
                > 1f => percentage - 1f,
                _ => percentage
            };
        }

        public float GetGameSpeed()
        {
            return _gameSpeed;
        }

        private void RefreshRevenuePerSecond()
        {
            var revenue = _clickerObjects.Where(x => x.Value.Count > 0)
                .Sum(o => o.Value.Sum(x => x.GetRevenuePerSecond()));
            UiManager.Instance.SetRevenueSpeed(revenue);
        }

        private void RefreshUI()
        {
            var balance = GetBalance();

            var addData = new UpgradeData(CanAddObject(), GetAddPrice(), balance);
            var mergeData = new UpgradeData(CanMergeObject(), GetMergePrice(), balance);
            var addStopData = new UpgradeData(CanAddStop(), GetStopPrice(), balance);
            var incomeData = new UpgradeData(CanIncreaseIncome(), GetIncomePrice(), balance);

            UiManager.Instance.Refresh(addData, mergeData, addStopData, incomeData, balance);
        }

        public void AddObjectClicked()
        {
            var price = GetAddPrice();
            if (!CanProvidePrice(price)) return;
            if (!CanAddObject()) return;
            AddObject();
        }

        public void MergeObjectClicked()
        {
            var price = GetMergePrice();
            if (!CanProvidePrice(price)) return;
            if (!CanMergeObject()) return;
            MergeObject();
        }

        public void AddStopClicked()
        {
            var price = GetStopPrice();
            if (!CanProvidePrice(price)) return;
            if (!CanAddStop()) return;
            AddStop();
        }

        public void IncomeClicked()
        {
            var price = GetIncomePrice();
            if (!CanProvidePrice(price)) return;
            if (!CanIncreaseIncome()) return;
            IncreaseIncome();
        }

        private void IncreaseIncome()
        {
            var price = GetIncomePrice();
            SetBalance(GetBalance() - price);

            IncreaseIncomeData();
        }

        private bool CanProvidePrice(int price)
        {
            return GetBalance() >= price;
        }

        private void AddObject()
        {
            var price = GetAddPrice();
            SetBalance(GetBalance() - price);

            var obj = Instantiate(prefabs[0]);
            var percentage = GetSpawnPercentage(_clickerObjects[0], out var spawnIndex);
            obj.SpawnPercentageReceived(percentage);
            // _clickerObjects[0].Add(obj);
            _clickerObjects[0].Insert(spawnIndex, obj);
            obj.SetSpawned(false);

            IncreaseAddObjectData();
            UpdateCounts();
        }

        private void MergeWithEffect(List<ClickerObject> objsList, int level)
        {
            // var bounds = new Bounds(objsList[0].transform.position, Vector3.zero);
            // objsList.ForEach(o =>
            // {
            //     o.StopAnimation();
            //     bounds.Encapsulate(o.transform.position);
            // });
            // var sequence = DOTween.Sequence();
            // var mergePoint = bounds.center;
            // mergePoint.y = 1.8f;
            // var plane = objsList[0].transform.GetComponent<Plane>();
            // var end = plane.GetEnd();
            // var endPos = plane.GetEndPos();
            // var rot = objsList[0].transform.eulerAngles;
            // rot.x = 0;

            var obj = Instantiate(prefabs[level + 1]);
            var percentage = GetSpawnPercentage(_clickerObjects[level + 1], out var spawnIndex);
            obj.SpawnPercentageReceived(percentage);
            _clickerObjects[level + 1].Insert(spawnIndex, obj);
            IncreaseMergeObjectData();
            UpdateCounts();
            objsList.ForEach(o => Destroy(o.gameObject));

            // objsList.ForEach(o =>
            // {
            //     sequence.Insert(0, o.transform.DOMove(mergePoint, .5f));
            //     sequence.Insert(0, o.transform.DOScale(Vector3.one * 0.5f, .5f));
            //     sequence.Insert(0, o.transform.DORotate(rot, .5f));
            //     sequence.Insert(.5f, o.transform.DOScale(Vector3.zero, .2f));
            // });
            // sequence.InsertCallback(.5f, () =>
            // {
            //     Instantiate(mergeEffectPrefab, mergePoint, Quaternion.identity);
            // });
            // sequence.OnComplete(() =>
            // {
            //     objsList.ForEach(o => Destroy(o.gameObject));
            //     
            //     obj.transform.DOScale(Vector3.one * 0.5f, .2f).OnComplete(() =>
            //     {
            //         obj.SetSpawned(true, end, endPos);
            //     });
            // });
        }

        private void MergeObject()
        {
            var price = GetMergePrice();
            SetBalance(GetBalance() - price);

            foreach (var (level, list) in _clickerObjects)
            {
                if (list.Count < 3) continue;

                var willBeMerged = list.TakeLast(3).ToList();
                list.RemoveRange(list.Count - 3, 3);
                MergeWithEffect(willBeMerged, level);

                break;
            }
        }

        private void AddStop()
        {
            var price = GetStopPrice();
            SetBalance(GetBalance() - price);

            IncreaseAddStopData();

            var stopLevel = GetStopLevel();
            RoadManager.Instance.RoadUnlocked(stopLevel);
            CameraController.Instance.RefreshZoom(stopLevel, true);
            IncreaseIncomeData();
        }

        private void UpdateCounts()
        {
            foreach (var (level, list) in _clickerObjects)
            {
                var key = "level_" + level + "_count";
                PlayerPrefs.SetInt(key, list.Count);
            }
        }

        private bool CanAddObject()
        {
            var totalObjects = _clickerObjects.Sum(o => o.Value.Count);
            return totalObjects < MaxObjectCount;
        }

        private bool CanMergeObject()
        {
            foreach (var (level, list) in _clickerObjects)
            {
                if (list.Count >= 3 && GetStopLevel() > level) return true;
            }

            return false;
        }

        private bool CanIncreaseIncome()
        {
            return HouseManager.Instance.CanUnlockHouse(GetIncomeLevel(), GetStopLevel());
        }

        private bool CanAddStop()
        {
            var stopLevel = GetStopLevel();
            return HouseManager.Instance.IsAllHousesUnlocked(stopLevel) && addStopPriceList.Count > stopLevel + 1;
        }

        private void IncreaseAddObjectData()
        {
            var price = GetAddPrice();
            var level = GetAddLevel();

            level++;
            price += level;

            SetAddLevel(level);
            SetAddPrice(price);
            RefreshUI();
        }

        private void IncreaseMergeObjectData()
        {
            var price = GetMergePrice();
            var level = GetMergeLevel();

            level++;
            price += level * 10;

            SetMergeLevel(level);
            SetMergePrice(price);
            RefreshUI();
        }

        private void IncreaseIncomeData()
        {
            var price = GetIncomePrice();
            var level = GetIncomeLevel();

            level++;
            price += level * 50;

            SetIncomeLevel(level);
            SetIncomePrice(price);

            var incomeLevel = GetIncomeLevel();
            HouseManager.Instance.HouseUnlocked(incomeLevel);
            
            RefreshUI();
        }

        private void IncreaseAddStopData()
        {
            var level = GetStopLevel();

            level++;
            var price = addStopPriceList[level];

            _stopLevel = level;
            SetStopLevel(level);
            SetStopPrice(price);
            RefreshUI();
        }

        public int GetLocalStopLevel()
        {
            return _stopLevel;
        }

        public class UpgradeData
        {
            public readonly bool CanUpgrade;
            public readonly int Price;
            public readonly int Balance;

            public UpgradeData(bool canUpgrade, int price, int balance)
            {
                CanUpgrade = canUpgrade;
                Price = price;
                Balance = balance;
            }
        }

        #region Getters

        private int GetBalance()
        {
            return PlayerPrefs.GetInt("balance", 0);
        }

        private int GetAddPrice()
        {
            return PlayerPrefs.GetInt("add_price", 0);
        }

        private int GetMergePrice()
        {
            return PlayerPrefs.GetInt("merge_price", 0);
        }

        private static int GetIncomePrice()
        {
            return PlayerPrefs.GetInt("income_price", 0);
        }

        private int GetStopPrice()
        {
            return PlayerPrefs.GetInt("stop_price", 0);
        }

        private int GetAddLevel()
        {
            return PlayerPrefs.GetInt("add_level", 0);
        }

        private int GetMergeLevel()
        {
            return PlayerPrefs.GetInt("merge_level", 0);
        }

        private static int GetIncomeLevel()
        {
            return PlayerPrefs.GetInt("income_level", 0);
        }

        private int GetStopLevel()
        {
            return PlayerPrefs.GetInt("stop_level", 0);
        }

        #endregion

        #region Setters

        private static void SetBalance(int balance)
        {
            PlayerPrefs.SetInt("balance", balance);
        }

        private static void SetAddPrice(int price)
        {
            PlayerPrefs.SetInt("add_price", price);
        }

        private static void SetMergePrice(int price)
        {
            PlayerPrefs.SetInt("merge_price", price);
        }

        private static void SetIncomeLevel(int level)
        {
            PlayerPrefs.SetInt("income_level", level);
        }

        private static void SetIncomePrice(int price)
        {
            PlayerPrefs.SetInt("income_price", price);
        }

        private static void SetStopPrice(int price)
        {
            PlayerPrefs.SetInt("stop_price", price);
        }

        private static void SetAddLevel(int level)
        {
            PlayerPrefs.SetInt("add_level", level);
        }

        private static void SetMergeLevel(int level)
        {
            PlayerPrefs.SetInt("merge_level", level);
        }

        private static void SetStopLevel(int level)
        {
            PlayerPrefs.SetInt("stop_level", level);
        }

        #endregion
    }
}