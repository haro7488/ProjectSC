---
type: spec
assembly: Sc.Contents.Gacha
category: Enhancement
status: implemented
version: "2.0"
dependencies: [Sc.Common, Sc.Packet, Sc.Data, Phase1.CostConfirmPopup, Phase0.LoadingIndicator]
created: 2026-01-18
updated: 2026-01-21
---

# 가챠 강화 (GachaEnhancement)

## 목적

기존 가챠 시스템을 확장하여 다중 배너 스크롤, 소환 히스토리, 확률 상세 팝업, Phase 0~1 연동 등 완성도 향상

## Phase 연계

| 연계 대상 | 적용 내용 |
|-----------|-----------|
| Phase 0 LoadingIndicator | 소환 요청 시 로딩 UI |
| Phase 0 Log | 소환 요청/결과 로깅 |
| Phase 1 CostConfirmPopup | 재화 소비 확인 |
| Phase 1 AlertPopup | 에러 표시 |
| Phase 2 TimeHelper | 남은 시간 포맷팅 |

---

## 아키텍처 변경 개요

### 현재 → 목표

```
[현재]
GachaScreen (단일 풀 하드코딩)
    └── SinglePull / MultiPull 버튼
    └── GachaResultPopup (커스텀)

[목표]
GachaScreen (다중 풀 선택)
    ├── BannerScrollView
    │   └── GachaBannerItem[] (선택 가능)
    ├── SelectedBannerDetailPanel
    │   ├── PityProgressBar
    │   ├── RateInfoDisplay
    │   └── PickupCharacterDisplay
    ├── ActionButtons
    │   └── SinglePull / MultiPull → CostConfirmPopup
    └── SubButtons
        ├── [확률 상세] → RateDetailPopup
        └── [히스토리] → GachaHistoryScreen
```

---

## 구현 범위

### Phase A: 마스터 데이터 확장

| 항목 | 설명 | 필드 |
|------|------|------|
| 배너 표시 | 배너 이미지, 표시 순서 | `BannerImagePath`, `DisplayOrder` |
| 소프트 천장 | 확률 점진 증가 | `PitySoftStart`, `PitySoftRateBonus` |

### Phase B: 유저 데이터 확장

| 항목 | 설명 |
|------|------|
| GachaHistoryRecord | 소환 이력 기록 |
| UserSaveData Migration | GachaHistory 필드 추가 |

### Phase C: GachaScreen 리팩토링

| 항목 | 설명 |
|------|------|
| BannerScrollView | 활성 배너 가로 스크롤 |
| GachaBannerItem | 배너 위젯 (선택, 남은 시간) |
| SelectedBannerDetailPanel | 선택 배너 상세 정보 |
| CostConfirmPopup 연동 | 소환 버튼 클릭 시 확인 팝업 |
| LoadingIndicator 연동 | 요청 중 로딩 표시 |

### Phase D: 팝업 추가

| 항목 | 설명 |
|------|------|
| RateDetailPopup | 확률 상세 (등급별 확률, 천장 설명) |

### Phase E: 히스토리 화면

| 항목 | 설명 |
|------|------|
| GachaHistoryScreen | 소환 이력 목록 |
| GachaHistoryItem | 이력 아이템 위젯 |

### Phase F (선택): 연출

| 항목 | 설명 |
|------|------|
| GachaAnimator | 소환 연출 (희귀도별, 스킵 가능) |

---

## 마스터 데이터 확장

### GachaPoolData 확장 필드

**위치**: `Assets/Scripts/Data/ScriptableObjects/GachaPoolData.cs`

