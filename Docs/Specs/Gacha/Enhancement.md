---
type: spec
assembly: Sc.Contents.Gacha
category: Enhancement
status: draft
version: "1.0"
dependencies: [Sc.Common, Sc.Packet, Phase1.RewardPopup, Phase0.LoadingIndicator]
created: 2026-01-18
updated: 2026-01-18
---

# 가챠 강화 (Phase 5.1)

## 목적

Phase 0~4 시스템과 연동하여 가챠 시스템 완성도 향상

## Phase 연계

| 연계 대상 | 적용 내용 |
|-----------|-----------|
| Phase 0 LoadingIndicator | 소환 요청 시 로딩 UI |
| Phase 0 Log | 소환 요청/결과 로깅 |
| Phase 0 Result<T> | API 반환 타입 통일 |
| Phase 1 RewardPopup | 가챠 결과 표시 |
| Phase 1 CostConfirmPopup | 재화 소비 확인 |
| Phase 1 AlertPopup | 에러 표시 |
| Phase 2 TimeHelper | 남은 시간 표시 |
| Phase 4 배너 패턴 | EventBannerItem 참조 |

---

## 마스터 데이터 확장

### GachaPoolData 확장

**위치**: `Assets/Scripts/Data/ScriptableObjects/GachaPoolData.cs`

```csharp
[CreateAssetMenu(fileName = "GachaPoolData", menuName = "SC/Data/GachaPoolData")]
public class GachaPoolData : ScriptableObject
{
    [Header("기본 정보")]
    public string Id;
    public string NameKey;
    public string DescriptionKey;
    public GachaType GachaType;

    [Header("확률")]
    public GachaRates Rates;

    [Header("배너 정보 (신규)")]
    public string BannerImagePath;
    public string BannerNameKey;
    public DateTime StartTime;          // 기간 한정 (null이면 상시)
    public DateTime EndTime;
    public bool IsPickup;               // 픽업 여부
    public List<string> PickupCharacterIds; // 픽업 캐릭터
    public int DisplayOrder;            // 표시 순서

    [Header("천장 정보 (신규)")]
    public int PityThreshold;           // 천장 횟수 (예: 90)
    public float PitySoftStart;         // 소프트 천장 시작 (예: 75)
    public float PitySoftRate;          // 소프트 천장 확률 보너스
    public bool ShowPityProgress;       // UI에 천장 진행도 표시
    public bool ResetPityOnGet;         // ★5 획득 시 천장 리셋

    [Header("비용")]
    public CostType CostType;
    public int SingleCost;              // 1회 비용
    public int MultiCost;               // 10회 비용 (할인 적용)

    // 헬퍼 메서드
    public bool IsActive(DateTime serverTime)
        => StartTime == default || (serverTime >= StartTime && serverTime < EndTime);

    public int GetRemainingDays(DateTime serverTime)
        => EndTime == default ? -1 : Math.Max(0, (EndTime - serverTime).Days);
}
```

---

## 유저 데이터 확장

### GachaHistoryRecord

**위치**: `Assets/Scripts/Data/Structs/UserData/GachaHistoryRecord.cs`

```csharp
[Serializable]
public struct GachaHistoryRecord
{
    public string PoolId;
    public long Timestamp;
    public GachaPullType PullType;      // Single, Multi
    public List<GachaResultItem> Results;
}
```

### UserSaveData 확장

```csharp
// UserSaveData.cs에 추가
public List<GachaHistoryRecord> GachaHistory;

// 헬퍼 메서드
public List<GachaHistoryRecord> GetHistoryByPool(string poolId, int limit = 50)
    => GachaHistory
        .Where(h => h.PoolId == poolId)
        .OrderByDescending(h => h.Timestamp)
        .Take(limit)
        .ToList();
```

---

## Request/Response 확장

### GachaResponse 확장

