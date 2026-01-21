---
type: spec
assembly: Sc.Contents.Gacha
category: System
status: implemented
version: "1.0"
dependencies: [Sc.Common, Sc.Packet, Sc.Data, Sc.Event, Sc.LocalServer]
created: 2026-01-14
updated: 2026-01-21
---

# Sc.Contents.Gacha

## 목적

캐릭터 소환(뽑기) 시스템. 재화를 소비하여 캐릭터를 획득하고 천장 시스템을 통해 확정 획득 제공

## 의존성

### 참조
- `Sc.Common` - UI 시스템, Navigation
- `Sc.Packet` - NetworkManager, Request/Response
- `Sc.Data` - 마스터/유저 데이터
- `Sc.Event` - 가챠 이벤트 발행
- `Sc.LocalServer` - GachaHandler, GachaService

### 참조됨
- `Sc.Contents.Lobby` - 가챠 진입

---

## 아키텍처 개요

```
┌─────────────────────────────────────────────────────────────┐
│                        UI Layer                              │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────────────┐  │
│  │ GachaScreen │  │GachaResult  │  │   CurrencyHUD       │  │
│  │             │  │   Popup     │  │                     │  │
│  └──────┬──────┘  └─────────────┘  └─────────────────────┘  │
└─────────┼───────────────────────────────────────────────────┘
          │
          ▼
┌─────────────────────────────────────────────────────────────┐
│                       Data Layer                             │
│  ┌─────────────────┐  ┌─────────────────┐                   │
│  │ GachaPoolData   │  │GachaPool        │                   │
│  │ (SO)            │  │Database (SO)    │                   │
│  └─────────────────┘  └─────────────────┘                   │
│  ┌─────────────────┐  ┌─────────────────┐                   │
│  │ GachaRates      │  │ GachaPityData   │                   │
│  │ (struct)        │  │ (UserData)      │                   │
│  └─────────────────┘  └─────────────────┘                   │
└─────────────────────────────────────────────────────────────┘
          │
          ▼
┌─────────────────────────────────────────────────────────────┐
│                      Server Layer                            │
│  ┌─────────────────────────────────────────────────────┐    │
│  │                   GachaHandler                       │    │
│  │  ├─ 재화 검증 (ServerValidator)                     │    │
│  │  ├─ 가챠 실행 (GachaService)                        │    │
│  │  ├─ 천장 카운트 관리 (GachaPityData)                │    │
│  │  └─ Delta 생성 (RewardService)                      │    │
│  └─────────────────────────────────────────────────────┘    │
│  ┌─────────────────────────────────────────────────────┐    │
│  │                   GachaService                       │    │
│  │  ├─ 확률 계산 (Rates + 천장 보정)                   │    │
│  │  └─ 캐릭터 선정 (풀별 랜덤)                         │    │
│  └─────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────┘
```

---

## 클래스 역할 정의

### 마스터 데이터

| 클래스 | 역할 | 상태 |
|--------|------|------|
| `GachaType` | 가챠 타입 열거형 (Standard, Pickup, Free) | ✅ |
| `GachaRates` | 확률 구조체 (SSR, SR, R) | ✅ |
| `GachaPoolData` | 가챠 풀 SO | ✅ |
| `GachaPoolDatabase` | 가챠 풀 DB SO | ✅ |

### 유저 데이터

| 클래스 | 역할 | 상태 |
|--------|------|------|
| `GachaPityData` | 천장 데이터 컨테이너 | ✅ |
| `GachaPityInfo` | 풀별 천장 정보 | ✅ |

### Request/Response

| 클래스 | 역할 | 상태 |
|--------|------|------|
| `GachaRequest` | 가챠 요청 (풀 ID, 타입) | ✅ |
| `GachaResponse` | 가챠 응답 (결과, 천장, Delta) | ✅ |
| `GachaResultItem` | 결과 아이템 (캐릭터, 희귀도, NEW, 천장) | ✅ |

### 이벤트

| 클래스 | 역할 | 상태 |
|--------|------|------|
| `GachaCompletedEvent` | 가챠 완료 이벤트 | ✅ |
| `GachaFailedEvent` | 가챠 실패 이벤트 | ✅ |

### Server Layer