```csharp
// 기존 필드 유지 + 추가

[Header("배너 표시 (Enhancement)")]
[SerializeField] private string _bannerImagePath;    // Addressables 경로
[SerializeField] private int _displayOrder;          // 표시 순서 (낮을수록 앞)

[Header("소프트 천장 (Enhancement)")]
[SerializeField] private int _pitySoftStart;         // 소프트 천장 시작 (예: 75)
[SerializeField] private float _pitySoftRateBonus;   // 회당 추가 확률 (예: 0.06)

// Properties
public string BannerImagePath => _bannerImagePath;
public int DisplayOrder => _displayOrder;
public int PitySoftStart => _pitySoftStart;
public float PitySoftRateBonus => _pitySoftRateBonus;

// 헬퍼 메서드
public bool IsActive(DateTime serverTime)
{
    if (string.IsNullOrEmpty(StartDate)) return IsActive;

    var start = DateTime.Parse(StartDate, null, DateTimeStyles.RoundtripKind);
    if (serverTime < start) return false;

    if (string.IsNullOrEmpty(EndDate)) return true;
    var end = DateTime.Parse(EndDate, null, DateTimeStyles.RoundtripKind);
    return serverTime < end;
}

public TimeSpan GetRemainingTime(DateTime serverTime)
{
    if (string.IsNullOrEmpty(EndDate)) return TimeSpan.MaxValue;
    var end = DateTime.Parse(EndDate, null, DateTimeStyles.RoundtripKind);
    return end - serverTime;
}
```

### GachaPool.json 확장

```json
{
  "Id": "gacha_pickup_aria",
  "Name": "아리아 픽업 소환",
  "Type": "Pickup",
  "CostType": "Gem",
  "CostAmount": 300,
  "CostAmount10": 2700,
  "PityCount": 90,
  "PitySoftStart": 75,
  "PitySoftRateBonus": 0.06,
  "BannerImagePath": "UI/Banners/banner_aria",
  "DisplayOrder": 1,
  "RateUpCharacterId": "char_001",
  "RateUpBonus": 0.5,
  "StartDate": "2026-01-15T00:00:00Z",
  "EndDate": "2026-02-15T00:00:00Z"
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
    public string Id;                       // UUID
    public string PoolId;                   // 가챠 풀 ID
    public string PoolName;                 // 풀 이름 (표시용)
    public long Timestamp;                  // Unix timestamp
    public GachaPullType PullType;          // Single, Multi
    public List<GachaHistoryResultItem> Results;

    // 요약 정보 (UI 표시용)
    public int SSRCount;
    public int SRCount;
    public int RCount;
}

[Serializable]
public struct GachaHistoryResultItem
{
    public string CharacterId;
    public Rarity Rarity;
    public bool IsNew;
    public bool IsPity;
}
```

### UserSaveData 확장

**Migration**: v7 (v6 → v7)

```csharp
// UserSaveData.cs
public List<GachaHistoryRecord> GachaHistory;

// 헬퍼 메서드
public List<GachaHistoryRecord> GetRecentHistory(int limit = 100)
    => GachaHistory?
        .OrderByDescending(h => h.Timestamp)
        .Take(limit)
        .ToList() ?? new List<GachaHistoryRecord>();

public List<GachaHistoryRecord> GetHistoryByPool(string poolId, int limit = 50)
    => GachaHistory?
        .Where(h => h.PoolId == poolId)
        .OrderByDescending(h => h.Timestamp)
        .Take(limit)
        .ToList() ?? new List<GachaHistoryRecord>();
```

### SaveMigrator 추가

```csharp
// v6 → v7
private UserSaveData MigrateV6ToV7(UserSaveData data)
{
    data.GachaHistory ??= new List<GachaHistoryRecord>();
    data.Version = 7;
    return data;
}
```

---

## GachaResponse 확장

**위치**: `Assets/Scripts/Data/Responses/GachaResponse.cs`

```csharp
[Serializable]
public class GachaResponse : IGameActionResponse
{
    // 기존 필드 유지
    public bool IsSuccess { get; set; }
    public int ErrorCode { get; set; }
    public string ErrorMessage { get; set; }
    public long ServerTime { get; set; }
    public UserDataDelta Delta { get; set; }
    public List<GachaResultItem> Results;
    public int CurrentPityCount;

    // 추가 필드
    public int PityThreshold;           // 하드 천장 횟수
    public int PitySoftStart;           // 소프트 천장 시작
    public bool HitPity;                // 이번 소환에서 천장 발동 여부
}
```

