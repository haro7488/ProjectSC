using System.Collections.Generic;
using Sc.Common.UI;
using Sc.Core;
using Sc.Data;
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
    }

    /// <summary>
    /// 캐릭터 목록 화면 - 보유 캐릭터 리스트
    /// </summary>
    public class CharacterListScreen : ScreenWidget<CharacterListScreen, CharacterListState>
    {
        [Header("UI References")]
        [SerializeField] private Transform _listContainer;
        [SerializeField] private GameObject _characterItemPrefab;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private Button _backButton;
        [SerializeField] private TMP_Text _countText;

        private CharacterListState _currentState;
        private readonly List<GameObject> _spawnedItems = new();

        protected override void OnInitialize()
        {
            Debug.Log("[CharacterListScreen] OnInitialize");

            if (_backButton != null)
            {
                _backButton.onClick.AddListener(OnBackClicked);
            }
        }

        protected override void OnBind(CharacterListState state)
        {
            _currentState = state ?? new CharacterListState();
            Debug.Log("[CharacterListScreen] OnBind");

            RefreshList();
        }

        protected override void OnShow()
        {
            Debug.Log("[CharacterListScreen] OnShow");

            if (DataManager.Instance != null)
            {
                DataManager.Instance.OnUserDataChanged += OnUserDataChanged;
            }

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
                Rarity.SSR => new Color(1f, 0.84f, 0f, 0.3f),      // 금색
                Rarity.SR => new Color(0.5f, 0f, 0.5f, 0.3f),      // 보라색
                Rarity.R => new Color(0f, 0.5f, 1f, 0.3f),         // 파란색
                _ => new Color(0.5f, 0.5f, 0.5f, 0.3f)             // 회색
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

            // TODO: 캐릭터 상세 화면으로 이동
            // CharacterDetailScreen.Open(new CharacterDetailState { CharacterId = characterId });
        }

        private void OnBackClicked()
        {
            Debug.Log("[CharacterListScreen] Back clicked");
            NavigationManager.Instance?.Back();
        }
    }
}
