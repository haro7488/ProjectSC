using Cysharp.Threading.Tasks;
using Sc.Common.UI;
using Sc.Common.UI.Widgets;
using Sc.Contents.Character;
using Sc.Contents.Event;
using Sc.Contents.Gacha;
using Sc.Contents.Shop;
using Sc.Contents.Stage;
using Sc.Core;
using Sc.Foundation;
using Sc.LocalServer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sc.Contents.Lobby
{
    /// <summary>
    /// 로비 화면 State
    /// </summary>
    public class LobbyState : IScreenState
    {
        /// <summary>
        /// 현재 선택된 탭 인덱스
        /// </summary>
        public int ActiveTabIndex;
    }

    /// <summary>
    /// 로비 화면 - 메인 허브 화면
    /// </summary>
    public class LobbyScreen : ScreenWidget<LobbyScreen, LobbyState>
    {
        [Header("Tab System")]
        [SerializeField] private TabGroupWidget _tabGroup;
        [SerializeField] private LobbyTabContent[] _tabContents;

        [Header("Legacy Buttons (deprecated)")]
        [SerializeField] private Button _gachaButton;
        [SerializeField] private Button _characterButton;
        [SerializeField] private Button _eventButton;
        [SerializeField] private Button _shopButton;
        [SerializeField] private Button _stageButton;

        [Header("UI References")]
        [SerializeField] private TMP_Text _welcomeText;

        private LobbyState _currentState;
        private LobbyEntryTaskRunner _taskRunner;
        private bool _isTaskRunning;
        private bool _useTabSystem;

        protected override void OnInitialize()
        {
            Debug.Log("[LobbyScreen] OnInitialize");

            // 탭 시스템 사용 여부 결정
            _useTabSystem = _tabGroup != null && _tabContents != null && _tabContents.Length > 0;

            if (_useTabSystem)
            {
                InitializeTabSystem();
            }
            else
            {
                InitializeLegacyButtons();
            }

            // Task Runner 초기화
            InitializeTaskRunner();

            // Badge Provider 등록
            RegisterBadgeProviders();
        }

        private void InitializeTabSystem()
        {
            Debug.Log("[LobbyScreen] Using Tab System");

            _tabGroup.OnTabChanged += OnTabChanged;

            // 탭 컨텐츠 초기 비활성화
            for (int i = 0; i < _tabContents.Length; i++)
            {
                if (_tabContents[i] != null)
                {
                    _tabContents[i].gameObject.SetActive(false);
                }
            }
        }

        private void InitializeLegacyButtons()
        {
            Debug.Log("[LobbyScreen] Using Legacy Buttons");

            if (_gachaButton != null)
                _gachaButton.onClick.AddListener(OnGachaButtonClicked);
            if (_characterButton != null)
                _characterButton.onClick.AddListener(OnCharacterButtonClicked);
            if (_eventButton != null)
                _eventButton.onClick.AddListener(OnEventButtonClicked);
            if (_shopButton != null)
                _shopButton.onClick.AddListener(OnShopButtonClicked);
            if (_stageButton != null)
                _stageButton.onClick.AddListener(OnStageButtonClicked);
        }

        private void InitializeTaskRunner()
        {
            var popupQueue = new PopupQueueService();
            _taskRunner = new LobbyEntryTaskRunner(popupQueue);

            // Task 등록
            _taskRunner.RegisterTask(new AttendanceCheckTask());

            // EventCurrencyConversionTask
            var eventDatabase = DataManager.Instance?.LiveEvents;
            if (eventDatabase != null)
            {
                var timeService = new ServerTimeService();
                var converter = new EventCurrencyConverter(eventDatabase, timeService);
                _taskRunner.RegisterTask(new EventCurrencyConversionTask(converter, DataManager.Instance));
            }
            else
            {
                Log.Warning("[LobbyScreen] EventCurrencyConverter 초기화 실패 - LiveEventDatabase 누락", LogCategory.Lobby);
            }

            _taskRunner.RegisterTask(new NewEventNotificationTask());

            Debug.Log("[LobbyScreen] TaskRunner 초기화 완료");
        }

        private void RegisterBadgeProviders()
        {
            if (BadgeManager.Instance == null)
                return;

            BadgeManager.Instance.Register(new EventBadgeProvider());
            BadgeManager.Instance.Register(new ShopBadgeProvider());
            BadgeManager.Instance.Register(new GachaBadgeProvider());

            BadgeManager.Instance.OnBadgeChanged += OnBadgeChanged;
        }

        protected override void OnBind(LobbyState state)
        {
            _currentState = state ?? new LobbyState();
            Debug.Log($"[LobbyScreen] OnBind - Tab: {_currentState.ActiveTabIndex}");

            // Header 설정
            ScreenHeader.Instance?.Configure("lobby_default");

            RefreshUI();
        }

        protected override void OnShow()
        {
            Debug.Log("[LobbyScreen] OnShow");

            // DataManager 이벤트 구독
            if (DataManager.Instance != null)
            {
                DataManager.Instance.OnUserDataChanged += OnUserDataChanged;
            }

            // 배지 전체 갱신
            BadgeManager.Instance?.RefreshAll();

            if (_useTabSystem)
            {
                RefreshAllBadges();

                // 초기 탭 선택
                int initialTab = _currentState?.ActiveTabIndex ?? 0;
                _tabGroup.SelectTab(initialTab);
            }

            RefreshUI();

            // 로비 진입 후처리 Task 실행
            RunLobbyEntryTasksAsync().Forget();
        }

        private async UniTaskVoid RunLobbyEntryTasksAsync()
        {
            if (_taskRunner == null || _isTaskRunning)
                return;

            _isTaskRunning = true;

            try
            {
                var summary = await _taskRunner.ExecuteAllAsync();

                Log.Info($"[LobbyScreen] Tasks completed: " +
                         $"Executed={summary.ExecutedTasks}, " +
                         $"Skipped={summary.SkippedTasks}, " +
                         $"Failed={summary.FailedTasks}",
                    LogCategory.Lobby);

                // 팝업 큐 처리
                await _taskRunner.PopupQueue.ProcessQueueAsync();
            }
            catch (System.Exception ex)
            {
                Log.Error($"[LobbyScreen] Task 실행 예외: {ex.Message}", LogCategory.Lobby);
            }
            finally
            {
                _isTaskRunning = false;
            }
        }

        protected override void OnHide()
        {
            Debug.Log("[LobbyScreen] OnHide");

            if (DataManager.Instance != null)
            {
                DataManager.Instance.OnUserDataChanged -= OnUserDataChanged;
            }
        }

        private void OnDestroy()
        {
            if (BadgeManager.Instance != null)
            {
                BadgeManager.Instance.OnBadgeChanged -= OnBadgeChanged;
            }
        }

        public override LobbyState GetState()
        {
            if (_currentState != null && _useTabSystem)
            {
                _currentState.ActiveTabIndex = _tabGroup.CurrentTabIndex;
            }
            return _currentState;
        }

        #region Tab System

        private void OnTabChanged(int index)
        {
            if (!_useTabSystem || _tabContents == null)
                return;

            for (int i = 0; i < _tabContents.Length; i++)
            {
                if (_tabContents[i] == null)
                    continue;

                if (i == index)
                {
                    _tabContents[i].gameObject.SetActive(true);
                    _tabContents[i].OnTabSelected();
                }
                else
                {
                    _tabContents[i].OnTabDeselected();
                    _tabContents[i].gameObject.SetActive(false);
                }
            }

            Debug.Log($"[LobbyScreen] Tab changed to {index}");
        }

        private void OnBadgeChanged(BadgeType type, int count)
        {
            if (!_useTabSystem)
                return;

            int tabIndex = GetTabIndexForBadge(type);
            if (tabIndex >= 0)
            {
                _tabGroup.SetBadgeCount(tabIndex, count);
            }
        }

        private void RefreshAllBadges()
        {
            if (!_useTabSystem || BadgeManager.Instance == null)
                return;

            _tabGroup.SetBadgeCount(0, BadgeManager.Instance.GetBadge(BadgeType.Home));
            _tabGroup.SetBadgeCount(1, BadgeManager.Instance.GetBadge(BadgeType.Character));
            _tabGroup.SetBadgeCount(2, BadgeManager.Instance.GetBadge(BadgeType.Gacha));
            // Settings 탭 (index 3)은 배지 없음
        }

        private int GetTabIndexForBadge(BadgeType type)
        {
            return type switch
            {
                BadgeType.Home => 0,
                BadgeType.Character => 1,
                BadgeType.Gacha => 2,
                _ => -1
            };
        }

        #endregion

        #region UI Refresh

        private void RefreshUI()
        {
            if (_welcomeText != null && DataManager.Instance?.IsInitialized == true)
            {
                var nickname = DataManager.Instance.Profile.Nickname;
                _welcomeText.text = $"환영합니다, {nickname}님!";
            }
        }

        private void OnUserDataChanged()
        {
            RefreshUI();

            // 배지 갱신
            BadgeManager.Instance?.RefreshAll();
        }

        #endregion

        #region Legacy Button Handlers

        private void OnGachaButtonClicked()
        {
            Debug.Log("[LobbyScreen] Gacha button clicked");
            GachaScreen.Open(new GachaState());
        }

        private void OnCharacterButtonClicked()
        {
            Debug.Log("[LobbyScreen] Character button clicked");
            CharacterListScreen.Open(new CharacterListState());
        }

        private void OnEventButtonClicked()
        {
            Debug.Log("[LobbyScreen] Event button clicked");
            LiveEventScreen.Open(new LiveEventState());
        }

        private void OnShopButtonClicked()
        {
            Debug.Log("[LobbyScreen] Shop button clicked");
            ShopScreen.Open(new ShopScreen.ShopState());
        }

        private void OnStageButtonClicked()
        {
            Debug.Log("[LobbyScreen] Stage button clicked");
            InGameContentDashboard.Open(new InGameContentDashboard.DashboardState());
        }

        #endregion
    }
}