---

## UI 구조

### GachaScreen (리팩토링)

```
[GachaScreen]
├── ScreenHeader
├── BannerScrollView (가로 스크롤, SnapCenter)
│   └── GachaBannerItem[]
│       ├── BannerImage (Addressables)
│       ├── BannerName
│       ├── RemainingTimeText (TimeHelper 포맷)
│       ├── PickupBadge (픽업 여부)
│       ├── FreeBadge (무료 소환)
│       └── SelectedIndicator
├── SelectedBannerDetailPanel
│   ├── BannerTitle
│   ├── PityProgressBar
│   │   ├── FillBar
│   │   ├── CurrentText (45회)
│   │   ├── ThresholdText (/90)
│   │   └── SoftPityMarker (75회 위치)
│   ├── PickupCharacterDisplay
│   │   ├── CharacterIcon
│   │   ├── CharacterName
│   │   └── PickupRateText (확률 UP!)
│   └── RateSummary
│       ├── SSRRate (★5: 3.0%)
│       ├── SRRate (★4: 12.0%)
│       └── RRate (★3: 85.0%)
├── CostDisplay
│   ├── SingleCost (300 Gem)
│   └── MultiCost (2,700 Gem)
├── ActionButtons
│   ├── [1회 소환] → CostConfirmPopup
│   └── [10회 소환] → CostConfirmPopup
├── SubButtons
│   ├── [확률 상세] → RateDetailPopup
│   └── [히스토리] → GachaHistoryScreen
└── CurrencyHUD (공통)
```

### GachaBannerItem

**위치**: `Assets/Scripts/Contents/OutGame/Gacha/GachaBannerItem.cs`

```csharp
public class GachaBannerItem : MonoBehaviour
{
    [SerializeField] private Image _bannerImage;
    [SerializeField] private TMP_Text _bannerName;
    [SerializeField] private TMP_Text _remainingTime;
    [SerializeField] private GameObject _pickupBadge;
    [SerializeField] private GameObject _freeBadge;
    [SerializeField] private GameObject _selectedIndicator;
    [SerializeField] private Button _button;

    private GachaPoolData _data;
    private Action<GachaPoolData> _onSelected;

    public void Setup(GachaPoolData data, bool isSelected, Action<GachaPoolData> onSelected)
    {
        _data = data;
        _onSelected = onSelected;

        _bannerName.text = data.Name;
        _pickupBadge.SetActive(data.Type == GachaType.Pickup);
        _freeBadge.SetActive(data.Type == GachaType.Free);
        _selectedIndicator.SetActive(isSelected);

        RefreshRemainingTime();
        LoadBannerImage();

        _button.onClick.AddListener(() => _onSelected?.Invoke(_data));
    }

    public void RefreshRemainingTime()
    {
        if (string.IsNullOrEmpty(_data.EndDate))
        {
            _remainingTime.gameObject.SetActive(false);
            return;
        }

        var serverTime = TimeService.Instance.ServerDateTime;
        var remaining = _data.GetRemainingTime(serverTime);
        _remainingTime.text = TimeHelper.FormatRemainingTime(remaining);
        _remainingTime.gameObject.SetActive(true);
    }

    private async void LoadBannerImage()
    {
        if (string.IsNullOrEmpty(_data.BannerImagePath)) return;
        var sprite = await AssetManager.Instance.LoadAsync<Sprite>(_data.BannerImagePath);
        if (sprite != null) _bannerImage.sprite = sprite;
    }

    public void SetSelected(bool selected)
    {
        _selectedIndicator.SetActive(selected);
    }
}
```

### RateDetailPopup