| 클래스 | 역할 | 상태 |
|--------|------|------|
| `GachaHandler` | 가챠 요청 핸들러 | ✅ |
| `GachaService` | 확률 계산, 캐릭터 선정 | ✅ |

### UI

| 클래스 | 역할 | 상태 |
|--------|------|------|
| `GachaScreen` | 가챠 메인 화면 | ✅ |
| `GachaState` | 화면 상태 (선택된 풀 ID) | ✅ |
| `GachaResultPopup` | 가챠 결과 팝업 | ✅ |
| `GachaResultState` | 결과 팝업 상태 | ✅ |

---

## 마스터 데이터

### GachaType

**위치**: `Assets/Scripts/Data/Enums/GachaType.cs`

```csharp
public enum GachaType
{
    Standard,   // 상시 배너
    Pickup,     // 픽업 배너
    Free        // 무료 일일 소환
}
```

### GachaRates

**위치**: `Assets/Scripts/Data/ScriptableObjects/GachaPoolData.cs`

```csharp
[Serializable]
public struct GachaRates
{
    [Range(0f, 1f)] public float SSR;   // ★5 확률 (0.03 = 3%)
    [Range(0f, 1f)] public float SR;    // ★4 확률 (0.12 = 12%)
    [Range(0f, 1f)] public float R;     // ★3 확률 (0.85 = 85%)

    public float Total => SSR + SR + R;
}
```

### GachaPoolData

**위치**: `Assets/Scripts/Data/ScriptableObjects/GachaPoolData.cs`

```csharp
[CreateAssetMenu(fileName = "GachaPoolData", menuName = "SC/Data/GachaPool")]
public class GachaPoolData : ScriptableObject
{
    [Header("기본 정보")]
    public string Id;
    public string Name;
    public string NameEn;
    public GachaType Type;

    [Header("비용")]
    public CostType CostType;
    public int CostAmount;          // 1회 비용
    public int CostAmount10;        // 10회 비용

    [Header("천장")]
    public int PityCount;           // 하드 천장 (예: 90)

    [Header("캐릭터 풀")]
    public string[] CharacterIds;

    [Header("확률")]
    public GachaRates Rates;

    [Header("픽업")]
    public string RateUpCharacterId;    // 픽업 캐릭터 (단일)
    [Range(0f, 1f)]
    public float RateUpBonus;           // 픽업 확률 보너스

    [Header("활성화")]
    public bool IsActive;
    public string StartDate;            // ISO 8601 형식
    public string EndDate;

    [Header("설명")]
    public string Description;
}
```

### GachaPool.json 예시

```json
{
  "version": "1.0.0",
  "data": [
    {
      "Id": "gacha_standard",
      "Name": "일반 소환",
      "Type": "Standard",
      "CostType": "Gem",
      "CostAmount": 300,
      "CostAmount10": 2700,
      "PityCount": 90,
      "Rates": { "SSR": 0.03, "SR": 0.12, "R": 0.85 },
      "IsActive": true
    },
    {
      "Id": "gacha_pickup_aria",
      "Name": "아리아 픽업 소환",
      "Type": "Pickup",
      "RateUpCharacterId": "char_001",
      "RateUpBonus": 0.5,
      "StartDate": "2026-01-15T00:00:00Z",
      "EndDate": "2026-02-15T00:00:00Z"
    }
  ]
}
```

---

## 유저 데이터

### GachaPityData

**위치**: `Assets/Scripts/Data/Structs/UserData/GachaPityData.cs`

```csharp
[Serializable]
public struct GachaPityData
{
    public List<GachaPityInfo> PityInfos;

    public GachaPityInfo GetOrCreatePityInfo(string gachaPoolId)
    {
        PityInfos ??= new List<GachaPityInfo>();
        var existing = PityInfos.FirstOrDefault(p => p.GachaPoolId == gachaPoolId);
        if (existing.GachaPoolId == null)
        {
            existing = new GachaPityInfo { GachaPoolId = gachaPoolId };
            PityInfos.Add(existing);
        }
        return existing;
    }
}

[Serializable]
public struct GachaPityInfo
{
    public string GachaPoolId;
    public int PityCount;           // 현재 천장 카운트
    public int TotalPullCount;      // 누적 소환 횟수
    public long LastPullAt;         // 마지막 소환 시각
}
```

