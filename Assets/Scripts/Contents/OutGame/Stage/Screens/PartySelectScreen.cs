using System.Collections.Generic;
using System.Linq;
using Sc.Common.UI;
using Sc.Common.UI.Attributes;
using Sc.Common.UI.Widgets;
using Sc.Contents.Stage.Widgets;
using Sc.Core;
using Sc.Data;
using Sc.Event.OutGame;
using Sc.Event.UI;
using Sc.Foundation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sc.Contents.Stage
{
    /// <summary>
    /// 파티 선택 화면.
    /// 스테이지 진입 전 파티를 편성합니다.
    /// 레퍼런스: Docs/Design/Reference/PartySelect.jpg
    /// </summary>
    [ScreenTemplate(ScreenTemplateType.Standard)]
    public class PartySelectScreen : ScreenWidget<PartySelectScreen, PartySelectScreen.PartySelectState>
    {
        /// <summary>
        /// 파티 선택 화면 상태
        /// </summary>
        public class PartySelectState : IScreenState
        {
            /// <summary>
            /// 진입할 스테이지 ID
            /// </summary>
            public string StageId { get; set; }

            /// <summary>
            /// 스테이지 데이터
            /// </summary>
            public StageData StageData { get; set; }

            /// <summary>
            /// 프리셋 그룹 ID (선택적)
            /// </summary>
            public string PresetGroupId { get; set; }
        }

        #region Widget References

        [Header("Widgets")] [SerializeField] private StageInfoWidget _stageInfoWidget;
        [SerializeField] private CharacterSelectWidget _characterSelectWidget;

        [Header("Party Slots (Formation)")] [SerializeField]
        private Transform _frontLineContainer;

        [SerializeField] private Transform _backLineContainer;
        [SerializeField] private PartySlotWidget[] _partySlots;

        [Header("Battle Preview")] [SerializeField]
        private Transform _battlePreviewArea;

        [SerializeField] private Transform _partyFormation;
        [SerializeField] private Transform _enemyFormation;

        #endregion

        #region Legacy Fields (호환성 유지)

        [Header("Stage Info")] [SerializeField]
        private TMP_Text _stageNameText;

        [SerializeField] private TMP_Text _stageDifficultyText;
        [SerializeField] private TMP_Text _recommendedPowerText;

        [Header("Party Slots")] [SerializeField]
        private Transform _partySlotContainer;

        [SerializeField] private TMP_Text _partyPowerText;

        [Header("Character List")] [SerializeField]
        private Transform _characterListContainer;

        [SerializeField] private TMP_Text _characterCountText;

        [Header("Quick Actions")] [SerializeField]
        private Button _autoFormButton;

        [SerializeField] private Button _clearAllButton;
        [SerializeField] private Button _stageInfoButton;
        [SerializeField] private Button _formationSettingButton;

        [Header("Footer")] [SerializeField] private TMP_Text _staminaCostText;
        [SerializeField] private Button _startButton;
        [SerializeField] private TMP_Text _startButtonText;
        [SerializeField] private Button _quickBattleButton;
        [SerializeField] private Button _autoToggleButton;
        [SerializeField] private TMP_Text _autoToggleText;

        [Header("Navigation")] [SerializeField]
        private Button _backButton;

        [SerializeField] private Button _homeButton;

        #endregion

        private PartySelectState _currentState;
        private readonly List<string> _selectedCharacterIds = new();
        private readonly List<PartySlotWidget> _partySlotWidgets = new();
        private PartySlotWidget _selectedSlot;
        private bool _isStarting;
        private bool _isAutoMode;

        protected override void OnInitialize()
        {
            Debug.Log("[PartySelectScreen] OnInitialize");

            SetupButtons();
            SetupWidgets();
            SetupPartySlots();
        }

        private void SetupButtons()
        {
            if (_backButton != null)
                _backButton.onClick.AddListener(OnBackClicked);

            if (_homeButton != null)
                _homeButton.onClick.AddListener(OnHomeClicked);

            if (_startButton != null)
                _startButton.onClick.AddListener(OnStartClicked);

            if (_autoFormButton != null)
                _autoFormButton.onClick.AddListener(OnAutoFormClicked);

            if (_clearAllButton != null)
                _clearAllButton.onClick.AddListener(OnClearAllClicked);

            if (_stageInfoButton != null)
                _stageInfoButton.onClick.AddListener(OnStageInfoClicked);

            if (_formationSettingButton != null)
                _formationSettingButton.onClick.AddListener(OnFormationSettingClicked);

            if (_quickBattleButton != null)
                _quickBattleButton.onClick.AddListener(OnQuickBattleClicked);

            if (_autoToggleButton != null)
                _autoToggleButton.onClick.AddListener(OnAutoToggleClicked);
        }

        private void SetupWidgets()
        {
            // CharacterSelectWidget 이벤트 연결
            if (_characterSelectWidget != null)
            {
                _characterSelectWidget.OnCharacterSelected += OnCharacterSelected;
                _characterSelectWidget.OnCharacterDetailRequested += OnCharacterDetailRequested;
            }
        }

        private void SetupPartySlots()
        {
            _partySlotWidgets.Clear();

            // 배열로 지정된 슬롯 사용
            if (_partySlots != null && _partySlots.Length > 0)
            {
                for (int i = 0; i < _partySlots.Length; i++)
                {
                    var slot = _partySlots[i];
                    if (slot != null)
                    {
                        bool isFrontLine = i < 3; // 0,1,2 = 앞줄, 3,4,5 = 뒷줄
                        slot.Initialize(i, isFrontLine);
                        slot.OnSlotClicked += OnPartySlotClicked;
                        slot.OnRemoveRequested += OnPartySlotRemoveRequested;
                        _partySlotWidgets.Add(slot);
                    }
                }
            }
        }

        protected override void OnBind(PartySelectState state)
        {
            _currentState = state ?? new PartySelectState();

            Debug.Log($"[PartySelectScreen] OnBind - StageId: {_currentState.StageId}");

            // Header 설정
            ScreenHeader.Instance?.Configure("party_select");

            // 스테이지 정보 표시
            RefreshStageInfo();

            // StageInfoWidget 구성
            if (_stageInfoWidget != null && _currentState.StageData != null)
            {
                _stageInfoWidget.Configure(_currentState.StageData);
            }

            // 파티 슬롯 초기화
            InitializePartySlots();

            // 캐릭터 목록 로드
            LoadCharacterList();

            // CharacterSelectWidget 초기화
            if (_characterSelectWidget != null)
            {
                var ownedCharacters = DataManager.Instance?.OwnedCharacters;
                var characters = ownedCharacters != null ? ownedCharacters.ToList() : new List<OwnedCharacter>();
                _characterSelectWidget.Initialize(characters);
            }

            // 푸터 갱신
            RefreshFooter();
        }

        protected override void OnShow()
        {
            Debug.Log("[PartySelectScreen] OnShow");

            // Header Back 이벤트 구독
            EventManager.Instance?.Subscribe<HeaderBackClickedEvent>(OnHeaderBackClicked);

            // Stage 이벤트 구독
            EventManager.Instance?.Subscribe<StageEnteredEvent>(OnStageEntered);
            EventManager.Instance?.Subscribe<StageEntryFailedEvent>(OnStageEntryFailed);
        }

        protected override void OnHide()
        {
            Debug.Log("[PartySelectScreen] OnHide");

            // Header Back 이벤트 해제
            EventManager.Instance?.Unsubscribe<HeaderBackClickedEvent>(OnHeaderBackClicked);

            // Stage 이벤트 해제
            EventManager.Instance?.Unsubscribe<StageEnteredEvent>(OnStageEntered);
            EventManager.Instance?.Unsubscribe<StageEntryFailedEvent>(OnStageEntryFailed);
        }

        public override PartySelectState GetState() => _currentState;

        #region Stage Info

        private void RefreshStageInfo()
        {
            var stage = _currentState.StageData;
            if (stage == null) return;

            if (_stageNameText != null)
            {
                _stageNameText.text = stage.Name;
            }

            if (_stageDifficultyText != null)
            {
                _stageDifficultyText.text = GetDifficultyText(stage.Difficulty);
            }

            if (_recommendedPowerText != null)
            {
                _recommendedPowerText.text = $"권장 전투력: {stage.RecommendedPower:N0}";
            }
        }

        #endregion

        #region Party Slots

        private void InitializePartySlots()
        {
            // TODO[P1]: 파티 슬롯 UI 초기화
            // 현재는 플레이스홀더
            _selectedCharacterIds.Clear();

            RefreshPartyPower();
        }

        private void RefreshPartyPower()
        {
            // TODO[P1]: 선택된 캐릭터들의 전투력 합산
            int totalPower = 0;

            if (_partyPowerText != null)
            {
                _partyPowerText.text = $"전투력: {totalPower:N0}";
            }
        }

        #endregion

        #region Character List

        private void LoadCharacterList()
        {
            // 보유 캐릭터 목록 로드
            var ownedCharacters = DataManager.Instance?.OwnedCharacters;
            var characterList = ownedCharacters?.ToList() ?? new List<OwnedCharacter>();

            // CharacterSelectWidget에 캐릭터 목록 전달
            if (_characterSelectWidget != null)
            {
                _characterSelectWidget.Initialize(characterList);
            }

            if (_characterCountText != null)
            {
                _characterCountText.text = $"캐릭터: {characterList.Count}";
            }
        }

        #endregion

        #region Footer

        private void RefreshFooter()
        {
            var stage = _currentState.StageData;
            if (stage == null) return;

            if (_staminaCostText != null)
            {
                int currentStamina = DataManager.Instance?.Currency.Stamina ?? 0;
                _staminaCostText.text = $"스태미나: {currentStamina} / {stage.StaminaCost}";
            }

            if (_startButtonText != null)
            {
                _startButtonText.text = "전투 시작";
            }

            // 시작 버튼 활성화 조건
            // - 최소 1명 선택
            // - 스태미나 충분
            // - 진행 중 아님
            bool canStart = !_isStarting;
            // TODO[P1]: && _selectedCharacterIds.Count > 0
            // TODO[P1]: && HasEnoughStamina()

            if (_startButton != null)
            {
                _startButton.interactable = canStart;
            }
        }

        #endregion

        #region Battle Start

        private void OnStartClicked()
        {
            if (_isStarting) return;

            Debug.Log($"[PartySelectScreen] Start clicked - StageId: {_currentState.StageId}");

            // TODO[P1]: 파티 선택 검증
            if (_selectedCharacterIds.Count == 0)
            {
                // 임시: 기본 파티로 시작 (첫 번째 캐릭터)
                var characters = DataManager.Instance.OwnedCharacters;
                if (characters != null && characters.Count > 0)
                {
                    _selectedCharacterIds.Add(characters[0].InstanceId);
                }
            }

            if (_selectedCharacterIds.Count == 0)
            {
                ShowNoCharacterPopup();
                return;
            }

            // 스테이지 입장 요청
            ExecuteEnterStage();
        }

        private void ExecuteEnterStage()
        {
            _isStarting = true;
            RefreshFooter();

            var request = EnterStageRequest.Create(
                _currentState.StageId,
                _selectedCharacterIds
            );

            Debug.Log($"[PartySelectScreen] Sending EnterStageRequest: {_currentState.StageId}");
            NetworkManager.Instance?.Send(request);
        }

        private void ShowNoCharacterPopup()
        {
            ConfirmPopup.Open(new ConfirmState
            {
                Title = "파티 편성",
                Message = "파티에 최소 1명의 캐릭터가 필요합니다.",
                ShowCancelButton = false
            });
        }

        #endregion

        #region Event Handlers

        private void OnStageEntered(StageEnteredEvent evt)
        {
            if (evt.StageId != _currentState.StageId) return;

            Debug.Log($"[PartySelectScreen] Stage entered successfully: {evt.BattleSessionId}");
            _isStarting = false;

            // TODO[FUTURE]: BattleScreen으로 전환 (InGame 시스템)
            // BattleReadyEvent 발행하여 Battle 시스템에 알림
            EventManager.Instance?.Publish(new BattleReadyEvent
            {
                BattleSessionId = evt.BattleSessionId,
                StageId = evt.StageId,
                PartyCharacterIds = _selectedCharacterIds
            });

            // 임시: 전투 완료 시뮬레이션 (실제로는 Battle 시스템에서 처리)
            Debug.Log("[PartySelectScreen] Battle simulation - navigating back");
            NavigationManager.Instance?.Back();
        }

        private void OnStageEntryFailed(StageEntryFailedEvent evt)
        {
            if (evt.StageId != _currentState.StageId) return;

            Debug.LogWarning($"[PartySelectScreen] Stage entry failed: {evt.ErrorCode} - {evt.ErrorMessage}");
            _isStarting = false;
            RefreshFooter();

            ConfirmPopup.Open(new ConfirmState
            {
                Title = "입장 실패",
                Message = evt.ErrorMessage,
                ShowCancelButton = false
            });
        }

        #endregion

        #region Navigation

        private void OnBackClicked()
        {
            Debug.Log("[PartySelectScreen] Back clicked");
            NavigationManager.Instance?.Back();
        }

        private void OnHomeClicked()
        {
            Debug.Log("[PartySelectScreen] Home clicked");
            // TODO[P2]: 문자열 기반 네비게이션 구현 필요
            // 현재는 순환 참조 방지를 위해 스택 팝으로 대체
            while (NavigationManager.Instance?.ScreenCount > 1)
            {
                NavigationManager.Instance?.Back();
            }
        }

        private void OnHeaderBackClicked(HeaderBackClickedEvent evt)
        {
            OnBackClicked();
        }

        #endregion

        #region Party Slot Handlers

        private void OnPartySlotClicked(PartySlotWidget slot, int slotIndex)
        {
            Debug.Log($"[PartySelectScreen] Party slot clicked: {slotIndex}");

            // 이전 선택 해제
            _selectedSlot?.SetSelected(false);

            // 새 슬롯 선택
            _selectedSlot = slot;
            _selectedSlot.SetSelected(true);
        }

        private void OnPartySlotRemoveRequested(PartySlotWidget slot, OwnedCharacter character)
        {
            Debug.Log($"[PartySelectScreen] Remove character from slot: {character.InstanceId}");

            if (string.IsNullOrEmpty(character.InstanceId)) return;

            // 슬롯에서 캐릭터 제거
            slot.ClearCharacter();
            _selectedCharacterIds.Remove(character.InstanceId);

            // CharacterSelectWidget 업데이트
            _characterSelectWidget?.RemoveAssignedCharacter(character.InstanceId);

            // UI 갱신
            RefreshPartyPower();
            RefreshFooter();
            UpdateStageInfoWidget();
        }

        #endregion

        #region Character Selection Handlers

        private void OnCharacterSelected(OwnedCharacter character)
        {
            Debug.Log($"[PartySelectScreen] Character selected: {character.InstanceId}");

            if (string.IsNullOrEmpty(character.InstanceId)) return;

            // 이미 편성된 캐릭터인 경우
            if (_selectedCharacterIds.Contains(character.InstanceId))
            {
                // 해당 슬롯에서 제거
                RemoveCharacterFromParty(character.InstanceId);
            }
            else
            {
                // 선택된 슬롯이 있으면 해당 슬롯에 할당
                if (_selectedSlot != null && _selectedSlot.CurrentState != PartySlotWidget.SlotState.Locked)
                {
                    AssignCharacterToSlot(_selectedSlot, character);
                }
                else
                {
                    // 빈 슬롯 찾아서 할당
                    var emptySlot = _partySlotWidgets.FirstOrDefault(s =>
                        s.CurrentState == PartySlotWidget.SlotState.Empty);

                    if (emptySlot != null)
                    {
                        AssignCharacterToSlot(emptySlot, character);
                    }
                    else
                    {
                        Debug.Log("[PartySelectScreen] No empty slot available");
                    }
                }
            }
        }

        private void OnCharacterDetailRequested(OwnedCharacter character)
        {
            Debug.Log($"[PartySelectScreen] Character detail requested: {character.InstanceId}");
            // TODO[P2]: CharacterDetailPopup 열기
        }

        private void AssignCharacterToSlot(PartySlotWidget slot, OwnedCharacter character)
        {
            // 기존 캐릭터가 있으면 제거
            if (!string.IsNullOrEmpty(slot.AssignedCharacter.InstanceId))
            {
                _selectedCharacterIds.Remove(slot.AssignedCharacter.InstanceId);
                _characterSelectWidget?.RemoveAssignedCharacter(slot.AssignedCharacter.InstanceId);
            }

            // 새 캐릭터 할당
            slot.AssignCharacter(character);
            _selectedCharacterIds.Add(character.InstanceId);
            _characterSelectWidget?.AddAssignedCharacter(character.InstanceId);

            // UI 갱신
            RefreshPartyPower();
            RefreshFooter();
            UpdateStageInfoWidget();
        }

        private void RemoveCharacterFromParty(string characterId)
        {
            var slot = _partySlotWidgets.FirstOrDefault(s =>
                s.AssignedCharacter.InstanceId == characterId);

            if (slot != null)
            {
                slot.ClearCharacter();
                _selectedCharacterIds.Remove(characterId);
                _characterSelectWidget?.RemoveAssignedCharacter(characterId);

                RefreshPartyPower();
                RefreshFooter();
                UpdateStageInfoWidget();
            }
        }

        #endregion

        #region Quick Action Handlers

        private void OnAutoFormClicked()
        {
            Debug.Log("[PartySelectScreen] Auto form clicked");
            // TODO[P2]: 자동 편성 로직
            // 전투력 순으로 최적의 캐릭터 자동 선택
        }

        private void OnClearAllClicked()
        {
            Debug.Log("[PartySelectScreen] Clear all clicked");

            foreach (var slot in _partySlotWidgets)
            {
                if (!string.IsNullOrEmpty(slot.AssignedCharacter.InstanceId))
                {
                    slot.ClearCharacter();
                }
            }

            _selectedCharacterIds.Clear();
            _characterSelectWidget?.SetAssignedCharacters(null);

            RefreshPartyPower();
            RefreshFooter();
            UpdateStageInfoWidget();
        }

        private void OnStageInfoClicked()
        {
            Debug.Log("[PartySelectScreen] Stage info clicked");
            // 이미 구현됨 - StageInfoPopup.Open 호출
            if (_currentState?.StageData != null)
            {
                StageInfoPopup.Open(new StageInfoState
                {
                    StageData = _currentState.StageData
                });
            }
        }

        private void OnFormationSettingClicked()
        {
            Debug.Log("[PartySelectScreen] Formation setting clicked");
            // TODO[P2]: PresetManagePopup 열기
        }

        private void OnQuickBattleClicked()
        {
            Debug.Log("[PartySelectScreen] Quick battle clicked");
            // TODO[FUTURE]: 빠른 전투 (소탕) 로직 (InGame 시스템)
        }

        private void OnAutoToggleClicked()
        {
            _isAutoMode = !_isAutoMode;

            if (_autoToggleText != null)
            {
                _autoToggleText.text = _isAutoMode ? "자동 ON" : "자동 OFF";
            }

            Debug.Log($"[PartySelectScreen] Auto mode: {_isAutoMode}");
        }

        #endregion

        #region UI Update Helpers

        private void UpdateStageInfoWidget()
        {
            if (_stageInfoWidget == null) return;

            int totalPower = CalculateTotalPower();
            _stageInfoWidget.UpdatePartyStatus(_selectedCharacterIds.Count, totalPower);
        }

        private int CalculateTotalPower()
        {
            int totalPower = 0;

            foreach (var slot in _partySlotWidgets)
            {
                if (!string.IsNullOrEmpty(slot.AssignedCharacter.InstanceId))
                {
                    // TODO[P1]: 실제 전투력 계산 로직 구현
                    // 현재는 레벨 * 100으로 임시 계산
                    totalPower += slot.AssignedCharacter.Level * 100;
                }
            }

            return totalPower;
        }

        #endregion

        #region Helpers

        private string GetDifficultyText(Difficulty difficulty)
        {
            return difficulty switch
            {
                Difficulty.Easy => "Easy",
                Difficulty.Normal => "Normal",
                Difficulty.Hard => "Hard",
                _ => difficulty.ToString()
            };
        }

        #endregion

        protected override void OnRelease()
        {
            CleanupButtons();
            CleanupWidgets();
            CleanupPartySlots();

            _selectedCharacterIds.Clear();
            _partySlotWidgets.Clear();
            _selectedSlot = null;
        }

        private void CleanupButtons()
        {
            if (_backButton != null)
                _backButton.onClick.RemoveListener(OnBackClicked);

            if (_homeButton != null)
                _homeButton.onClick.RemoveListener(OnHomeClicked);

            if (_startButton != null)
                _startButton.onClick.RemoveListener(OnStartClicked);

            if (_autoFormButton != null)
                _autoFormButton.onClick.RemoveListener(OnAutoFormClicked);

            if (_clearAllButton != null)
                _clearAllButton.onClick.RemoveListener(OnClearAllClicked);

            if (_stageInfoButton != null)
                _stageInfoButton.onClick.RemoveListener(OnStageInfoClicked);

            if (_formationSettingButton != null)
                _formationSettingButton.onClick.RemoveListener(OnFormationSettingClicked);

            if (_quickBattleButton != null)
                _quickBattleButton.onClick.RemoveListener(OnQuickBattleClicked);

            if (_autoToggleButton != null)
                _autoToggleButton.onClick.RemoveListener(OnAutoToggleClicked);
        }

        private void CleanupWidgets()
        {
            if (_characterSelectWidget != null)
            {
                _characterSelectWidget.OnCharacterSelected -= OnCharacterSelected;
                _characterSelectWidget.OnCharacterDetailRequested -= OnCharacterDetailRequested;
            }
        }

        private void CleanupPartySlots()
        {
            foreach (var slot in _partySlotWidgets)
            {
                if (slot != null)
                {
                    slot.OnSlotClicked -= OnPartySlotClicked;
                    slot.OnRemoveRequested -= OnPartySlotRemoveRequested;
                }
            }
        }
    }
}