```
[RateDetailPopup]
├── Title (확률 상세)
├── RateList
│   ├── ★5 캐릭터: 3.0%
│   │   └── (픽업 시) 픽업 캐릭터: 1.5%
│   ├── ★4 캐릭터: 12.0%
│   └── ★3 캐릭터: 85.0%
├── PityExplanation
│   ├── 소프트 천장: 75회부터 회당 6% 확률 증가
│   └── 하드 천장: 90회 ★5 확정
├── CharacterList (접이식)
│   └── 획득 가능 캐릭터 목록
└── [닫기] 버튼
```

### GachaHistoryScreen

```
[GachaHistoryScreen]
├── ScreenHeader ([소환 기록])
├── FilterTabs (TabGroupWidget)
│   ├── [전체]
│   ├── [픽업]
│   └── [일반]
├── HistoryScrollView
│   └── GachaHistoryItem[]
│       ├── Timestamp (2026-01-21 14:30)
│       ├── PoolName (아리아 픽업 소환)
│       ├── PullType (10회 소환)
│       ├── ResultSummary
│       │   ├── SSRBadge (★5 x1)
│       │   ├── SRBadge (★4 x3)
│       │   └── RBadge (★3 x6)
│       └── [상세] 버튼 (선택적)
└── EmptyState (기록이 없습니다)
```

---

## 흐름

### 소환 흐름 (Enhancement 적용)

```
[GachaScreen] 배너 선택
        │
        ▼
[SelectedBannerDetailPanel 갱신]
        │
        ▼
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
│ Show()              │
└─────────┬───────────┘
          │
          ▼
[NetworkManager.Send(GachaRequest)]
          │
          ▼
[GachaHandler.Handle]
    ├─ 소프트 천장 확률 계산
    ├─ 가챠 실행
    ├─ 히스토리 저장 (GachaHistory)
    └─ Response 생성
          │
          ▼
┌─────────────────────┐
│ LoadingIndicator    │
│ Hide()              │
└─────────┬───────────┘
          │
          ▼
┌─────────────────────┐
│ GachaResultPopup    │
│ (결과 표시)         │
└─────────┬───────────┘
          │
          ▼
[PityProgressBar 갱신]
```

### 에러 처리 흐름

```
[GachaResponse.IsSuccess == false]
          │
          ▼
┌───────────────────────────────────────┐
│ ErrorCode 분기                        │
│                                       │
│ 1001 (재화 부족):                     │
│   → AlertPopup("보석이 부족합니다")   │
│                                       │
│ 1002 (풀 없음):                       │
│   → AlertPopup("배너가 종료됨")       │
│   → 배너 목록 새로고침                │
│                                       │
│ 기타:                                 │
│   → AlertPopup(ErrorMessages.Get())   │
└───────────────────────────────────────┘
```

---

## GachaHandler 수정사항

### 소프트 천장 확률 계산

```csharp
// GachaService.cs
public Rarity CalculateRarity(int currentPity, GachaPoolData pool)
{
    var ssrRate = pool.Rates.SSR;

    // 하드 천장
    if (currentPity >= pool.PityCount)
        return Rarity.SSR;

    // 소프트 천장
    if (pool.PitySoftStart > 0 && currentPity >= pool.PitySoftStart)
    {
        var overCount = currentPity - pool.PitySoftStart;
        ssrRate += pool.PitySoftRateBonus * overCount;
        ssrRate = Math.Min(ssrRate, 1f);  // 최대 100%
    }

    var roll = Random.Range(0f, 1f);
    if (roll < ssrRate) return Rarity.SSR;
    if (roll < ssrRate + pool.Rates.SR) return Rarity.SR;
    return Rarity.R;
}
```

### 히스토리 저장