---

## Request/Response

### GachaRequest

**위치**: `Assets/Scripts/Data/Requests/GachaRequest.cs`

```csharp
[Serializable]
public struct GachaRequest : IRequest
{
    public long Timestamp { get; set; }
    public string GachaPoolId;
    public GachaPullType PullType;      // Single, Multi

    public static GachaRequest CreateSingle(string poolId)
        => new() { GachaPoolId = poolId, PullType = GachaPullType.Single };

    public static GachaRequest CreateMulti(string poolId)
        => new() { GachaPoolId = poolId, PullType = GachaPullType.Multi };
}

public enum GachaPullType { Single, Multi }
```

### GachaResponse

**위치**: `Assets/Scripts/Data/Responses/GachaResponse.cs`

```csharp
[Serializable]
public class GachaResponse : IGameActionResponse
{
    public bool IsSuccess { get; set; }
    public int ErrorCode { get; set; }
    public string ErrorMessage { get; set; }
    public long ServerTime { get; set; }
    public UserDataDelta Delta { get; set; }

    public List<GachaResultItem> Results;
    public int CurrentPityCount;
}

[Serializable]
public class GachaResultItem
{
    public string CharacterId;
    public Rarity Rarity;
    public bool IsNew;
    public bool IsPity;
}
```

---

## 흐름

### 가챠 실행 흐름

```
[GachaScreen] 1회/10회 버튼 클릭
    │
    ▼
[NetworkManager.Send(GachaRequest)]
    │
    ▼
[GachaHandler.Handle]
    ├─ 재화 검증 (ServerValidator)
    ├─ 가챠 실행 (GachaService)
    │   ├─ 천장 확률 보정
    │   └─ 캐릭터 랜덤 선택
    ├─ 캐릭터 지급 (UserSaveData.Characters)
    ├─ 재화 차감 (RewardService)
    ├─ 천장 카운트 업데이트
    └─ Delta 생성
    │
    ▼
[GachaResponse]
    │
    ▼
[GachaResponseHandler]
    ├─ DataManager.ApplyDelta
    └─ EventManager.Publish(GachaCompletedEvent)
    │
    ▼
[GachaScreen.OnGachaCompleted]
    │
    ▼
[GachaResultPopup.Open]
```

### 천장 시스템

```
PityCount 증가 (매 소환)
    │
    ├─ PityCount >= 90 → SSR 확정
    │
    └─ SSR 획득 시 → PityCount = 0 (리셋)
```

---

## 에러 코드

| ErrorCode | 값 | 설명 |
|-----------|-----|------|
| `GACHA_NOT_ENOUGH_CURRENCY` | 1001 | 재화 부족 |
| `GACHA_POOL_NOT_FOUND` | 1002 | 풀 없음/종료 |
| `GACHA_POOL_NOT_ACTIVE` | 1003 | 비활성 풀 |

---

## 파일 구조

```
Assets/Scripts/
├── Contents/OutGame/Gacha/
│   ├── GachaScreen.cs          ✅
│   ├── GachaResultPopup.cs     ✅
│   └── IsExternalInit.cs       ✅
├── Data/
│   ├── ScriptableObjects/
│   │   ├── GachaPoolData.cs    ✅
│   │   └── GachaPoolDatabase.cs ✅
│   ├── Requests/
│   │   └── GachaRequest.cs     ✅
│   └── Responses/
│       └── GachaResponse.cs    ✅
├── LocalServer/
│   ├── Handlers/
│   │   └── GachaHandler.cs     ✅
│   └── Services/
│       └── GachaService.cs     ✅
└── Event/OutGame/
    └── GachaEvents.cs          ✅

Assets/Data/MasterData/
└── GachaPool.json              ✅
```

---

## 관련 문서

- [Gacha/Enhancement.md](Gacha/Enhancement.md) - 가챠 강화 (배너 스크롤, 히스토리 등)
- [Common/Popups/CostConfirmPopup.md](Common/Popups/CostConfirmPopup.md) - 재화 확인 팝업
- [Common/Popups/RewardPopup.md](Common/Popups/RewardPopup.md) - 보상 팝업
