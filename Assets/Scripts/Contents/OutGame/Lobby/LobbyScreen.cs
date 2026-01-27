using System.Linq;
using Cysharp.Threading.Tasks;
using Sc.Common.UI;
using Sc.Common.UI.Attributes;
using Sc.Common.UI.Widgets;
using Sc.Contents.Character;
using Sc.Contents.Event;
using Sc.Contents.Gacha;
using Sc.Contents.Lobby.Widgets;
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
    /// 로비 메인 화면.
    /// 스펙: Docs/Specs/Lobby.md
    /// </summary>
    [ScreenTemplate(ScreenTemplateType.Tabbed)]
    public class LobbyScreen : ScreenWidget<LobbyScreen, EmptyScreenState>
    {
        #region SerializeFields

        [Header("Left Top Area")] [SerializeField]
        private EventBannerCarousel _eventBannerCarousel;

        [SerializeField] private PassButton[] _passButtons;

        [Header("Right Top Area")] [SerializeField]
        private StageProgressWidget _stageProgressWidget;

        [SerializeField] private QuickMenuButton[] _quickMenuButtons;

        [Header("Center Area")] [SerializeField]
        private CharacterDisplayWidget _characterDisplay;

        [Header("Right Bottom Area")] [SerializeField]
        private GameObject _inGameDashboard;

        [SerializeField] private Button _stageShortcutButton;
        [SerializeField] private TMP_Text _stageShortcutLabel;
        [SerializeField] private Button _adventureButton;

        [Header("Bottom Nav")] [SerializeField]
        private ContentNavButton[] _contentNavButtons;

        [SerializeField] private ScrollRect _bottomNavScroll;

        #endregion

        #region Private Fields

        private LobbyEntryTaskRunner _taskRunner;
        private bool _isTaskRunning;
        private int _currentCharacterIndex;

        #endregion

        #region Lifecycle

        protected override void OnInitialize()
        {
            Debug.Log("[LobbyScreen] OnInitialize");

            InitializeEventBanner();
            InitializePassButtons();
            InitializeQuickMenu();
            InitializeCharacterDisplay();
            InitializeInGameDashboard();
            InitializeBottomNav();
            InitializeTaskRunner();
            RegisterBadgeProviders();
        }

        protected override void OnBind(EmptyScreenState state)
        {
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

            // 배지 갱신
            BadgeManager.Instance?.RefreshAll();
            RefreshAllBadges();

            RefreshUI();

            // 로비 진입 후처리 Task 실행
            RunLobbyEntryTasksAsync().Forget();
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

            CleanupEventHandlers();
        }

        #endregion

        #region Initialization

        private void InitializeEventBanner()
        {
            if (_eventBannerCarousel == null) return;

            _eventBannerCarousel.OnBannerClicked += OnBannerClicked;
        }

        private void InitializePassButtons()
        {
            if (_passButtons == null) return;

            foreach (var button in _passButtons)
            {
                if (button != null)
                {
                    button.OnClicked += OnPassButtonClicked;
                }
            }
        }

        private void InitializeQuickMenu()
        {
            if (_quickMenuButtons == null) return;

            foreach (var button in _quickMenuButtons)
            {
                if (button != null)
                {
                    button.OnClicked += OnQuickMenuClicked;
                }
            }
        }

        private void InitializeCharacterDisplay()
        {
            if (_characterDisplay == null) return;

            _characterDisplay.OnCharacterClicked += OnCharacterClicked;
            _characterDisplay.OnCharacterChanged += OnCharacterChanged;

            // 보유 캐릭터 목록으로 초기화
            var ownedCharacters = DataManager.Instance?.OwnedCharacters;
            if (ownedCharacters != null && ownedCharacters.Count > 0)
            {
                var characterIds = ownedCharacters.Select(c => c.CharacterId).ToArray();
                _characterDisplay.Initialize(characterIds);
            }
        }

        private void InitializeInGameDashboard()
        {
            if (_stageShortcutButton != null)
            {
                _stageShortcutButton.onClick.AddListener(OnStageShortcutClicked);
            }

            if (_adventureButton != null)
            {
                _adventureButton.onClick.AddListener(OnAdventureClicked);
            }
        }

        private void InitializeBottomNav()
        {
            if (_contentNavButtons == null) return;

            foreach (var button in _contentNavButtons)
            {
                if (button != null)
                {
                    button.OnClicked += OnContentNavClicked;
                }
            }
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

        private void CleanupEventHandlers()
        {
            if (_eventBannerCarousel != null)
                _eventBannerCarousel.OnBannerClicked -= OnBannerClicked;

            if (_passButtons != null)
            {
                foreach (var button in _passButtons)
                {
                    if (button != null)
                        button.OnClicked -= OnPassButtonClicked;
                }
            }

            if (_quickMenuButtons != null)
            {
                foreach (var button in _quickMenuButtons)
                {
                    if (button != null)
                        button.OnClicked -= OnQuickMenuClicked;
                }
            }

            if (_characterDisplay != null)
            {
                _characterDisplay.OnCharacterClicked -= OnCharacterClicked;
                _characterDisplay.OnCharacterChanged -= OnCharacterChanged;
            }

            if (_stageShortcutButton != null)
                _stageShortcutButton.onClick.RemoveListener(OnStageShortcutClicked);

            if (_adventureButton != null)
                _adventureButton.onClick.RemoveListener(OnAdventureClicked);

            if (_contentNavButtons != null)
            {
                foreach (var button in _contentNavButtons)
                {
                    if (button != null)
                        button.OnClicked -= OnContentNavClicked;
                }
            }
        }

        #endregion

        #region Event Handlers

        private void OnBannerClicked(int index)
        {
            Debug.Log($"[LobbyScreen] Banner clicked: {index}");
            // TODO[P2]: 배너에 연결된 화면으로 이동
        }

        private void OnPassButtonClicked(string passType)
        {
            Debug.Log($"[LobbyScreen] Pass button clicked: {passType}");
            // TODO[P2]: 패스 상세 화면으로 이동
        }

        private void OnQuickMenuClicked(string targetScreen)
        {
            Debug.Log($"[LobbyScreen] QuickMenu clicked: {targetScreen}");
            NavigateTo(targetScreen);
        }

        private void OnCharacterClicked(string characterId)
        {
            Debug.Log($"[LobbyScreen] Character clicked: {characterId}");
            // TODO[P2]: 캐릭터 상호작용 또는 상세 화면
        }

        private void OnCharacterChanged(int index)
        {
            Debug.Log($"[LobbyScreen] Character changed: {index}");
            _currentCharacterIndex = index;
        }

        private void OnStageShortcutClicked()
        {
            Debug.Log("[LobbyScreen] Stage shortcut clicked");
            NavigateTo("StageSelectScreen");
        }

        private void OnAdventureClicked()
        {
            Debug.Log("[LobbyScreen] Adventure clicked");
            NavigateTo("StageSelectScreen");
        }

        private void OnContentNavClicked(string targetScreen)
        {
            Debug.Log($"[LobbyScreen] ContentNav clicked: {targetScreen}");
            NavigateTo(targetScreen);
        }

        private void OnUserDataChanged()
        {
            RefreshUI();
            BadgeManager.Instance?.RefreshAll();
        }

        private void OnBadgeChanged(BadgeType type, int count)
        {
            RefreshBadgeForType(type, count);
        }

        #endregion

        #region Navigation

        private void NavigateTo(string screenName)
        {
            if (string.IsNullOrEmpty(screenName))
            {
                Debug.LogWarning("[LobbyScreen] NavigateTo: screenName is empty");
                return;
            }

            Debug.Log($"[LobbyScreen] Navigate to: {screenName}");

            switch (screenName)
            {
                case "GachaScreen":
                    GachaScreen.Open();
                    break;
                case "GachaHistoryScreen":
                    GachaHistoryScreen.Open();
                    break;
                case "ShopScreen":
                    ShopScreen.Open();
                    break;
                case "CharacterListScreen":
                    CharacterListScreen.Open();
                    break;
                case "CharacterDetailScreen":
                    // CharacterDetailScreen은 캐릭터 ID가 필요하므로 기본값 사용
                    CharacterDetailScreen.Open(new CharacterDetailState());
                    break;
                case "StageSelectScreen":
                    StageSelectScreen.Open();
                    break;
                case "PartySelectScreen":
                    PartySelectScreen.Open();
                    break;
                case "LiveEventScreen":
                    LiveEventScreen.Open();
                    break;
                case "EventDetailScreen":
                    EventDetailScreen.Open(new EventDetailState());
                    break;
                default:
                    Debug.LogWarning($"[LobbyScreen] Unknown screen: {screenName}");
                    break;
            }
        }

        #endregion

        #region UI Refresh

        private void RefreshUI()
        {
            RefreshStageProgress();
            RefreshCharacterDisplay();
            RefreshInGameDashboard();
        }

        private void RefreshStageProgress()
        {
            if (_stageProgressWidget == null) return;

            // DataManager에서 현재 스테이지 진행 정보 가져오기
            var stageProgress = DataManager.Instance?.StageProgress;
            if (stageProgress == null)
            {
                _stageProgressWidget.SetProgress(1, 1, "시작의 길");
                return;
            }

            var chapter = stageProgress.Value.CurrentChapter;
            var stage = stageProgress.Value.CurrentStageNumber;

            // 스테이지 이름 가져오기 (StageDatabase에서 조회)
            var stageName = GetCurrentStageName(chapter, stage);
            _stageProgressWidget.SetProgress(chapter, stage, stageName);
        }


        private string GetCurrentStageName(int chapter, int stage)
        {
            // StageDatabase에서 현재 스테이지 정보 조회
            var stages = DataManager.Instance?.Stages;
            if (stages == null) return $"챕터 {chapter} - {stage}";

            // stageId 형식: "stage_{chapter}_{stage}" 또는 유사한 패턴
            var stageId = $"stage_{chapter}_{stage}";
            var stageData = stages.GetById(stageId);

            return stageData?.Name ?? $"챕터 {chapter} - {stage}";
        }

        private void RefreshCharacterDisplay()
        {
            if (_characterDisplay == null) return;

            // DataManager에서 보유 캐릭터 정보 가져와서 표시
            var ownedCharacters = DataManager.Instance?.OwnedCharacters;
            if (ownedCharacters != null && ownedCharacters.Count > 0)
            {
                var characterIds = ownedCharacters.Select(c => c.CharacterId).ToArray();
                _characterDisplay.Initialize(characterIds);
            }
        }

        private void RefreshInGameDashboard()
        {
            if (_stageShortcutLabel == null) return;

            // TODO[P2]: DataManager에서 현재 스테이지 정보 가져오기
            _stageShortcutLabel.text = "11-1 바로 가자!";
        }

        private void RefreshAllBadges()
        {
            if (BadgeManager.Instance == null) return;

            // ContentNavButton 배지 갱신
            RefreshBadgeForType(BadgeType.Gacha, BadgeManager.Instance.GetBadge(BadgeType.Gacha));
            RefreshBadgeForType(BadgeType.Shop, BadgeManager.Instance.GetBadge(BadgeType.Shop));
            RefreshBadgeForType(BadgeType.Character, BadgeManager.Instance.GetBadge(BadgeType.Character));
            RefreshBadgeForType(BadgeType.Event, BadgeManager.Instance.GetBadge(BadgeType.Event));
        }

        private void RefreshBadgeForType(BadgeType type, int count)
        {
            if (_contentNavButtons == null) return;

            string targetScreen = type switch
            {
                BadgeType.Gacha => "GachaScreen",
                BadgeType.Shop => "ShopScreen",
                BadgeType.Character => "CharacterListScreen",
                BadgeType.Event => "LiveEventScreen",
                _ => null
            };

            if (string.IsNullOrEmpty(targetScreen)) return;

            foreach (var button in _contentNavButtons)
            {
                if (button != null && button.TargetScreen == targetScreen)
                {
                    button.SetBadge(count);
                    break;
                }
            }
        }

        #endregion

        #region Lobby Entry Tasks

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

        #endregion
    }
}