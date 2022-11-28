using UnityEngine;

namespace Clicker
{
    public class ClickerObject : MonoBehaviour
    {
        [SerializeField] private int earnPrice;
        [SerializeField] private int level;

        protected bool IsSpawned;

        protected virtual float GetEarnDuration()
        {
            return 0f;
        }

        public virtual void StopAnimation()
        {
            
        }

        protected virtual void Spawned(bool merged, Stop end = null, Vector3 endPos = default)
        {
            
        }

        public virtual void SpawnPercentageReceived(float spawnPercentage)
        {
            
        }

        protected int GetEarnPrice()
        {
            return earnPrice;
        }

        public float GetRevenuePerSecond()
        {
            return earnPrice / (GetEarnDuration() / ClickerManager.Instance.GetGameSpeed());
        }

        public void SetSpawned(bool merged, Stop end = null, Vector3 endPos = default)
        {
            IsSpawned = true;

            if (merged)
            {
                Spawned(true, end, endPos);
            }
            else
            {
                Spawned(false);
            }
        }

        protected int GetLevel()
        {
            return level;
        }
    }
}