```csharp
[Serializable]
public struct GachaResponse : IGameActionResponse
{
    public bool IsSuccess { get; set; }
    public ErrorCode ErrorCode { get; set; }
    public long ServerTime { get; set; }
    public UserDataDelta Delta { get; set; }

    public List<GachaResultItem> Results;

    // 신규 필드
    public int CurrentPityCount;        // 현재 천장 카운트
    public int PityThreshold;           // 천장 도달 횟수
    public bool HitPity;                // 이번 소환에서 천장 발동 여부
}
```

---

## UI 구조

### GachaScreen (리팩토링)

```
[GachaScreen]
├── ScreenHeader
├── BannerScrollView (가로 스크롤)
│   └── GachaBannerItem[]
│       ├── BannerImage
│       ├── BannerName
│       ├── RemainingTime
│       ├── PickupCharacterIcons (최대 3개)
│       └── SelectIndicator
├── SelectedBannerDetailPanel
│   ├── PickupInfo
│   │   ├── PickupCharacterCard[]
│   │   └── PickupRateText (예: "픽업 확률 UP!")
│   ├── RateInfo
│   │   ├── Star5Rate (0.6%)
│   │   ├── Star4Rate (5.1%)
│   │   └── Star3Rate (94.3%)
│   ├── PityProgress
│   │   ├── ProgressBar
│   │   ├── CurrentCount / Threshold (예: 45/90)
│   │   └── PityGuaranteeText (예: "45회 후 ★5 확정")
│   └── CostDisplay
│       ├── SingleCost (300 Gem)
│       └── MultiCost (2700 Gem, 10% 할인)
├── ActionButtons
│   ├── [1회 소환] → CostConfirmPopup
│   └── [10회 소환] → CostConfirmPopup
├── SubButtons
│   ├── [확률 상세] → RateDetailPopup
│   └── [히스토리] → GachaHistoryScreen
└── CurrencyHUD (공통)
```

### GachaBannerItem

```csharp
public class GachaBannerItem : Widget
{
    [SerializeField] private Image _bannerImage;
    [SerializeField] private TMP_Text _bannerName;
    [SerializeField] private TMP_Text _remainingTime;
    [SerializeField] private GameObject _pickupBadge;
    [SerializeField] private Image[] _pickupCharacterIcons;
    [SerializeField] private GameObject _selectedIndicator;
    [SerializeField] private GameObject _newBadge;

    private GachaPoolData _data;

    public void Setup(GachaPoolData data, bool isSelected)
    {
        _data = data;
        // 배너 이미지, 이름, 남은 시간, 픽업 캐릭터 표시
        RefreshTimeDisplay();
    }

    public void RefreshTimeDisplay()
    {
        if (_data.EndTime != default)
        {
            var remaining = _data.GetRemainingDays(TimeService.Instance.ServerDateTime);
            _remainingTime.text = TimeHelper.FormatRemainingDays(remaining);
        }
    }
}
```

### GachaHistoryScreen

```
[GachaHistoryScreen]
├── ScreenHeader ([히스토리])
├── FilterTabs
│   ├── [전체]
│   ├── [픽업]
│   └── [일반]
├── HistoryScrollView
│   └── GachaHistoryItem[]
│       ├── Timestamp (TimeHelper 포맷)
│       ├── BannerName
│       ├── PullType (1회/10회)
│       ├── ResultSummary
│       │   ├── Star5Count (★5 x1)
│       │   ├── Star4Count (★4 x3)
│       │   └── Star3Count (★3 x6)
│       └── [상세] 버튼 → GachaHistoryDetailPopup
└── EmptyState (기록 없음)
```

### RateDetailPopup

```
[RateDetailPopup]
├── Title (확률 상세)
├── RateList
│   ├── ★5 캐릭터: 0.6%
│   │   └── 픽업 캐릭터 (50%): 0.3%
│   ├── ★4 캐릭터: 5.1%
│   └── ★3 캐릭터: 94.3%
├── PityExplanation
│   ├── 소프트 천장: 75회부터 확률 상승
│   └── 하드 천장: 90회 ★5 확정
└── [닫기] 버튼
```

---

## 소환 연출 (선택적)

### GachaAnimator

