using System;
using Sc.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sc.Contents.Gacha
{
    /// <summary>
    /// 가챠 배너 아이템 위젯
    /// </summary>
    public class GachaBannerItem : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button _bannerButton;
        [SerializeField] private Image _bannerImage;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _remainingTimeText;
        [SerializeField] private GameObject _selectedIndicator;
        [SerializeField] private GameObject _rateUpBadge;
        [SerializeField] private Image _backgroundImage;

        private GachaPoolData _poolData;
        private Action<string> _onSelected;
        private bool _isSelected;

        /// <summary>
        /// 현재 풀 ID
        /// </summary>
        public string PoolId => _poolData?.Id;

        /// <summary>
        /// 선택 여부
        /// </summary>
        public bool IsSelected => _isSelected;

        private void Awake()
        {
            if (_bannerButton != null)
            {
                _bannerButton.onClick.AddListener(OnBannerClicked);
            }
        }

        private void OnDestroy()
        {
            if (_bannerButton != null)
            {
                _bannerButton.onClick.RemoveListener(OnBannerClicked);
            }
        }

        /// <summary>
        /// 배너 설정
        /// </summary>
        public void Setup(GachaPoolData poolData, Action<string> onSelected, bool isSelected = false)
        {
            _poolData = poolData;
            _onSelected = onSelected;
            _isSelected = isSelected;

            RefreshUI();
        }

        /// <summary>
        /// 선택 상태 설정
        /// </summary>
        public void SetSelected(bool selected)
        {
            _isSelected = selected;
            UpdateSelectedState();
        }

        /// <summary>
        /// 남은 시간 갱신 (주기적 호출용)
        /// </summary>
        public void RefreshRemainingTime(DateTime serverTime)
        {
            if (_remainingTimeText == null || _poolData == null) return;

            var remaining = _poolData.GetRemainingTime(serverTime);
            if (remaining.HasValue)
            {
                _remainingTimeText.gameObject.SetActive(true);
                _remainingTimeText.text = FormatRemainingTime(remaining.Value);
            }
            else
            {
                _remainingTimeText.gameObject.SetActive(false);
            }
        }

        private void RefreshUI()
        {
            if (_poolData == null) return;

            // 이름 설정
            if (_nameText != null)
            {
                _nameText.text = _poolData.Name;
            }

            // 픽업 뱃지
            if (_rateUpBadge != null)
            {
                _rateUpBadge.SetActive(_poolData.Type == GachaType.Pickup &&
                                        !string.IsNullOrEmpty(_poolData.RateUpCharacterId));
            }

            // 남은 시간 초기 설정
            RefreshRemainingTime(DateTime.UtcNow);

            // 선택 상태
            UpdateSelectedState();

            // 배경색 설정 (타입별)
            UpdateBackgroundColor();
        }

        private void UpdateSelectedState()
        {
            if (_selectedIndicator != null)
            {
                _selectedIndicator.SetActive(_isSelected);
            }

            if (_bannerButton != null)
            {
                var colors = _bannerButton.colors;
                colors.normalColor = _isSelected
                    ? new Color(1f, 1f, 1f, 1f)
                    : new Color(0.8f, 0.8f, 0.8f, 1f);
                _bannerButton.colors = colors;
            }
        }

        private void UpdateBackgroundColor()
        {
            if (_backgroundImage == null || _poolData == null) return;

            _backgroundImage.color = _poolData.Type switch
            {
                GachaType.Pickup => new Color(1f, 0.9f, 0.7f, 1f),    // 골드 계열
                GachaType.Free => new Color(0.7f, 0.9f, 1f, 1f),      // 하늘색 계열
                _ => new Color(0.9f, 0.9f, 0.9f, 1f)                   // 기본 회색
            };
        }

        private string FormatRemainingTime(TimeSpan remaining)
        {
            if (remaining.TotalDays >= 1)
            {
                return $"{(int)remaining.TotalDays}일 {remaining.Hours}시간";
            }
            else if (remaining.TotalHours >= 1)
            {
                return $"{(int)remaining.TotalHours}시간 {remaining.Minutes}분";
            }
            else if (remaining.TotalMinutes >= 1)
            {
                return $"{(int)remaining.TotalMinutes}분";
            }
            else
            {
                return "곧 종료";
            }
        }

        private void OnBannerClicked()
        {
            if (_poolData != null)
            {
                _onSelected?.Invoke(_poolData.Id);
            }
        }
    }
}
