using System;
using System.Text;
using Sc.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sc.Contents.Gacha
{
    /// <summary>
    /// 가챠 히스토리 아이템 위젯
    /// </summary>
    public class GachaHistoryItem : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_Text _poolNameText;
        [SerializeField] private TMP_Text _dateTimeText;
        [SerializeField] private TMP_Text _pullTypeText;
        [SerializeField] private TMP_Text _resultsText;
        [SerializeField] private TMP_Text _summaryText;
        [SerializeField] private Button _expandButton;
        [SerializeField] private GameObject _expandedPanel;
        [SerializeField] private Image _backgroundImage;

        private GachaHistoryRecord _record;
        private bool _isExpanded;

        private void Awake()
        {
            if (_expandButton != null)
            {
                _expandButton.onClick.AddListener(ToggleExpand);
            }
        }

        private void OnDestroy()
        {
            if (_expandButton != null)
            {
                _expandButton.onClick.RemoveListener(ToggleExpand);
            }
        }

        /// <summary>
        /// 히스토리 레코드 설정
        /// </summary>
        public void Setup(GachaHistoryRecord record)
        {
            _record = record;
            _isExpanded = false;

            RefreshUI();
        }

        private void RefreshUI()
        {
            // 풀 이름
            if (_poolNameText != null)
            {
                _poolNameText.text = _record.PoolName;
            }

            // 날짜/시간
            if (_dateTimeText != null)
            {
                var dateTime = DateTimeOffset.FromUnixTimeSeconds(_record.Timestamp).LocalDateTime;
                _dateTimeText.text = dateTime.ToString("yyyy-MM-dd HH:mm");
            }

            // 뽑기 유형
            if (_pullTypeText != null)
            {
                _pullTypeText.text = _record.PullType == GachaPullType.Single ? "1회" : "10연차";
            }

            // 요약 정보
            if (_summaryText != null)
            {
                var sb = new StringBuilder();

                if (_record.SSRCount > 0)
                {
                    sb.Append($"<color=#FFD700>SSR x{_record.SSRCount}</color>");
                }

                if (_record.SRCount > 0)
                {
                    if (sb.Length > 0) sb.Append(" | ");
                    sb.Append($"<color=#9932CC>SR x{_record.SRCount}</color>");
                }

                if (_record.RCount > 0)
                {
                    if (sb.Length > 0) sb.Append(" | ");
                    sb.Append($"<color=#4169E1>R x{_record.RCount}</color>");
                }

                _summaryText.text = sb.ToString();
            }

            // 배경색 (SSR 여부에 따라)
            if (_backgroundImage != null)
            {
                _backgroundImage.color = _record.SSRCount > 0
                    ? new Color(1f, 0.95f, 0.8f, 1f)   // 금빛 배경
                    : new Color(0.95f, 0.95f, 0.95f, 1f); // 기본 배경
            }

            // 확장 패널 상태
            UpdateExpandState();
        }

        private void ToggleExpand()
        {
            _isExpanded = !_isExpanded;
            UpdateExpandState();
        }

        private void UpdateExpandState()
        {
            if (_expandedPanel != null)
            {
                _expandedPanel.SetActive(_isExpanded);
            }

            // 결과 상세 (확장 시에만)
            if (_resultsText != null && _isExpanded)
            {
                RefreshResultsDetail();
            }
        }

        private void RefreshResultsDetail()
        {
            if (_resultsText == null || _record.Results == null) return;

            var sb = new StringBuilder();
            sb.AppendLine("<b>상세 결과</b>");
            sb.AppendLine();

            for (int i = 0; i < _record.Results.Count; i++)
            {
                var result = _record.Results[i];
                var charName = GetCharacterName(result.CharacterId);
                var rarityColor = GetRarityColorHex(result.Rarity);

                sb.Append($"<color={rarityColor}>[{result.Rarity}] {charName}</color>");

                if (result.IsNew)
                {
                    sb.Append(" <color=#FFD700>NEW</color>");
                }

                if (result.IsPity)
                {
                    sb.Append(" <color=#FF4500>PITY</color>");
                }

                sb.AppendLine();
            }

            _resultsText.text = sb.ToString();
        }

        private string GetCharacterName(string characterId)
        {
            var charData = Sc.Core.DataManager.Instance?.Characters?.GetById(characterId);
            return charData?.Name ?? characterId;
        }

        private string GetRarityColorHex(Rarity rarity)
        {
            return rarity switch
            {
                Rarity.SSR => "#FFD700",
                Rarity.SR => "#9932CC",
                Rarity.R => "#4169E1",
                _ => "#888888"
            };
        }
    }
}