```csharp
public class GachaAnimator : MonoBehaviour
{
    [SerializeField] private float _singleDuration = 2f;
    [SerializeField] private float _multiDuration = 5f;
    [SerializeField] private GameObject _skipButton;

    private bool _isSkipped;

    public async UniTask PlayAsync(List<GachaResultItem> results)
    {
        _skipButton.SetActive(true);
        _isSkipped = false;

        foreach (var result in results)
        {
            if (_isSkipped) break;

            await PlaySingleRevealAsync(result);
        }

        _skipButton.SetActive(false);
    }

    public void OnSkipClicked()
    {
        _isSkipped = true;
    }

    private async UniTask PlaySingleRevealAsync(GachaResultItem result)
    {
        // 희귀도별 연출
        // ★5: 금색 이펙트, 긴 연출
        // ★4: 보라색 이펙트, 중간 연출
        // ★3: 파란색 이펙트, 짧은 연출
    }
}
```

---

## 흐름

### 소환 흐름

```
[1회/10회 소환] 클릭
        │
        ▼
┌─────────────────────┐
│  CostConfirmPopup   │  ← Phase 1
│  "300 Gem 소비?"    │
└─────────┬───────────┘
          │ 확인
          ▼
┌─────────────────────┐
│ LoadingIndicator    │  ← Phase 0
│ "소환 중..."        │
└─────────┬───────────┘
          │
          ▼
[NetworkManager.Send(GachaRequest)]
          │
          ▼
┌─────────────────────┐
│ GachaAnimator       │  ← 선택적
│ (연출, 스킵 가능)   │
└─────────┬───────────┘
          │
          ▼
┌─────────────────────┐
│ RewardPopup         │  ← Phase 1
│ (결과 표시)         │
└─────────┬───────────┘
          │
          ▼
[천장 카운트 갱신]
[히스토리 저장]
```

### 에러 처리 흐름

```
[GachaResponse.IsSuccess == false]
          │
          ▼
┌─────────────────────────────────────┐
│ ErrorCode 분기                       │
│                                      │
│ GACHA_NOT_ENOUGH_CURRENCY:           │
│   → AlertPopup("재화가 부족합니다")  │
│                                      │
│ GACHA_POOL_NOT_FOUND:                │
│   → AlertPopup("배너가 종료되었습니다")│
│                                      │
│ 기타:                                │
│   → AlertPopup(ErrorMessages.Get())  │
└──────────────────────────────────────┘
```

---

## 구현 체크리스트

```
가챠 강화 (Phase 5.1):

마스터 데이터:
- [ ] GachaPoolData.cs 확장 (배너, 천장 필드)
- [ ] GachaPool.json 샘플 데이터 업데이트
- [ ] MasterDataImporter 업데이트

유저 데이터:
- [ ] GachaHistoryRecord.cs 생성
- [ ] UserSaveData.GachaHistory 필드 추가
- [ ] UserSaveData 버전 마이그레이션

Request/Response:
- [ ] GachaResponse 확장 (천장 정보)

UI:
- [ ] GachaBannerItem.cs 생성
- [ ] GachaScreen.cs 리팩토링
- [ ] GachaHistoryScreen.cs 생성
- [ ] GachaHistoryItem.cs 생성
- [ ] RateDetailPopup.cs 생성
- [ ] GachaAnimator.cs 생성 (선택)

연동:
- [ ] GachaResultPopup → RewardPopup 교체
- [ ] 소환 버튼 → CostConfirmPopup 연동
- [ ] 에러 → AlertPopup + ErrorCode 연동
- [ ] 로딩 → LoadingIndicator 적용
- [ ] 남은 시간 → TimeHelper 사용
- [ ] Log.Info/Error 적용

프리팹:
- [ ] GachaBannerItem 프리팹
- [ ] GachaHistoryItem 프리팹
- [ ] MVPSceneSetup 업데이트
```

---

## 관련 문서

- [Gacha.md](../Gacha.md) - 가챠 시스템 개요
- [Common/Reward.md](../Common/Reward.md) - Phase 1 보상 시스템
- [Common/Popups/RewardPopup.md](../Common/Popups/RewardPopup.md) - Phase 1 RewardPopup
