using System.Collections.Generic;
using Sc.Common.UI;
using Sc.Common.UI.Attributes;
using Sc.Common.UI.Widgets;
using Sc.Contents.Character.Widgets;
using Sc.Core;
using Sc.Data;
using Sc.Event.UI;
using Sc.Foundation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sc.Contents.Character
{
    /// <summary>
    /// 캐릭터 목록 화면 State
    /// </summary>
    public class CharacterListState : IScreenState
    {
        /// <summary>
        /// 현재 스크롤 위치
        /// </summary>
        public float ScrollPosition;

        /// <summary>
        /// 선택된 캐릭터 ID
        /// </summary>
        public string SelectedCharacterId;

        /// <summary>
        /// 현재 탭 (0: 전체, 1: 즐겨찾기)
        /// </summary>
        public int CurrentTab;
    }

    /// <summary>
    /// 캐릭터 목록 화면 - 보유 캐릭터 리스트
    /// 스펙: Docs/Specs/Character.md
    /// </summary>
    [ScreenTemplate(ScreenTemplateType.Standard)]
    public class CharacterListScreen : ScreenWidget<CharacterListScreen, CharacterListState>
    {
        #region SerializeFields

        [Header("Tab Area")] [SerializeField] private Button _allCharactersTab;
        [SerializeField] private TMP_Text _allCharactersTabText;
        [SerializeField] private Button _favoritesTab;
        [SerializeField] private TMP_Text _favoritesTabText;

        [Header("Filter Area")] [SerializeField]
        private CharacterFilterWidget _filterWidget;

        [Header("Character Grid")] [SerializeField]
        private Transform _characterGridContainer;

        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private GameObject _characterCardPrefab;

        [Header("Legacy - To Remove")] [SerializeField]
        private Transform _listContainer;

        [SerializeField] private GameObject _characterItemPrefab;
        [SerializeField] private Button _backButton;
        [SerializeField] private TMP_Text _countText;

        #endregion

        #region Private Fields

        private CharacterListState _currentState;
        private readonly List<GameObject> _spawnedItems = new();
        private readonly List<CharacterCard> _spawnedCards = new();

        #endregion

        #region Lifecycle

        protected override void OnInitialize()
        {
            Debug.Log("[CharacterListScreen] OnInitialize");

            InitializeTabs();
            InitializeFilter();
            InitializeLegacy();
        }

        private void InitializeTabs()
        {
            if (_allCharactersTab != null)
            {
                _allCharactersTab.onClick.AddListener(() => OnTabSelected(0));
            }

            if (_favoritesTab != null)
            {
                _favoritesTab.onClick.AddListener(() => OnTabSelected(1));
            }
        }

        private void InitializeFilter()
        {
            if (_filterWidget != null)
            {
                _filterWidget.OnFilterToggled += OnFilterToggled;
                _filterWidget.OnSortTypeChanged += OnSortTypeChanged;
                _filterWidget.OnSortOrderChanged += OnSortOrderChanged;
            }
        }

        private void InitializeLegacy()
        {
            // 기존 Back 버튼 (ScreenHeader로 대체 예정)
            if (_backButton != null)
            {
                _backButton.onClick.AddListener(OnBackClicked);
            }
        }

        #endregion

        #region Tab Handlers

        private void OnTabSelected(int tabIndex)
        {
            if (_currentState != null)
            {
                _currentState.CurrentTab = tabIndex;
            }

            UpdateTabUI();
            RefreshList();
        }

        private void UpdateTabUI()
        {
            var isAllTab = _currentState?.CurrentTab == 0;

            // 탭 버튼 시각적 상태 업데이트
            if (_allCharactersTab != null)
            {
                _allCharactersTab.interactable = !isAllTab;
            }

            if (_favoritesTab != null)
            {
                _favoritesTab.interactable = isAllTab;
            }
        }

        #endregion

        #region Filter Handlers

        private void OnFilterToggled(bool isOn)
        {
            Debug.Log($"[CharacterListScreen] Filter toggled: {isOn}");
            RefreshList();
        }

        private void OnSortTypeChanged(SortType sortType)
        {
            Debug.Log($"[CharacterListScreen] Sort type changed: {sortType}");
            RefreshList();
        }

        private void OnSortOrderChanged(bool isAscending)
        {
            Debug.Log($"[CharacterListScreen] Sort order changed: {(isAscending ? "Ascending" : "Descending")}");
            RefreshList();
        }

        #endregion

        protected override void OnBind(CharacterListState state)
        {
            _currentState = state ?? new CharacterListState();
            Debug.Log("[CharacterListScreen] OnBind");

            // Header 설정
            ScreenHeader.Instance?.Configure("character_list");

            RefreshList();
        }

        protected override void OnShow()
        {
            Debug.Log("[CharacterListScreen] OnShow");

            if (DataManager.Instance != null)
            {
                DataManager.Instance.OnUserDataChanged += OnUserDataChanged;
            }

            // Header Back 이벤트 구독
            EventManager.Instance?.Subscribe<HeaderBackClickedEvent>(OnHeaderBackClicked);

            RefreshList();

            // 스크롤 위치 복원
            if (_scrollRect != null && _currentState != null)
            {
                _scrollRect.verticalNormalizedPosition = _currentState.ScrollPosition;
            }
        }

        protected override void OnHide()
        {
            Debug.Log("[CharacterListScreen] OnHide");

            if (DataManager.Instance != null)
            {
                DataManager.Instance.OnUserDataChanged -= OnUserDataChanged;
            }

            // Header Back 이벤트 해제
            EventManager.Instance?.Unsubscribe<HeaderBackClickedEvent>(OnHeaderBackClicked);

            // 스크롤 위치 저장
            if (_scrollRect != null && _currentState != null)
            {
                _currentState.ScrollPosition = _scrollRect.verticalNormalizedPosition;
            }
        }

        protected override void OnRelease()
        {
            ClearList();
        }

        public override CharacterListState GetState() => _currentState;

        private void RefreshList()
        {
            ClearList();

            if (DataManager.Instance?.IsInitialized != true)
                return;

            var ownedCharacters = DataManager.Instance.OwnedCharacters;

            // 카운트 텍스트
            if (_countText != null)
            {
                _countText.text = $"보유 캐릭터: {ownedCharacters.Count}";
            }

            // 캐릭터 아이템 생성
            if (_listContainer != null && _characterItemPrefab != null)
            {
                foreach (var owned in ownedCharacters)
                {
                    var itemGo = Instantiate(_characterItemPrefab, _listContainer);
                    SetupCharacterItem(itemGo, owned);
                    _spawnedItems.Add(itemGo);
                }
            }
        }

        private void SetupCharacterItem(GameObject itemGo, OwnedCharacter owned)
        {
            // 마스터 데이터 조회
            var masterData = DataManager.Instance?.GetCharacterMasterData(owned);

            // 이름 텍스트
            var nameText = itemGo.GetComponentInChildren<TMP_Text>();
            if (nameText != null)
            {
                var displayName = masterData?.Name ?? owned.CharacterId;
                var rarityText = masterData != null ? GetRarityText(masterData.Rarity) : "?";
                nameText.text = $"[{rarityText}] {displayName} Lv.{owned.Level}";
            }

            // 배경색 (희귀도에 따라)
            var image = itemGo.GetComponent<Image>();
            if (image != null && masterData != null)
            {
                image.color = GetRarityColor(masterData.Rarity);
            }

            // 버튼 클릭 이벤트
            var button = itemGo.GetComponent<Button>();
            if (button != null)
            {
                var characterId = owned.CharacterId;
                button.onClick.AddListener(() => OnCharacterItemClicked(characterId));
            }
        }

        private string GetRarityText(Rarity rarity)
        {
            return rarity switch
            {
                Rarity.SSR => "SSR",
                Rarity.SR => "SR",
                Rarity.R => "R",
                _ => "N"
            };
        }

        private Color GetRarityColor(Rarity rarity)
        {
            return rarity switch
            {
                Rarity.SSR => new Color(1f, 0.84f, 0f, 0.3f), // 금색
                Rarity.SR => new Color(0.5f, 0f, 0.5f, 0.3f), // 보라색
                Rarity.R => new Color(0f, 0.5f, 1f, 0.3f), // 파란색
                _ => new Color(0.5f, 0.5f, 0.5f, 0.3f) // 회색
            };
        }

        private void ClearList()
        {
            foreach (var item in _spawnedItems)
            {
                if (item != null)
                {
                    Destroy(item);
                }
            }

            _spawnedItems.Clear();
        }

        private void OnUserDataChanged()
        {
            RefreshList();
        }

        private void OnCharacterItemClicked(string characterId)
        {
            Debug.Log($"[CharacterListScreen] Character clicked: {characterId}");
            _currentState.SelectedCharacterId = characterId;

            // 캐릭터 상세 화면으로 이동
            CharacterDetailScreen.Open(new CharacterDetailState { CharacterId = characterId });
        }

        private void OnBackClicked()
        {
            Debug.Log("[CharacterListScreen] Back clicked");
            NavigationManager.Instance?.Back();
        }

        private void OnHeaderBackClicked(HeaderBackClickedEvent evt)
        {
            OnBackClicked();
        }
    }
}