```csharp
// GachaHandler.cs Handle 메서드 내
// 가챠 실행 후 히스토리 저장
var historyRecord = new GachaHistoryRecord
{
    Id = Guid.NewGuid().ToString(),
    PoolId = request.GachaPoolId,
    PoolName = poolData.Name,
    Timestamp = _timeService.ServerTimeUtc,
    PullType = request.PullType,
    Results = results.Select(r => new GachaHistoryResultItem
    {
        CharacterId = r.CharacterId,
        Rarity = r.Rarity,
        IsNew = r.IsNew,
        IsPity = r.IsPity
    }).ToList(),
    SSRCount = results.Count(r => r.Rarity == Rarity.SSR),
    SRCount = results.Count(r => r.Rarity == Rarity.SR),
    RCount = results.Count(r => r.Rarity == Rarity.R)
};

userData.GachaHistory ??= new List<GachaHistoryRecord>();
userData.GachaHistory.Add(historyRecord);

// 최근 200건만 유지
if (userData.GachaHistory.Count > 200)
{
    userData.GachaHistory = userData.GachaHistory
        .OrderByDescending(h => h.Timestamp)
        .Take(200)
        .ToList();
}
```

---

## 구현 체크리스트

```
가챠 강화 (GachaEnhancement):

Phase A - 마스터 데이터 확장:
- [x] GachaPoolData.cs 필드 추가 (BannerImagePath, DisplayOrder, PitySoft*)
- [x] GachaPool.json 샘플 데이터 업데이트
- [x] MasterDataImporter 업데이트

Phase B - 유저 데이터 확장:
- [x] GachaHistoryRecord.cs 생성
- [x] UserSaveData.GachaHistory 필드 추가
- [x] SaveMigrator v7→v8 마이그레이션

Phase C - GachaScreen 리팩토링:
- [x] GachaBannerItem.cs 생성
- [x] GachaScreen 배너 스크롤 구현
- [x] SelectedBannerDetailPanel 구현
- [x] PityProgressBar 위젯 구현
- [x] CostConfirmPopup 연동
- [x] LoadingIndicator 연동
- [x] Log.Info/Error 적용

Phase D - 팝업:
- [x] RateDetailPopup.cs 생성
- [ ] RateDetailPopup 프리팹 (Unity에서 설정 필요)

Phase E - 히스토리:
- [x] GachaHistoryScreen.cs 생성
- [x] GachaHistoryState.cs 생성
- [x] GachaHistoryItem.cs 생성
- [ ] GachaHistoryScreen 프리팹 (Unity에서 설정 필요)

Phase F - Server:
- [x] GachaService 소프트 천장 로직
- [x] GachaHandler 히스토리 저장 로직
- [x] GachaResponse 확장 필드 (HitPity, PityThreshold)

테스트:
- [x] GachaPoolData 헬퍼 메서드 테스트
- [x] 소프트 천장 확률 계산 테스트
- [x] 히스토리 저장/조회 테스트
```

---

## 파일 구조 (목표)

```
Assets/Scripts/Contents/OutGame/Gacha/
├── GachaScreen.cs              (수정)
├── GachaResultPopup.cs         (기존)
├── GachaBannerItem.cs          (신규)
├── GachaHistoryScreen.cs       (신규)
├── GachaHistoryItem.cs         (신규)
├── RateDetailPopup.cs          (신규)
└── Widgets/
    └── PityProgressBar.cs      (신규)

Assets/Scripts/Data/
├── ScriptableObjects/
│   └── GachaPoolData.cs        (수정)
├── Structs/UserData/
│   └── GachaHistoryRecord.cs   (신규)
└── Responses/
    └── GachaResponse.cs        (수정)

Assets/Scripts/LocalServer/
└── Services/
    └── GachaService.cs         (수정)
```

---

## 관련 문서

- [Gacha.md](../Gacha.md) - 가챠 시스템 기본
- [Common/Popups/CostConfirmPopup.md](../Common/Popups/CostConfirmPopup.md) - Phase 1 CostConfirmPopup
- [Core/TimeService.md](../Core/TimeService.md) - TimeHelper 포맷팅
- [Foundation/LoadingIndicator.md](../Foundation/LoadingIndicator.md) - Phase 0 로딩
