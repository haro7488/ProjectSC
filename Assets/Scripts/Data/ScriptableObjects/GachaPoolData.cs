using System;
using System.Globalization;
using UnityEngine;

namespace Sc.Data
{
    /// <summary>
    /// 가챠 확률 데이터
    /// </summary>
    [Serializable]
    public struct GachaRates
    {
        [Range(0f, 1f)] public float SSR;
        [Range(0f, 1f)] public float SR;
        [Range(0f, 1f)] public float R;

        public float Total => SSR + SR + R;
    }

    /// <summary>
    /// 가챠 풀 마스터 데이터
    /// </summary>
    [CreateAssetMenu(fileName = "GachaPoolData", menuName = "SC/Data/GachaPool")]
    public class GachaPoolData : ScriptableObject
    {
        [Header("기본 정보")]
        [SerializeField] private string _id;
        [SerializeField] private string _name;
        [SerializeField] private string _nameEn;
        [SerializeField] private GachaType _type;

        [Header("비용")]
        [SerializeField] private CostType _costType;
        [SerializeField] private int _costAmount;
        [SerializeField] private int _costAmount10;

        [Header("천장")]
        [SerializeField] private int _pityCount;

        [Header("캐릭터 풀")]
        [SerializeField] private string[] _characterIds;

        [Header("확률")]
        [SerializeField] private GachaRates _rates;

        [Header("픽업")]
        [SerializeField] private string _rateUpCharacterId;
        [Range(0f, 1f)]
        [SerializeField] private float _rateUpBonus;

        [Header("활성화")]
        [SerializeField] private bool _isActive;
        [SerializeField] private string _startDate;
        [SerializeField] private string _endDate;

        [Header("UI")]
        [SerializeField] private string _bannerImagePath;
        [SerializeField] private int _displayOrder;

        [Header("소프트 천장")]
        [SerializeField] private int _pitySoftStart;
        [Range(0f, 1f)]
        [SerializeField] private float _pitySoftRateBonus;

        [Header("설명")]
        [TextArea(2, 4)]
        [SerializeField] private string _description;

        // Properties (읽기 전용)
        public string Id => _id;
        public string Name => _name;
        public string NameEn => _nameEn;
        public GachaType Type => _type;
        public CostType CostType => _costType;
        public int CostAmount => _costAmount;
        public int CostAmount10 => _costAmount10;
        public int PityCount => _pityCount;
        public string[] CharacterIds => _characterIds;
        public GachaRates Rates => _rates;
        public string RateUpCharacterId => _rateUpCharacterId;
        public float RateUpBonus => _rateUpBonus;
        public bool IsActive => _isActive;
        public string StartDate => _startDate;
        public string EndDate => _endDate;
        public string BannerImagePath => _bannerImagePath;
        public int DisplayOrder => _displayOrder;
        public int PitySoftStart => _pitySoftStart;
        public float PitySoftRateBonus => _pitySoftRateBonus;
        public string Description => _description;

        /// <summary>
        /// 지정된 서버 시간 기준으로 풀이 활성 상태인지 확인
        /// </summary>
        public bool IsActiveAt(DateTime serverTime)
        {
            if (!_isActive) return false;

            if (!string.IsNullOrEmpty(_startDate))
            {
                if (DateTime.TryParse(_startDate, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var start))
                {
                    if (serverTime < start) return false;
                }
            }

            if (!string.IsNullOrEmpty(_endDate))
            {
                if (DateTime.TryParse(_endDate, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var end))
                {
                    if (serverTime > end) return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 남은 시간 계산 (종료일이 없으면 null 반환)
        /// </summary>
        public TimeSpan? GetRemainingTime(DateTime serverTime)
        {
            if (string.IsNullOrEmpty(_endDate)) return null;

            if (DateTime.TryParse(_endDate, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var end))
            {
                var remaining = end - serverTime;
                return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
            }

            return null;
        }

#if UNITY_EDITOR
        /// <summary>
        /// Editor 전용: JSON 데이터로 초기화
        /// </summary>
        public void Initialize(string id, string name, string nameEn, GachaType type,
            CostType costType, int costAmount, int costAmount10, int pityCount,
            string[] characterIds, GachaRates rates, string rateUpCharacterId,
            float rateUpBonus, bool isActive, string startDate, string endDate,
            string bannerImagePath, int displayOrder, int pitySoftStart, float pitySoftRateBonus,
            string description)
        {
            _id = id;
            _name = name;
            _nameEn = nameEn;
            _type = type;
            _costType = costType;
            _costAmount = costAmount;
            _costAmount10 = costAmount10;
            _pityCount = pityCount;
            _characterIds = characterIds;
            _rates = rates;
            _rateUpCharacterId = rateUpCharacterId;
            _rateUpBonus = rateUpBonus;
            _isActive = isActive;
            _startDate = startDate;
            _endDate = endDate;
            _bannerImagePath = bannerImagePath;
            _displayOrder = displayOrder;
            _pitySoftStart = pitySoftStart;
            _pitySoftRateBonus = pitySoftRateBonus;
            _description = description;
        }
#endif
    }
}
