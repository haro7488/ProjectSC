using Sc.Common.UI;
using Sc.Contents.Stage;
using Sc.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sc.Contents.Event
{
    /// <summary>
    /// 이벤트 스테이지 탭.
    /// 이벤트 스테이지 목록으로 이동하는 버튼 제공.
    /// </summary>
    public class EventStageTab : Widget
    {
        [Header("UI References")]
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private Button _stageListButton;
        [SerializeField] private TMP_Text _stageListButtonText;

        [Header("Legacy (Hide)")]
        [SerializeField] private TMP_Text _placeholderText;
        [SerializeField] private GameObject _stageListContainer;

        private string _eventId;
        private string _stageGroupId;

        protected override void OnInitialize()
        {
            Debug.Log("[EventStageTab] OnInitialize");

            if (_stageListButton != null)
            {
                _stageListButton.onClick.AddListener(OnStageListClicked);
            }
        }

        /// <summary>
        /// 스테이지 탭 설정
        /// </summary>
        public void Setup(string eventId, string stageGroupId)
        {
            _eventId = eventId;
            _stageGroupId = stageGroupId;

            Debug.Log($"[EventStageTab] Setup - EventId: {eventId}, StageGroupId: {stageGroupId}");

            RefreshUI();
        }

        private void RefreshUI()
        {
            // 플레이스홀더 숨기기
            if (_placeholderText != null)
            {
                _placeholderText.gameObject.SetActive(false);
            }

            if (_stageListContainer != null)
            {
                _stageListContainer.SetActive(false);
            }

            // 타이틀
            if (_titleText != null)
            {
                _titleText.text = "이벤트 스테이지";
            }

            // 설명
            if (_descriptionText != null)
            {
                _descriptionText.text = "이벤트 전용 스테이지에서\n특별한 보상을 획득하세요!";
            }

            // 버튼 텍스트
            if (_stageListButtonText != null)
            {
                _stageListButtonText.text = "스테이지 목록";
            }

            // 버튼 표시
            if (_stageListButton != null)
            {
                _stageListButton.gameObject.SetActive(true);
            }
        }

        private void OnStageListClicked()
        {
            Debug.Log($"[EventStageTab] OnStageListClicked - navigating to StageSelectScreen");

            // StageSelectScreen으로 이동
            StageSelectScreen.Open(new StageSelectScreen.StageSelectState
            {
                ContentType = InGameContentType.Event,
                CategoryId = _stageGroupId
            });
        }

        protected override void OnShow()
        {
            Debug.Log("[EventStageTab] OnShow");
        }

        protected override void OnHide()
        {
            Debug.Log("[EventStageTab] OnHide");
        }

        protected override void OnRelease()
        {
            if (_stageListButton != null)
            {
                _stageListButton.onClick.RemoveListener(OnStageListClicked);
            }

            _eventId = null;
            _stageGroupId = null;
        }
    }
}
