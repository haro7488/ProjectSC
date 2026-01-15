using System.Linq;
using Sc.Common.UI;
using Sc.Core;
using Sc.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sc.Contents.Character
{
    /// <summary>
    /// 캐릭터 상세 화면 State
    /// </summary>
    public class CharacterDetailState : IScreenState
    {
        /// <summary>
        /// 표시할 캐릭터 인스턴스 ID
        /// </summary>
        public string InstanceId;

        /// <summary>
        /// 캐릭터 마스터 데이터 ID
        /// </summary>
        public string CharacterId;
    }

    /// <summary>
    /// 캐릭터 상세 화면 - 선택한 캐릭터의 상세 정보 표시
    /// </summary>
    public class CharacterDetailScreen : ScreenWidget<CharacterDetailScreen, CharacterDetailState>
    {
        [Header("UI References")]
        [SerializeField] private Button _backButton;
        [SerializeField] private TMP_Text _titleText;

        [Header("캐릭터 기본 정보")]
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _rarityText;
        [SerializeField] private TMP_Text _classText;
        [SerializeField] private TMP_Text _elementText;
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private Image _characterImage;

        [Header("스탯 정보")]
        [SerializeField] private TMP_Text _hpText;
        [SerializeField] private TMP_Text _atkText;
        [SerializeField] private TMP_Text _defText;
        [SerializeField] private TMP_Text _spdText;
        [SerializeField] private TMP_Text _critRateText;
        [SerializeField] private TMP_Text _critDamageText;

        [Header("추가 정보")]
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private TMP_Text _ascensionText;

        private CharacterDetailState _currentState;
        private OwnedCharacter? _ownedCharacter;
        private CharacterData _masterData;

        protected override void OnInitialize()
        {
            Debug.Log("[CharacterDetailScreen] OnInitialize");

            if (_backButton != null)
            {
                _backButton.onClick.AddListener(OnBackClicked);
            }
        }

        protected override void OnBind(CharacterDetailState state)
        {
            _currentState = state ?? new CharacterDetailState();
            Debug.Log($"[CharacterDetailScreen] OnBind - CharacterId: {_currentState.CharacterId}");

            LoadCharacterData();
            RefreshUI();
        }

        protected override void OnShow()
        {
            Debug.Log("[CharacterDetailScreen] OnShow");

            if (DataManager.Instance != null)
            {
                DataManager.Instance.OnUserDataChanged += OnUserDataChanged;
            }
        }

        protected override void OnHide()
        {
            Debug.Log("[CharacterDetailScreen] OnHide");

            if (DataManager.Instance != null)
            {
                DataManager.Instance.OnUserDataChanged -= OnUserDataChanged;
            }
        }

        protected override void OnRelease()
        {
            _ownedCharacter = null;
            _masterData = null;
        }

        public override CharacterDetailState GetState() => _currentState;

        private void LoadCharacterData()
        {
            if (DataManager.Instance?.IsInitialized != true)
                return;

            // InstanceId가 있으면 보유 캐릭터에서 찾기
            if (!string.IsNullOrEmpty(_currentState.InstanceId))
            {
                var owned = DataManager.Instance.OwnedCharacters
                    .FirstOrDefault(c => c.InstanceId == _currentState.InstanceId);

                if (!string.IsNullOrEmpty(owned.InstanceId))
                {
                    _ownedCharacter = owned;
                    _currentState.CharacterId = owned.CharacterId;
                }
            }
            // CharacterId로 찾기
            else if (!string.IsNullOrEmpty(_currentState.CharacterId))
            {
                var owned = DataManager.Instance.OwnedCharacters
                    .FirstOrDefault(c => c.CharacterId == _currentState.CharacterId);

                if (!string.IsNullOrEmpty(owned.InstanceId))
                {
                    _ownedCharacter = owned;
                }
            }

            // 마스터 데이터 로드
            if (!string.IsNullOrEmpty(_currentState.CharacterId))
            {
                _masterData = DataManager.Instance.Characters?.GetById(_currentState.CharacterId);
            }
        }

        private void RefreshUI()
        {
            if (_masterData == null)
            {
                Debug.LogWarning($"[CharacterDetailScreen] MasterData not found: {_currentState.CharacterId}");
                SetEmptyState();
                return;
            }

            // 타이틀
            if (_titleText != null)
            {
                _titleText.text = "캐릭터 상세";
            }

            // 기본 정보
            if (_nameText != null)
            {
                _nameText.text = _masterData.Name;
            }

            if (_rarityText != null)
            {
                _rarityText.text = GetRarityText(_masterData.Rarity);
                _rarityText.color = GetRarityColor(_masterData.Rarity);
            }

            if (_classText != null)
            {
                _classText.text = GetClassText(_masterData.CharacterClass);
            }

            if (_elementText != null)
            {
                _elementText.text = GetElementText(_masterData.Element);
                _elementText.color = GetElementColor(_masterData.Element);
            }

            // 레벨 (보유 캐릭터 데이터)
            if (_levelText != null)
            {
                var level = _ownedCharacter?.Level ?? 1;
                _levelText.text = $"Lv. {level}";
            }

            // 돌파 단계
            if (_ascensionText != null)
            {
                var ascension = _ownedCharacter?.Ascension ?? 0;
                _ascensionText.text = $"돌파 {ascension}단계";
            }

            // 스탯 정보
            if (_hpText != null)
            {
                _hpText.text = $"HP: {_masterData.BaseHp}";
            }

            if (_atkText != null)
            {
                _atkText.text = $"ATK: {_masterData.BaseAtk}";
            }

            if (_defText != null)
            {
                _defText.text = $"DEF: {_masterData.BaseDef}";
            }

            if (_spdText != null)
            {
                _spdText.text = $"SPD: {_masterData.BaseSpd}";
            }

            if (_critRateText != null)
            {
                _critRateText.text = $"치명률: {_masterData.CritRate:P1}";
            }

            if (_critDamageText != null)
            {
                _critDamageText.text = $"치명피해: {_masterData.CritDamage:P0}";
            }

            // 설명
            if (_descriptionText != null)
            {
                _descriptionText.text = _masterData.Description ?? "";
            }

            // 배경색 (희귀도)
            if (_characterImage != null)
            {
                _characterImage.color = GetRarityBackgroundColor(_masterData.Rarity);
            }
        }

        private void SetEmptyState()
        {
            if (_nameText != null) _nameText.text = "???";
            if (_rarityText != null) _rarityText.text = "?";
            if (_classText != null) _classText.text = "???";
            if (_elementText != null) _elementText.text = "???";
            if (_levelText != null) _levelText.text = "Lv. ?";
            if (_descriptionText != null) _descriptionText.text = "데이터를 찾을 수 없습니다.";
        }

        #region Helper Methods

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
                Rarity.SSR => new Color(1f, 0.84f, 0f),      // 금색
                Rarity.SR => new Color(0.7f, 0.3f, 0.9f),    // 보라색
                Rarity.R => new Color(0.3f, 0.6f, 1f),       // 파란색
                _ => Color.gray
            };
        }

        private Color GetRarityBackgroundColor(Rarity rarity)
        {
            return rarity switch
            {
                Rarity.SSR => new Color(1f, 0.84f, 0f, 0.2f),
                Rarity.SR => new Color(0.7f, 0.3f, 0.9f, 0.2f),
                Rarity.R => new Color(0.3f, 0.6f, 1f, 0.2f),
                _ => new Color(0.5f, 0.5f, 0.5f, 0.2f)
            };
        }

        private string GetClassText(CharacterClass characterClass)
        {
            return characterClass switch
            {
                CharacterClass.Warrior => "전사",
                CharacterClass.Mage => "마법사",
                CharacterClass.Archer => "궁수",
                CharacterClass.Healer => "힐러",
                CharacterClass.Tank => "탱커",
                CharacterClass.Assassin => "암살자",
                _ => "???"
            };
        }

        private string GetElementText(Element element)
        {
            return element switch
            {
                Element.Fire => "화",
                Element.Water => "수",
                Element.Wind => "풍",
                Element.Earth => "지",
                Element.Light => "광",
                Element.Dark => "암",
                _ => "무"
            };
        }

        private Color GetElementColor(Element element)
        {
            return element switch
            {
                Element.Fire => new Color(1f, 0.3f, 0.2f),
                Element.Water => new Color(0.2f, 0.5f, 1f),
                Element.Wind => new Color(0.3f, 0.9f, 0.5f),
                Element.Earth => new Color(0.6f, 0.4f, 0.2f),
                Element.Light => new Color(1f, 0.95f, 0.6f),
                Element.Dark => new Color(0.5f, 0.2f, 0.6f),
                _ => Color.white
            };
        }

        #endregion

        #region Event Handlers

        private void OnUserDataChanged()
        {
            LoadCharacterData();
            RefreshUI();
        }

        private void OnBackClicked()
        {
            Debug.Log("[CharacterDetailScreen] Back clicked");
            NavigationManager.Instance?.Back();
        }

        #endregion
    }
}