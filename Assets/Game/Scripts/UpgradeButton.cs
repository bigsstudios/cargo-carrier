using Clicker;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts
{
    public class UpgradeButton : MultiImageButton
    {
        [SerializeField] private Image topImage;
        [SerializeField] private Image bottomImage;
        [SerializeField] private Image shadowImage;
        [SerializeField] private TMP_Text priceText;

        private Color _topColor;
        private Color _bottomColor;
        private Color _shadowColor;

        private bool _isHided;
        private bool _isPassive;

        private readonly Color _topPassiveColor = new(0.6603774f, 0.6603774f, 0.6603774f);
        private readonly Color _bottomPassiveColor = new(0.7924528f, 0.7924528f, 0.7924528f);
        private readonly Color _shadowPassiveColor = new(0.4811321f, 0.4811321f, 0.4811321f);

        protected override void Awake()
        {
            _topColor = topImage.color;
            _bottomColor = bottomImage.color;
            _shadowColor = shadowImage.color;
        }

        private void SetPassive()
        {
            _isPassive = true;
            interactable = false;
            topImage.color = _topPassiveColor;
            bottomImage.color = _bottomPassiveColor;
            shadowImage.color = _shadowPassiveColor;
        }

        private void SetActive()
        {
            _isPassive = false;
            interactable = true;
            topImage.color = _topColor;
            bottomImage.color = _bottomColor;
            shadowImage.color = _shadowColor;
        }

        private void Hide()
        {
            _isHided = true;
            transform.DOScale(Vector3.zero, .2f);
        }

        private void Show()
        {
            _isHided = false;
            transform.DOScale(Vector3.one, .2f);
        }

        public void Refresh(ClickerManager.UpgradeData data)
        {
            switch (_isHided)
            {
                case false when !data.CanUpgrade:
                    Hide();
                    break;
                case true when data.CanUpgrade:
                    Show();
                    break;
            }

            switch (_isPassive)
            {
                case false when data.Price > data.Balance:
                    SetPassive();
                    break;
                case true when data.Price <= data.Balance:
                    SetActive();
                    break;
            }

            if (data.Price > data.Balance)
            {
                SetPassive();
            }

            priceText.SetText(data.Price.ToString());
        }
    }
}