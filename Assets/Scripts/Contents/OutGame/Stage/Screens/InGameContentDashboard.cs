using System;
using Sc.Common.UI;
using Sc.Common.UI.Attributes;
using Sc.Common.UI.Widgets;
using Sc.Contents.Stage.Widgets;
using Sc.Data;
using Sc.Event.UI;
using Sc.Foundation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sc.Contents.Stage
{
    /// <summary>
    /// 인게임 컨텐츠 대시보드.
    /// 컨텐츠 종류(메인스토리, 골드던전, 경험치던전 등)를 선택하는 화면입니다.
    /// 스펙: Docs/Specs/Stage.md - InGameContentDashboard UI 레이아웃 구조
    /// 레퍼런스: Docs/Design/Reference/StageDashbaord.jpg
    /// </summary>
    [ScreenTemplate(ScreenTemplateType.Standard)]
    public class InGameContentDashboard : ScreenWidget<InGameContentDashboard, InGameContentDashboard.DashboardState>
    {
        /// <summary>
        /// 대시보드 상태
        /// </summary>
        public class DashboardState : IScreenState
        {
            /// <summary>
            /// 초기 선택 컨텐츠 타입 (하이라이트용)
            /// </summary>
            public InGameContentType? InitialContentType { get; set; }
        }

        #region Widgets

        [Header("Widgets")] [SerializeField] private ContentProgressWidget _progressWidget;
        [SerializeField] private QuickActionWidget _quickActionWidget;

        #endregion

        #region Content Buttons - Left Side

        [Header("Left Side")] [SerializeField] private Button _shortTermClassButton;
        [SerializeField] private TMP_Text _shortTermClassSeasonText;
        [SerializeField] private Button _dimensionClashButton;
        [SerializeField] private TMP_Text _dimensionClashDungeonText;

        #endregion

        #region Content Buttons - Center Area

        [Header("Center Area")] [SerializeField]
        private Button _nurulingBustersButton;

        [SerializeField] private Button _pvpButton;
        [SerializeField] private Button _mainStoryButton;
        [SerializeField] private TMP_Text _mainStoryProgressText;
        [SerializeField] private TMP_Text _mainStoryStageNameText;
        [SerializeField] private TMP_Text _mainStoryTimeRemainingText;

        #endregion

        #region Content Buttons - Right Side

        [Header("Right Side")] [SerializeField]
        private Button _dungeonButton;

        [SerializeField] private Button _invasionButton;
        [SerializeField] private Button _deckFormationButton;

        #endregion

        #region Right Top Area

        [Header("Right Top Area")] [SerializeField]
        private TMP_Text _stageProgressText;

        [SerializeField] private Button _stageProgressNavigateButton;

        #endregion

        #region Navigation

        [Header("Navigation")] [SerializeField]
        private Button _backButton;

        #endregion

        private DashboardState _currentState;
        private string _currentMainStoryStageId;

        protected override void OnInitialize()
        {
            Debug.Log("[InGameContentDashboard] OnInitialize");

            SetupButtonListeners();
            SetupWidgetCallbacks();
        }

        protected override void OnBind(DashboardState state)
        {
            _currentState = state ?? new DashboardState();

            Debug.Log("[InGameContentDashboard] OnBind");

            // Header 설정
            ScreenHeader.Instance?.Configure("adventure_title");

            RefreshContentButtons();
            RefreshProgressInfo();
        }

        protected override void OnShow()
        {
            Debug.Log("[InGameContentDashboard] OnShow");

            // Header Back 이벤트 구독
            EventManager.Instance?.Subscribe<HeaderBackClickedEvent>(OnHeaderBackClicked);
        }

        protected override void OnHide()
        {
            Debug.Log("[InGameContentDashboard] OnHide");

            // Header Back 이벤트 해제
            EventManager.Instance?.Unsubscribe<HeaderBackClickedEvent>(OnHeaderBackClicked);
        }

        public override DashboardState GetState() => _currentState;

        #region Setup

        private void SetupButtonListeners()
        {
            // Navigation
            if (_backButton != null)
                _backButton.onClick.AddListener(OnBackClicked);

            // Left Side
            if (_shortTermClassButton != null)
                _shortTermClassButton.onClick.AddListener(() =>
                    OnContentButtonClicked(ContentButtonType.ShortTermClass));
            if (_dimensionClashButton != null)
                _dimensionClashButton.onClick.AddListener(() =>
                    OnContentButtonClicked(ContentButtonType.DimensionClash));

            // Center Area
            if (_nurulingBustersButton != null)
                _nurulingBustersButton.onClick.AddListener(() =>
                    OnContentButtonClicked(ContentButtonType.NurulingBusters));
            if (_pvpButton != null)
                _pvpButton.onClick.AddListener(() => OnContentButtonClicked(ContentButtonType.PVP));
            if (_mainStoryButton != null)
                _mainStoryButton.onClick.AddListener(() => OnContentButtonClicked(ContentButtonType.MainStory));

            // Right Side
            if (_dungeonButton != null)
                _dungeonButton.onClick.AddListener(() => OnContentButtonClicked(ContentButtonType.Dungeon));
            if (_invasionButton != null)
                _invasionButton.onClick.AddListener(() => OnContentButtonClicked(ContentButtonType.Invasion));
            if (_deckFormationButton != null)
                _deckFormationButton.onClick.AddListener(() => OnContentButtonClicked(ContentButtonType.DeckFormation));

            // Right Top Area
            if (_stageProgressNavigateButton != null)
                _stageProgressNavigateButton.onClick.AddListener(OnStageProgressNavigateClicked);
        }

        private void SetupWidgetCallbacks()
        {
            if (_progressWidget != null)
            {
                _progressWidget.OnNavigateClicked += OnProgressWidgetNavigateClicked;
            }

            if (_quickActionWidget != null)
            {
                _quickActionWidget.OnQuickEntryClicked += OnQuickEntryClicked;
                _quickActionWidget.OnDeckFormationClicked += OnDeckFormationClicked;
            }
        }

        private void RemoveButtonListeners()
        {
            // Navigation
            if (_backButton != null)
                _backButton.onClick.RemoveAllListeners();

            // Left Side
            if (_shortTermClassButton != null)
                _shortTermClassButton.onClick.RemoveAllListeners();
            if (_dimensionClashButton != null)
                _dimensionClashButton.onClick.RemoveAllListeners();

            // Center Area
            if (_nurulingBustersButton != null)
                _nurulingBustersButton.onClick.RemoveAllListeners();
            if (_pvpButton != null)
                _pvpButton.onClick.RemoveAllListeners();
            if (_mainStoryButton != null)
                _mainStoryButton.onClick.RemoveAllListeners();

            // Right Side
            if (_dungeonButton != null)
                _dungeonButton.onClick.RemoveAllListeners();
            if (_invasionButton != null)
                _invasionButton.onClick.RemoveAllListeners();
            if (_deckFormationButton != null)
                _deckFormationButton.onClick.RemoveAllListeners();

            // Right Top Area
            if (_stageProgressNavigateButton != null)
                _stageProgressNavigateButton.onClick.RemoveAllListeners();
        }

        private void RemoveWidgetCallbacks()
        {
            if (_progressWidget != null)
            {
                _progressWidget.OnNavigateClicked -= OnProgressWidgetNavigateClicked;
            }

            if (_quickActionWidget != null)
            {
                _quickActionWidget.OnQuickEntryClicked -= OnQuickEntryClicked;
                _quickActionWidget.OnDeckFormationClicked -= OnDeckFormationClicked;
            }
        }

        #endregion

        #region Content Refresh

        private void RefreshContentButtons()
        {
            // TODO[P1]: 실제 컨텐츠 상태를 UserData에서 가져와서 설정
            // 현재는 플레이스홀더 데이터 사용

            // Left Side - Short Term Class (단기 속성반)
            if (_shortTermClassSeasonText != null)
            {
                _shortTermClassSeasonText.text = "02/19/11:00 시즌 시작";
            }

            // Left Side - Dimension Clash (차원 대충돌)
            if (_dimensionClashDungeonText != null)
            {
                _dimensionClashDungeonText.text = "딜: 리버리";
            }

            // 버튼 잠금 상태 설정
            SetButtonLockState(_shortTermClassButton, IsContentLocked(ContentButtonType.ShortTermClass));
            SetButtonLockState(_dimensionClashButton, IsContentLocked(ContentButtonType.DimensionClash));
            SetButtonLockState(_nurulingBustersButton, IsContentLocked(ContentButtonType.NurulingBusters));
            SetButtonLockState(_pvpButton, IsContentLocked(ContentButtonType.PVP));
            SetButtonLockState(_dungeonButton, IsContentLocked(ContentButtonType.Dungeon));
            SetButtonLockState(_invasionButton, IsContentLocked(ContentButtonType.Invasion));
        }

        private void RefreshProgressInfo()
        {
            // TODO[P1]: 실제 진행 정보를 UserData에서 가져와서 설정
            // 현재는 플레이스홀더 데이터 사용

            // Right Top Area - Stage Progress Widget
            if (_stageProgressText != null)
            {
                _stageProgressText.text = "11-10 최후의 방어선! 알프트반선!";
            }

            // Center - Main Story Progress
            if (_mainStoryProgressText != null)
            {
                _mainStoryProgressText.text = "제 1 엘리베이터 B7 도전중";
            }

            if (_mainStoryStageNameText != null)
            {
                _mainStoryStageNameText.text = "세계수 급착기지";
            }

            if (_mainStoryTimeRemainingText != null)
            {
                _mainStoryTimeRemainingText.text = "06일 17시간 07분";
            }

            _currentMainStoryStageId = "stage_main_11_10"; // TODO[P1]: 실제 스테이지 ID

            // Progress Widget 설정
            if (_progressWidget != null)
            {
                _progressWidget.Configure(
                    InGameContentType.MainStory,
                    "제 1 엘리베이터 B7 도전중",
                    "세계수 급착기지",
                    _currentMainStoryStageId
                );
                _progressWidget.SetTimeRemaining(TimeSpan.FromDays(6).Add(TimeSpan.FromHours(17))
                    .Add(TimeSpan.FromMinutes(7)));
            }

            // Quick Action Widget 설정
            if (_quickActionWidget != null)
            {
                _quickActionWidget.SetQuickEntryState(false, "빠른전투불가");
                _quickActionWidget.SetAutoRepeatState(false);
                _quickActionWidget.SetSkipTicketCount(0);
            }
        }

        private void SetButtonLockState(Button button, bool isLocked)
        {
            if (button == null) return;

            button.interactable = !isLocked;

            // TODO[P2]: 잠금 아이콘/이펙트 표시
            var canvasGroup = button.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = isLocked ? 0.5f : 1f;
            }
        }

        private bool IsContentLocked(ContentButtonType buttonType)
        {
            // TODO[P1]: 실제 해금 조건 확인 (UserData 기반)
            return buttonType switch
            {
                ContentButtonType.MainStory => false,
                ContentButtonType.Dungeon => false,
                ContentButtonType.DeckFormation => false,
                ContentButtonType.ShortTermClass => true,
                ContentButtonType.DimensionClash => true,
                ContentButtonType.NurulingBusters => true,
                ContentButtonType.PVP => true,
                ContentButtonType.Invasion => true,
                _ => true
            };
        }

        #endregion

        #region Content Button Handlers

        private enum ContentButtonType
        {
            MainStory,
            Dungeon,
            Invasion,
            ShortTermClass,
            DimensionClash,
            NurulingBusters,
            PVP,
            DeckFormation
        }

        private void OnContentButtonClicked(ContentButtonType buttonType)
        {
            Debug.Log($"[InGameContentDashboard] Content button clicked: {buttonType}");

            switch (buttonType)
            {
                case ContentButtonType.MainStory:
                    OpenStageSelectScreen(InGameContentType.MainStory);
                    break;

                case ContentButtonType.Dungeon:
                    OpenStageDashboard(InGameContentType.GoldDungeon);
                    break;

                case ContentButtonType.Invasion:
                    ShowComingSoonPopup("침공전");
                    break;

                case ContentButtonType.ShortTermClass:
                    ShowComingSoonPopup("단기 속성반");
                    break;

                case ContentButtonType.DimensionClash:
                    ShowComingSoonPopup("차원 대충돌");
                    break;

                case ContentButtonType.NurulingBusters:
                    ShowComingSoonPopup("누룽버스터즈");
                    break;

                case ContentButtonType.PVP:
                    ShowComingSoonPopup("PVP");
                    break;

                case ContentButtonType.DeckFormation:
                    ShowComingSoonPopup("덱 편성");
                    break;
            }
        }

        private void ShowComingSoonPopup(string featureName)
        {
            Debug.Log($"[InGameContentDashboard] {featureName} not implemented yet");

            ConfirmPopup.Open(new ConfirmState
            {
                Title = "준비 중",
                Message = $"{featureName} 기능은\n추후 업데이트될 예정입니다.",
                ConfirmText = "확인",
                ShowCancelButton = false
            });
        }

        private void OpenStageSelectScreen(InGameContentType contentType)
        {
            StageSelectScreen.Open(new StageSelectScreen.StageSelectState
            {
                ContentType = contentType
            });
        }

        private void OpenStageDashboard(InGameContentType contentType)
        {
            StageDashboard.Open(new StageDashboard.StageDashboardState
            {
                ContentType = contentType
            });
        }

        #endregion

        #region Widget Handlers

        private void OnProgressWidgetNavigateClicked(InGameContentType contentType, string stageId)
        {
            Debug.Log($"[InGameContentDashboard] Progress navigate clicked: {contentType}, {stageId}");
            OpenStageSelectScreen(contentType);
        }

        private void OnStageProgressNavigateClicked()
        {
            Debug.Log("[InGameContentDashboard] Stage progress navigate clicked");
            OpenStageSelectScreen(InGameContentType.MainStory);
        }

        private void OnQuickEntryClicked()
        {
            Debug.Log("[InGameContentDashboard] Quick entry clicked");
            // TODO[FUTURE]: 빠른 전투 로직 (InGame 시스템)
        }

        private void OnDeckFormationClicked()
        {
            Debug.Log("[InGameContentDashboard] Deck formation clicked");
            OnContentButtonClicked(ContentButtonType.DeckFormation);
        }

        #endregion

        #region Navigation

        private void OnBackClicked()
        {
            Debug.Log("[InGameContentDashboard] Back clicked");
            NavigationManager.Instance?.Back();
        }

        private void OnHeaderBackClicked(HeaderBackClickedEvent evt)
        {
            OnBackClicked();
        }

        #endregion

        protected override void OnRelease()
        {
            RemoveButtonListeners();
            RemoveWidgetCallbacks();
        }
    }
}