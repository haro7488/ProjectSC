using System;
using Sc.Core;
using Sc.Data;
using Sc.Foundation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sc.Contents.Stage
{
    /// <summary>
    /// 이벤트 스테이지 컨텐츠 모듈.
    /// 이벤트 이름, 남은 기간, 이벤트 재화 표시.
    /// </summary>
    public class EventStageContentModule : BaseStageContentModule
    {
        private TMP_Text _eventNameText;
        private TMP_Text _remainingTimeText;
        private TMP_Text _eventCurrencyText;
        private Image _eventBanner;
        private Image _currencyIcon;

        private LiveEventData _eventData;
        private string _eventId;

        protected override string GetPrefabAddress()
        {
            // UI 프리팹이 준비되면 아래 주석 해제
            // return "UI/Stage/EventStageModuleUI";
            return null;
        }

        protected override void OnInitialize()
        {
            Log.Debug("[EventStageContentModule] OnInitialize", LogCategory.UI);

            // UI 컴포넌트 찾기 (프리팹 있을 경우)
            if (_rootInstance != null)
            {
                _eventNameText = FindTransform("EventName")?.GetComponent<TMP_Text>();
                _remainingTimeText = FindTransform("RemainingTime")?.GetComponent<TMP_Text>();
                _eventCurrencyText = FindTransform("EventCurrency")?.GetComponent<TMP_Text>();
                _eventBanner = FindTransform("EventBanner")?.GetComponent<Image>();
                _currencyIcon = FindTransform("CurrencyIcon")?.GetComponent<Image>();
            }

            // 이벤트 데이터 로드
            LoadEventData();

            // UI 업데이트
            UpdateUI();
        }

        protected override void OnRefreshInternal(string selectedStageId)
        {
            // 남은 시간, 이벤트 재화 갱신
            UpdateRemainingTime();
            UpdateEventCurrency();
        }

        protected override void OnStageSelectedInternal(StageData stageData)
        {
            // 선택된 스테이지 기반 추가 정보 (필요 시)
            Log.Debug($"[EventStageContentModule] Event stage selected: {stageData?.Id}", LogCategory.UI);
        }

        protected override void OnCategoryIdChanged(string categoryId)
        {
            // 카테고리 ID가 이벤트 ID로 사용됨
            _eventId = categoryId;
            LoadEventData();
            UpdateUI();
        }

        protected override void OnReleaseInternal()
        {
            _eventData = null;
            _eventId = null;
        }

        /// <summary>
        /// 외부에서 이벤트 ID 설정
        /// </summary>
        public void SetEventId(string eventId)
        {
            _eventId = eventId;
            LoadEventData();
            UpdateUI();
        }

        #region Private Methods

        private void LoadEventData()
        {
            // 카테고리 ID가 없으면 초기 카테고리 사용
            if (string.IsNullOrEmpty(_eventId))
            {
                _eventId = _categoryId;
            }

            if (string.IsNullOrEmpty(_eventId))
            {
                Log.Debug("[EventStageContentModule] No eventId set", LogCategory.UI);
                _eventData = null;
                return;
            }

            var eventDb = DataManager.Instance?.GetDatabase<LiveEventDatabase>();
            if (eventDb == null)
            {
                Log.Warning("[EventStageContentModule] LiveEventDatabase not found", LogCategory.UI);
                _eventData = null;
                return;
            }

            _eventData = eventDb.GetById(_eventId);

            if (_eventData == null)
            {
                Log.Warning($"[EventStageContentModule] Event not found: {_eventId}", LogCategory.UI);
            }
            else
            {
                Log.Debug($"[EventStageContentModule] Loaded event: {_eventData.Id}, Name: {_eventData.NameKey}", LogCategory.UI);
            }
        }

        private void UpdateUI()
        {
            if (_eventData == null)
            {
                ClearUI();
                return;
            }

            // 이벤트 이름 (NameKey 사용, 로컬라이제이션 필요 시 확장)
            if (_eventNameText != null)
            {
                _eventNameText.text = _eventData.NameKey;
            }

            // 이벤트 배너 (BannerImage는 Addressables 주소, 필요 시 비동기 로드)
            if (_eventBanner != null && !string.IsNullOrEmpty(_eventData.BannerImage))
            {
                // TODO: AssetManager.LoadAsync로 배너 이미지 로드
                _eventBanner.gameObject.SetActive(true);
            }

            // 남은 시간
            UpdateRemainingTime();

            // 이벤트 재화
            UpdateEventCurrency();
        }

        private void ClearUI()
        {
            if (_eventNameText != null)
            {
                _eventNameText.text = "";
            }

            if (_remainingTimeText != null)
            {
                _remainingTimeText.text = "";
            }

            if (_eventCurrencyText != null)
            {
                _eventCurrencyText.text = "";
            }

            if (_eventBanner != null)
            {
                _eventBanner.gameObject.SetActive(false);
            }
        }

        private void UpdateRemainingTime()
        {
            if (_remainingTimeText == null || _eventData == null) return;

            var now = DateTime.UtcNow;
            var endTime = _eventData.EndTime;
            var remaining = endTime - now;

            if (remaining.TotalSeconds <= 0)
            {
                _remainingTimeText.text = "종료됨";
                _remainingTimeText.color = Color.gray;
                return;
            }

            _remainingTimeText.text = FormatRemainingTime(remaining);
            _remainingTimeText.color = remaining.TotalHours < 24 ? Color.red : Color.white; // 24시간 미만이면 빨간색
        }

        private void UpdateEventCurrency()
        {
            if (_eventCurrencyText == null || _eventData == null) return;

            // 유저의 이벤트 재화 조회
            var currencyAmount = 0;
            if (DataManager.Instance != null && _eventData.HasEventCurrency)
            {
                var currencyId = _eventData.CurrencyPolicy.CurrencyId;
                if (!string.IsNullOrEmpty(currencyId))
                {
                    var eventCurrency = DataManager.Instance.EventCurrency;
                    currencyAmount = eventCurrency.GetAmount(_eventData.Id, currencyId);
                }
            }

            _eventCurrencyText.text = $"{currencyAmount:N0}";
        }

        private string FormatRemainingTime(TimeSpan timeSpan)
        {
            if (timeSpan.TotalDays >= 1)
            {
                return $"남은 기간: {(int)timeSpan.TotalDays}일 {timeSpan.Hours}시간";
            }
            else if (timeSpan.TotalHours >= 1)
            {
                return $"남은 기간: {(int)timeSpan.TotalHours}시간 {timeSpan.Minutes}분";
            }
            else
            {
                return $"남은 기간: {timeSpan.Minutes}분";
            }
        }

        #endregion
    }
}
