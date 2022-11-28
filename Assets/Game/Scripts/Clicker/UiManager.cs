using DG.Tweening;
using Game.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Clicker
{
    public class UiManager : MonoBehaviour
    {
        public static UiManager Instance { get; private set; }

        [SerializeField] private UpgradeButton addObjectButton;
        [SerializeField] private UpgradeButton mergeObjectButton;
        [SerializeField] private UpgradeButton addStopButton;
        [SerializeField] private UpgradeButton incomeButton;
        [SerializeField] private TMP_Text balanceText;
        [SerializeField] private TMP_Text speedText;
        [SerializeField] private Image mergeHand;
        [SerializeField] private Image addHand;
        [SerializeField] private Image fasterHand;
        [SerializeField] private TMP_Text fasterText;

        private bool _mergeHandShown;
        private bool _fasterHandShown;
        private bool _addHandShown;
        private Tween _mergeHandTween;
        private Tween _addHandTween;
        private Tween _fasterHandTween;
        private bool _firstMerge = true;
        private bool _firstAdd = true;

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
        
        public void ClickedScreenToSpeedUp()
        {
            if (_fasterHandShown)
            {
                HideFasterHand();
            }
        }
        
        private void ShowFasterHand()
        {
            _fasterHandShown = true;
            fasterText.gameObject.SetActive(true);
            fasterHand.transform.localScale = Vector3.one;
            var sequence = DOTween.Sequence();
            sequence.Append(fasterHand.transform.DOScale(Vector3.one * 1.2f, .5f).SetEase(Ease.InOutQuad));
            sequence.Append(fasterHand.transform.DOScale(Vector3.one, .5f).SetEase(Ease.InOutQuad));
            sequence.SetLoops(-1, LoopType.Restart);
            sequence.SetUpdate(true);
            _fasterHandTween = sequence;
        }

        private void HideFasterHand()
        {
            _fasterHandShown = false;
            _fasterHandTween?.Kill();
            fasterHand.transform.localScale = Vector3.zero;
            fasterText.gameObject.SetActive(false);
        }

        private void ShowMergeHand()
        {
            _firstMerge = false;
            _mergeHandShown = true;
            mergeHand.transform.localScale = Vector3.one;
            var sequence = DOTween.Sequence();
            sequence.Append(mergeHand.transform.DOScale(Vector3.one * 1.2f, .5f).SetEase(Ease.InOutQuad));
            sequence.Append(mergeHand.transform.DOScale(Vector3.one, .5f).SetEase(Ease.InOutQuad));
            sequence.SetLoops(-1, LoopType.Restart);
            sequence.SetUpdate(true);
            _mergeHandTween = sequence;
        }

        private void HideMergeHand()
        {
            _mergeHandShown = false;
            _mergeHandTween?.Kill();
            mergeHand.transform.localScale = Vector3.zero;
        }
        
        private void ShowAddHand()
        {
            _firstAdd = false;
            _addHandShown = true;
            addHand.transform.localScale = Vector3.one;
            var sequence = DOTween.Sequence();
            sequence.Append(addHand.transform.DOScale(Vector3.one * 1.2f, .5f).SetEase(Ease.InOutQuad));
            sequence.Append(addHand.transform.DOScale(Vector3.one, .5f).SetEase(Ease.InOutQuad));
            sequence.SetLoops(-1, LoopType.Restart);
            sequence.SetUpdate(true);
            _addHandTween = sequence;
        }

        private void HideAddHand()
        {
            _addHandShown = false;
            _addHandTween?.Kill();
            addHand.transform.localScale = Vector3.zero;
        }

        public void SetRevenueSpeed(float revenuePerSeconds)
        {
            speedText.SetText("$" + revenuePerSeconds.ToString("F2") + "/s");
        }

        public void Refresh(ClickerManager.UpgradeData addData,
            ClickerManager.UpgradeData mergeData,
            ClickerManager.UpgradeData addStopData,
            ClickerManager.UpgradeData incomeData,
            int balance)
        {
            addObjectButton.Refresh(addData);
            mergeObjectButton.Refresh(mergeData);
            addStopButton.Refresh(addStopData);
            incomeButton.Refresh(incomeData);
            balanceText.SetText(balance.ToString());
            
            // if (!_mergeHandShown && _firstMerge && mergeData.CanUpgrade && mergeData.Price <= mergeData.Balance)
            // {
            //     ShowMergeHand();
            // }
            //
            // if (!_addHandShown && _firstAdd && addData.CanUpgrade && addData.Price <= addData.Balance)
            // {
            //     ShowAddHand();
            // }
        }

        public void AddObjectClicked()
        {
            // if (_addHandShown)
            // {
            //     HideAddHand();
            //     ShowFasterHand();
            // }
            
            ClickerManager.Instance.AddObjectClicked();
        }

        public void MergeObjectClicked()
        {
            // if (_mergeHandShown)
            // {
            //     HideMergeHand();
            // }
            
            ClickerManager.Instance.MergeObjectClicked();
        }

        public void AddStopClicked()
        {
            ClickerManager.Instance.AddStopClicked();
        }

        public void IncomeClicked()
        {
            ClickerManager.Instance.IncomeClicked();
        }
    }
}