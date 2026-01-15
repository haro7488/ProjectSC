using System;
using Sc.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sc.Common.UI.Widgets
{
    /// <summary>
    /// 재화 표시 HUD (Gold, Gem 표시 + 충전 버튼)
    /// </summary>
    public class CurrencyHUD : Widget
    {
        [Header("Gold")]
        [SerializeField] private TMP_Text _goldText;
        [SerializeField] private Button _goldAddButton;

        [Header("Gem")]
        [SerializeField] private TMP_Text _gemText;
        [SerializeField] private Button _gemAddButton;

        /// <summary>
        /// Gold 충전 버튼 클릭 이벤트
        /// </summary>
        public event Action OnGoldAddClicked;

        /// <summary>
        /// Gem 충전 버튼 클릭 이벤트
        /// </summary>
        public event Action OnGemAddClicked;

        protected override void OnInitialize()
        {
            if (_goldAddButton != null)
            {
                _goldAddButton.onClick.AddListener(() => OnGoldAddClicked?.Invoke());
            }

            if (_gemAddButton != null)
            {
                _gemAddButton.onClick.AddListener(() => OnGemAddClicked?.Invoke());
            }
        }

        protected override void OnShow()
        {
            // DataManager 이벤트 구독
            if (DataManager.Instance != null)
            {
                DataManager.Instance.OnUserDataChanged += RefreshDisplay;
            }

            RefreshDisplay();
        }

        protected override void OnHide()
        {
            // DataManager 이벤트 해제
            if (DataManager.Instance != null)
            {
                DataManager.Instance.OnUserDataChanged -= RefreshDisplay;
            }
        }

        protected override void OnRefresh()
        {
            RefreshDisplay();
        }

        private void RefreshDisplay()
        {
            if (DataManager.Instance?.IsInitialized != true)
                return;

            var currency = DataManager.Instance.Currency;

            if (_goldText != null)
            {
                _goldText.text = FormatCurrency(currency.Gold);
            }

            if (_gemText != null)
            {
                _gemText.text = FormatCurrency(currency.TotalGem);
            }
        }

        /// <summary>
        /// 재화 수량 포맷 (1000 → 1,000 / 1000000 → 1.0M)
        /// </summary>
        private string FormatCurrency(long amount)
        {
            if (amount >= 1000000)
            {
                return $"{amount / 1000000f:F1}M";
            }
            if (amount >= 1000)
            {
                return $"{amount:N0}";
            }
            return amount.ToString();
        }
    }
}
