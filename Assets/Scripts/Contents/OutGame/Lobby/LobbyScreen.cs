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
        [Header("UI References")] [SerializeField]
        private Button _gachaButton;

        [SerializeField] private Button _characterButton;
        [SerializeField] private Button _eventButton;
        [SerializeField] private Button _shopButton;
        [SerializeField] private Button _stageButton;
        [SerializeField] private TMP_Text _welcomeText;

        private LobbyState _currentState;
        private LobbyEntryTaskRunner _taskRunner;
        private bool _isTaskRunning;

        protected override void OnInitialize()
        {
            Debug.Log("[LobbyScreen] OnInitialize");

            // 버튼 연결
            if (_gachaButton != null)
            {
                _gachaButton.onClick.AddListener(OnGachaButtonClicked);
            }

            if (_characterButton != null)
            {
                _characterButton.onClick.AddListener(OnCharacterButtonClicked);
            }

            if (_eventButton != null)
            {
                _eventButton.onClick.AddListener(OnEventButtonClicked);
            }

            if (_shopButton != null)
            {
                _shopButton.onClick.AddListener(OnShopButtonClicked);
            }

            if (_stageButton != null)
            {
                _stageButton.onClick.AddListener(OnStageButtonClicked);
            }

            // Task Runner 초기화
            InitializeTaskRunner();
        }

        private void InitializeTaskRunner()
        {
            var popupQueue = new PopupQueueService();
            _taskRunner = new LobbyEntryTaskRunner(popupQueue);

            // Task 등록
            _taskRunner.RegisterTask(new AttendanceCheckTask());

            // EventCurrencyConversionTask (EventCurrencyConverter 필요)
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

            RefreshUI();

            // 로비 진입 후처리 Task 실행
            RunLobbyEntryTasksAsync().Forget();
        }

        private async UniTaskVoid RunLobbyEntryTasksAsync()
        {
            if (_taskRunner == null || _isTaskRunning)
            {
                return;
            }

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

            // DataManager 이벤트 해제
            if (DataManager.Instance != null)
            {
                DataManager.Instance.OnUserDataChanged -= OnUserDataChanged;
            }
        }

        public override LobbyState GetState() => _currentState;

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
        }

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
    }
}