using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Clicker
{
    public class MoneyUI : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private TMP_Text priceText;

        public void Animate(Color color, int price, int roadLevel)
        {
            spriteRenderer.material.color = color;
            priceText.SetText("$" + price);
            StartCoroutine(AnimateAsync(roadLevel));
        }

        private IEnumerator AnimateAsync(int roadLevel)
        {
            var moneyTransform = transform;
            moneyTransform.DOMove(moneyTransform.position + moneyTransform.up * 0.2f, 1f);
            yield return moneyTransform.DOScale(Vector3.one * (roadLevel + 1), .5f).WaitForCompletion();
            priceText.DOFade(0f, .5f);
            yield return spriteRenderer.DOFade(0f, .5f).WaitForCompletion();
            Destroy(gameObject);
        }

        private void Update()
        {
            transform.rotation = CameraController.Instance.GetMainCam().transform.rotation;
        }
    }
}