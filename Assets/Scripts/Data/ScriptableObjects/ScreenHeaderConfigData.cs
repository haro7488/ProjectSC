using System.Collections.Generic;
using UnityEngine;

namespace Sc.Data
{
    /// <summary>
    /// Screen Header 설정 데이터
    /// </summary>
    [CreateAssetMenu(fileName = "ScreenHeaderConfigData", menuName = "SC/Data/ScreenHeaderConfig")]
    public class ScreenHeaderConfigData : ScriptableObject
    {
        [Header("식별")]
        [SerializeField] private string _id;

        [Header("타이틀")]
        [SerializeField] private string _title;

        [Header("버튼 표시 설정")]
        [SerializeField] private bool _showBackButton;
        [SerializeField] private bool _showProfileButton;
        [SerializeField] private bool _showMenuButton;
        [SerializeField] private bool _showMailButton;
        [SerializeField] private bool _showNoticeButton;

        [Header("재화 표시 설정")]
        [SerializeField] private bool _showCurrency;
        [SerializeField] private List<string> _currencyTypes = new();

        // Properties (읽기 전용)
        public string Id => _id;
        public string Title => _title;
        public bool ShowBackButton => _showBackButton;
        public bool ShowProfileButton => _showProfileButton;
        public bool ShowMenuButton => _showMenuButton;
        public bool ShowMailButton => _showMailButton;
        public bool ShowNoticeButton => _showNoticeButton;
        public bool ShowCurrency => _showCurrency;
        public IReadOnlyList<string> CurrencyTypes => _currencyTypes;

#if UNITY_EDITOR
        /// <summary>
        /// Editor 전용: JSON 데이터로 초기화
        /// </summary>
        public void Initialize(
            string id,
            string title,
            bool showBackButton,
            bool showProfileButton,
            bool showMenuButton,
            bool showMailButton,
            bool showNoticeButton,
            bool showCurrency,
            List<string> currencyTypes)
        {
            _id = id;
            _title = title;
            _showBackButton = showBackButton;
            _showProfileButton = showProfileButton;
            _showMenuButton = showMenuButton;
            _showMailButton = showMailButton;
            _showNoticeButton = showNoticeButton;
            _showCurrency = showCurrency;
            _currencyTypes = currencyTypes ?? new List<string>();
        }
#endif
    }
}
