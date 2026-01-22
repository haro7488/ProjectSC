using Cysharp.Threading.Tasks;
using Sc.Common.UI;
using Sc.Common.UI.Widgets;
using Sc.Contents.Event;
using Sc.Contents.Gacha;
using Sc.Contents.Shop;
using Sc.Core;
using Sc.Foundation;
using Sc.LocalServer;
using TMPro;
using UnityEngine;

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

        [Header("UI References")]
        [SerializeField] private TMP_Text _welcomeText;

        private LobbyState _currentState;
        private LobbyEntryTaskRunner _taskRunner;
        private bool _isTaskRunning;

        protected override void OnInitialize()
        {
            Debug.Log("[LobbyScreen] OnInitialize 시작");
            Debug.Log($"[LobbyScreen] _tabGroup:{_tabGroup != null}, _tabContents:{_tabContents != null}, Length:{_tabContents?.Length ?? 0}");

            if (_tabGroup == null || _tabContents == null || _tabContents.Length == 0)
            {
                Debug.LogError("[LobbyScreen] TabGroup 또는 TabContents가 설정되지 않았습니다.");
                return;
            }

            for (int i = 0; i < _tabContents.Length; i++)
            {
                Debug.Log($"[LobbyScreen] TabContent[{i}]: {(_tabContents[i] != null ? _tabContents[i].name : "NULL")}");
            }

            InitializeTabSystem();
            InitializeTaskRunner();
            RegisterBadgeProviders();
            Debug.Log("[LobbyScreen] OnInitialize 완료");
        }

        private void InitializeTabSystem()
        {
            Debug.Log("[LobbyScreen] InitializeTabSystem 시작");
            _tabGroup.OnTabChanged += OnTabChanged;

            // 탭 컨텐츠 초기 비활성화
            for (int i = 0; i < _tabContents.Length; i++)
            {
                if (_tabContents[i] != null)
                {
                    _tabContents[i].gameObject.SetActive(false);
                    Debug.Log($"[LobbyScreen] TabContent[{i}] 비활성화됨");
                }
            }
            Debug.Log("[LobbyScreen] InitializeTabSystem 완료");
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
            Debug.Log("[LobbyScreen] OnShow 시작");
            Debug.Log($"[LobbyScreen] _tabGroup:{_tabGroup != null}, _tabContents:{_tabContents?.Length ?? 0}");

            // DataManager 이벤트 구독
            if (DataManager.Instance != null)
            {
                DataManager.Instance.OnUserDataChanged += OnUserDataChanged;
            }

            // 배지 전체 갱신
            BadgeManager.Instance?.RefreshAll();
            RefreshAllBadges();

            // 초기 탭 선택
            int initialTab = _currentState?.ActiveTabIndex ?? 0;
            Debug.Log($"[LobbyScreen] SelectTab 호출 예정 - index:{initialTab}");
            _tabGroup.SelectTab(initialTab);
            Debug.Log("[LobbyScreen] SelectTab 호출 완료");

            RefreshUI();

            // 로비 진입 후처리 Task 실행
            RunLobbyEntryTasksAsync().Forget();
            Debug.Log("[LobbyScreen] OnShow 완료");
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
            if (_currentState != null && _tabGroup != null)
            {
                _currentState.ActiveTabIndex = _tabGroup.CurrentTabIndex;
            }
            return _currentState;
        }

        #region Tab System

        private void OnTabChanged(int index)
        {
            Debug.Log($"[LobbyScreen] OnTabChanged 호출됨 - index:{index}");
            
            if (_tabContents == null)
            {
                Debug.LogError("[LobbyScreen] OnTabChanged - _tabContents가 null!");
                return;
            }

            for (int i = 0; i < _tabContents.Length; i++)
            {
                if (_tabContents[i] == null)
                {
                    Debug.LogWarning($"[LobbyScreen] TabContent[{i}]가 null");
                    continue;
                }

                if (i == index)
                {
                    Debug.Log($"[LobbyScreen] TabContent[{i}] 활성화 - {_tabContents[i].name}");
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
            int tabIndex = GetTabIndexForBadge(type);
            if (tabIndex >= 0 && _tabGroup != null)
            {
                _tabGroup.SetBadgeCount(tabIndex, count);
            }
        }

        private void RefreshAllBadges()
        {
            if (BadgeManager.Instance == null || _tabGroup == null)
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
    }
}
