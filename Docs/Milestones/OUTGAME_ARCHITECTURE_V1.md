# 아웃게임 아키텍처 1차 완성 마일스톤

## 개요

| 항목 | 내용 |
|------|------|
| **마일스톤 ID** | OUTGAME-V1 |
| **목표** | 아웃게임 핵심 기능 기초 토대 완성 |
| **범위** | 가챠, 상점, 캐릭터리스트, 스테이지진입, 이벤트진입 |
| **시작일** | 2026-01-17 |
| **상태** | 🔨 진행 중 |

---

## 목표 기능

```
┌─────────────────────────────────────────────────────────────┐
│                      [Lobby Screen]                          │
│                                                              │
│   ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐   │
│   │  가챠    │  │  상점    │  │ 캐릭터   │  │ 스테이지 │   │
│   │ (Gacha)  │  │ (Shop)   │  │ (Char)   │  │ (Stage)  │   │
│   └────┬─────┘  └────┬─────┘  └────┬─────┘  └────┬─────┘   │
│        │             │             │             │          │
│   ┌──────────┐                                              │
│   │ 이벤트   │                                              │
│   │ (Event)  │                                              │
│   └────┬─────┘                                              │
└────────┼─────────────┼─────────────┼─────────────┼──────────┘
         │             │             │             │
         ▼             ▼             ▼             ▼
┌──────────────┐ ┌──────────────┐ ┌──────────────┐ ┌──────────────┐
│ GachaScreen  │ │ ShopScreen   │ │ CharList     │ │ StageList    │
│ GachaResult  │ │ PurchasePopup│ │ CharDetail   │ │ PartySelect  │
│ (✅ 완료)    │ │ (❌ 미구현)  │ │ (✅ 완료)    │ │ (❌ 미구현)  │
└──────────────┘ └──────────────┘ └──────────────┘ └──────────────┘
         │
         ▼
┌──────────────┐
│ EventDash    │
│ EventDetail  │
│ (❌ 미구현)  │
└──────────────┘
```

---

## Phase 구성

| Phase | 이름 | 핵심 산출물 | 의존성 |
|-------|------|-------------|--------|
| **0** | Foundation | Log, ErrorCode, SaveManager, LoadingIndicator | - |
| **1** | 공통 모듈 | RewardInfo, TimeService, SystemPopup, RewardPopup | Phase 0 |
| **2** | 상점 | ShopScreen, PurchaseAsync | Phase 1 |
| **3** | 스테이지 진입 | StageListScreen, PartySelectScreen | Phase 1 |
| **4** | 라이브 이벤트 | EventDashboard, EventDetail | Phase 1, 2 |
| **5** | 기존 기능 강화 | 가챠 연출, 캐릭터 필터/정렬 | Phase 1~4 |

### 제외 항목 (별도 마일스톤)

| 기능 | 이유 | 예정 마일스톤 |
|------|------|---------------|
| **시즌패스** | 미션 기반형 선택 → 별도 시스템 필요 (복잡도 높음) | PASS-V1 |

---

## Phase 0: Foundation

> **목표**: 전체 시스템에서 사용하는 기반 인프라 구축
> **상태**: 📝 설계 완료

### 0.1 로깅 시스템 (Log)

**필요 이유**: 디버깅, 문제 추적, 릴리즈 빌드 최적화

#### 산출물

| 파일 | 위치 | 역할 |
|------|------|------|
| `LogLevel.cs` | Foundation/ | 로그 레벨 enum |
| `Log.cs` | Foundation/ | 정적 로깅 API |
| `ILogOutput.cs` | Foundation/ | 출력 인터페이스 |
| `UnityLogOutput.cs` | Foundation/ | Unity 콘솔 출력 |
| `LogConfig.cs` | Foundation/ | 로그 설정 (ScriptableObject) |

#### LogLevel 정의
```csharp
public enum LogLevel
{
    Verbose = 0,    // 상세 디버깅 (릴리즈 제거)
    Debug = 1,      // 개발 디버깅 (릴리즈 제거)
    Info = 2,       // 정보성 로그
    Warning = 3,    // 경고
    Error = 4,      // 에러
    None = 5,       // 로그 끔
}
```

#### Log API
```csharp
public static class Log
{
    // 기본 사용
    [Conditional("ENABLE_LOG_VERBOSE")]
    public static void Verbose(string message);
    
    [Conditional("ENABLE_LOG_DEBUG"), Conditional("UNITY_EDITOR")]
    public static void Debug(string message);
    
    public static void Info(string message);
    public static void Warning(string message);
    public static void Error(string message, Exception ex = null);
    
    // 카테고리 지정
    public static void Info(string category, string message);
    
    // 구조화된 로그
    public static void Info<T>(string category, string message, T context);
}
```

### 0.2 에러 처리 시스템 (Error)

**필요 이유**: 일관된 에러 코드, 명시적 에러 전파

#### 산출물

| 파일 | 위치 | 역할 |
|------|------|------|
| `ErrorCode.cs` | Foundation/ | 에러 코드 상수 |
| `ErrorMessages.cs` | Foundation/ | 에러 메시지 매핑 |
| `Result.cs` | Foundation/ | Result<T> 구조체 |

#### ErrorCode 체계
```csharp
public static class ErrorCode
{
    // 공통 (0xxx)
    public const string SUCCESS = "0000";
    public const string UNKNOWN = "0001";
    public const string INVALID_REQUEST = "0002";
    public const string TIMEOUT = "0003";
    public const string NETWORK_ERROR = "0004";
    
    // 인증 (1xxx)
    public const string AUTH_FAILED = "1001";
    public const string SESSION_EXPIRED = "1002";
    
    // 상점 (2xxx)
    public const string SHOP_INSUFFICIENT_CURRENCY = "2001";
    public const string SHOP_LIMIT_EXCEEDED = "2002";
    public const string SHOP_PRODUCT_NOT_FOUND = "2003";
    public const string SHOP_EVENT_ENDED = "2004";
    
    // 가챠 (3xxx)
    public const string GACHA_NOT_ENOUGH_CURRENCY = "3001";
    public const string GACHA_POOL_NOT_FOUND = "3002";
    
    // 스테이지 (4xxx)
    public const string STAGE_NOT_UNLOCKED = "4001";
    public const string STAGE_STAMINA_NOT_ENOUGH = "4002";
    
    // 데이터 (5xxx)
    public const string DATA_SAVE_FAILED = "5001";
    public const string DATA_LOAD_FAILED = "5002";
    public const string DATA_MIGRATION_FAILED = "5003";
    public const string DATA_CORRUPTED = "5004";
}
```

#### Result<T> 패턴
```csharp
public readonly struct Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T Value { get; }
    public string ErrorCode { get; }
    
    public static Result<T> Success(T value);
    public static Result<T> Failure(string errorCode);
    
    public Result<U> Map<U>(Func<T, U> mapper);
    public Result<T> OnSuccess(Action<T> action);
    public Result<T> OnFailure(Action<string> action);
}
```

### 0.3 세이브 시스템 (Save)

**필요 이유**: 안전한 저장/로드, 버전 마이그레이션

#### 산출물

| 파일 | 위치 | 역할 |
|------|------|------|
| `ISaveStorage.cs` | Core/Interfaces/ | 저장소 인터페이스 |
| `FileSaveStorage.cs` | Core/Services/ | 파일 기반 저장소 |
| `SaveManager.cs` | Core/Managers/ | 저장/로드 관리 |
| `ISaveMigration.cs` | Core/Interfaces/ | 마이그레이션 인터페이스 |
| `SaveMigrator.cs` | Core/Services/ | 마이그레이션 실행 |

#### SaveManager API
```csharp
public class SaveManager : Singleton<SaveManager>
{
    public int CurrentVersion { get; }
    
    // 저장/로드
    public Result<bool> Save(UserSaveData data);
    public Result<UserSaveData> Load();
    
    // 마이그레이션
    public bool NeedsMigration(UserSaveData data);
    public UserSaveData Migrate(UserSaveData data);
    
    // 자동 저장
    public void EnableAutoSave(float intervalSeconds);
    public void DisableAutoSave();
}
```

#### 마이그레이션 체인
```csharp
public interface ISaveMigration
{
    int FromVersion { get; }
    int ToVersion { get; }
    UserSaveData Migrate(UserSaveData data);
}
```

### 0.4 로딩 UI (LoadingIndicator)

**필요 이유**: 네트워크 요청 중 화면 차단, UX 개선

#### 산출물

| 파일 | 위치 | 역할 |
|------|------|------|
| `LoadingIndicator.cs` | Common/UI/ | 로딩 관리 (싱글톤) |
| `LoadingWidget.cs` | Common/UI/Widgets/ | 로딩 UI 위젯 |

#### LoadingIndicator API
```csharp
public class LoadingIndicator : Singleton<LoadingIndicator>
{
    // 기본 사용
    public void Show();
    public void Hide();
    
    // 메시지 포함
    public void Show(string message);
    
    // 타임아웃 자동 해제
    public void Show(float timeoutSeconds);
    
    // 스코프 기반 (using 패턴)
    public IDisposable Scope(string message = null);
}
```

#### 사용 예시
```csharp
// using 패턴
using (LoadingIndicator.Instance.Scope("구매 중..."))
{
    var response = await _apiClient.PurchaseAsync(request);
}
// 자동 Hide

// 수동 제어
LoadingIndicator.Instance.Show();
try { ... }
finally { LoadingIndicator.Instance.Hide(); }
```

### 0.5 체크리스트

```
Phase 0 체크리스트:

로깅 시스템:
- [ ] LogLevel.cs 생성
- [ ] Log.cs 생성 (정적 API)
- [ ] ILogOutput.cs 생성
- [ ] UnityLogOutput.cs 생성
- [ ] LogConfig.cs 생성 (ScriptableObject)
- [ ] 에디터 로그 필터 UI (선택)

에러 처리:
- [ ] ErrorCode.cs 생성
- [ ] ErrorMessages.cs 생성
- [ ] Result.cs 생성

세이브 시스템:
- [ ] ISaveStorage.cs 생성
- [ ] FileSaveStorage.cs 생성
- [ ] SaveManager.cs 생성
- [ ] ISaveMigration.cs 생성
- [ ] SaveMigrator.cs 생성
- [ ] UserSaveData에 Version 필드 추가

로딩 UI:
- [ ] LoadingIndicator.cs 생성
- [ ] LoadingWidget.cs 생성
- [ ] Loading 프리팹 생성
- [ ] MVPSceneSetup에 LoadingIndicator 추가
```

---

## Phase 1: 공통 모듈

> **목표**: 여러 기능에서 공유하는 기반 구조 구축
> **상태**: ✅ 설계 완료

### 1.1 보상 시스템 (Reward)

**필요 이유**: 가챠, 상점, 스테이지, 이벤트 모두 "보상 지급" 필요

#### 산출물

| 파일 | 위치 | 역할 |
|------|------|------|
| `RewardType.cs` | Data/Enums/ | 보상 타입 열거형 (4개) |
| `ItemCategory.cs` | Data/Enums/ | 아이템 세부 분류 (6개) |
| `RewardInfo.cs` | Data/Structs/Common/ | 보상 정보 구조체 |
| `RewardHelper.cs` | Core/Utility/ | UI 헬퍼 (포맷팅, 아이콘) |

#### RewardType 정의 (확정)
```csharp
public enum RewardType
{
    Currency,       // 재화 (골드, 젬, 스태미나, 이벤트 코인)
    Item,           // 모든 인벤토리 아이템
    Character,      // 캐릭터 획득
    PlayerExp,      // 플레이어 경험치
}

public enum ItemCategory
{
    Equipment,      // 장비 (장착 전, 수량 기반)
    Consumable,     // 소모품 (경험치 아이템, 버프 등)
    Material,       // 재료 (강화, 진화, 돌파)
    CharacterShard, // 캐릭터 조각
    Furniture,      // 가구
    Ticket,         // 소환 티켓
}
```

#### RewardInfo 구조체 (확정)
```csharp
[Serializable]
public struct RewardInfo
{
    public RewardType Type;
    public string ItemId;
    public int Amount;
}
```

#### RewardHelper 역할 (클라이언트)
- `FormatText(RewardInfo)` → UI 표시용 텍스트
- `GetIconPath(RewardInfo)` → 아이콘 경로
- `GetRarityColor(RewardInfo)` → 희귀도 색상

### 1.2 서버/클라이언트 분리 (Sc.LocalServer)

**필요 이유**: 서버 로직과 클라이언트 코드 명확 분리

#### 새 Assembly 구조
```
Sc.LocalServer (서버 모사 영역)
├── LocalGameServer.cs
├── Handlers/
│   ├── LoginHandler.cs
│   ├── GachaHandler.cs
│   ├── ShopHandler.cs
│   └── StageHandler.cs
├── Validators/
│   └── ServerValidator.cs
└── Services/
    ├── RewardService.cs    # Delta 생성
    └── GachaService.cs

Sc.Core (클라이언트)
├── ResponseValidator.cs    # 2차 검증 (요청-응답 일관성)
└── Utility/
    └── RewardHelper.cs
```

### 1.3 범용 팝업 (SystemPopup)

**필요 이유**: 확인, 알림, 입력, 재화 소비 등 공통 UI 패턴

#### 산출물 (하이브리드 구조)

| 파일 | 위치 | 역할 |
|------|------|------|
| `SystemPopupBase.cs` | Common/UI/Popups/ | 추상 베이스 |
| `ConfirmPopup.cs` | Common/UI/Popups/ | 확인/취소 팝업 |
| `AlertPopup.cs` | Common/UI/Popups/ | 알림 팝업 (Info/Warning/Error) |
| `InputPopup.cs` | Common/UI/Popups/ | 텍스트 입력 팝업 |
| `CostConfirmPopup.cs` | Common/UI/Popups/ | 재화 소비 확인 팝업 |
| `ButtonStyle.cs` | Common/UI/Popups/ | 버튼 스타일 (Default/Primary/Danger/Secondary) |

### 1.4 보상 팝업 (RewardPopup)

**필요 이유**: 보상 획득 시 시각적 표시

#### 산출물 (좌우 스크롤 카드형)

| 파일 | 위치 | 역할 |
|------|------|------|
| `RewardPopup.cs` | Common/UI/Popups/ | 메인 (좌우 스크롤) |
| `RewardFullViewPopup.cs` | Common/UI/Popups/ | 전체 보기 (그리드) |
| `RewardCard.cs` | Common/UI/Popups/ | 개별 카드 위젯 |

#### 레이아웃
```
┌─────────────────────────────────────────┐
│              획득 보상 (3/7)             │
├─────────────────────────────────────────┤
│        ┌─────────────────────┐          │
│   ◀    │   [카드 내용]       │    ▶    │
│        └─────────────────────┘          │
│              ● ○ ○ ○ ○ ○ ○              │
├─────────────────────────────────────────┤
│      [전체 보기]       [확인]            │
└─────────────────────────────────────────┘
```

#### 정렬/표시 규칙
- 정렬: 희귀도 순 (캐릭터 > 장비 > 재료 > 재화)
- 전체 보기: 4열 그리드 서브 팝업
- 신규 아이템: NEW 뱃지 + 금색 프레임

### 1.5 시간 처리 (TimeService)

**필요 이유**: 구매 제한, 이벤트, 출석 등 여러 시스템에서 시간 기반 로직 필요

#### 설계 원칙
- **저장/계산**: 항상 UTC (Unix timestamp)
- **UI 표시**: 로컬 시간 변환
- **리셋 기준**: UTC 0시

#### 산출물

| 파일 | 위치 | 역할 |
|------|------|------|
| `ITimeService.cs` | Core/Interfaces/ | 시간 서비스 인터페이스 |
| `TimeService.cs` | Core/Services/ | 클라이언트 시간 서비스 |
| `ServerTimeService.cs` | LocalServer/Services/ | 서버 시간 서비스 |
| `TimeHelper.cs` | Core/Utility/ | UI 표시용 헬퍼 |

#### ITimeService 인터페이스
```csharp
public interface ITimeService
{
    // 현재 서버 시간 (UTC, Unix timestamp)
    long ServerTimeUtc { get; }
    DateTime ServerDateTime { get; }
    
    // 리셋 시간 계산
    long GetNextResetTime(LimitType limitType);
    
    // 리셋 여부 체크
    bool HasResetOccurred(long lastTimestamp, LimitType limitType);
    
    // 이벤트 기간 체크
    bool IsWithinPeriod(long startTime, long endTime);
}
```

#### TimeHelper (UI 표시)
```csharp
public static class TimeHelper
{
    // UTC → 로컬 시간 문자열
    public static string ToLocalTimeString(long utcTimestamp, string format = "yyyy-MM-dd HH:mm");
    
    // 남은 시간 표시 (예: "2시간 30분", "1일 5시간")
    public static string FormatRemainingTime(long targetUtcTimestamp);
    
    // 리셋까지 남은 시간
    public static string FormatTimeUntilReset(LimitType limitType, ITimeService timeService);
}
```

#### 리셋 시간 규칙
| LimitType | 리셋 시점 |
|-----------|----------|
| Daily | 매일 UTC 00:00 |
| Weekly | 매주 월요일 UTC 00:00 |
| Monthly | 매월 1일 UTC 00:00 |
| Permanent | 리셋 없음 |
| EventPeriod | 이벤트 종료 시 (별도 관리) |

### 1.6 체크리스트

```
Phase 1 체크리스트:

보상 시스템:
- [ ] RewardType.cs 생성
- [ ] ItemCategory.cs 생성
- [ ] RewardInfo.cs 생성
- [ ] RewardHelper.cs 생성

서버/클라 분리:
- [ ] Sc.LocalServer Assembly 생성
- [ ] LocalGameServer.cs 생성
- [ ] RewardService.cs 생성 (Delta 생성)
- [ ] ResponseValidator.cs 생성 (2차 검증)
- [ ] 기존 LocalApiClient 리팩토링

시간 처리:
- [ ] ITimeService.cs 생성
- [ ] TimeService.cs 생성 (클라이언트)
- [ ] ServerTimeService.cs 생성 (Sc.LocalServer)
- [ ] TimeHelper.cs 생성 (UI 표시)

시스템 팝업:
- [ ] SystemPopupBase.cs 생성
- [ ] ButtonStyle.cs 생성
- [ ] ConfirmPopup.cs 생성
- [ ] AlertPopup.cs 생성
- [ ] InputPopup.cs 생성
- [ ] CostConfirmPopup.cs 생성

보상 팝업:
- [ ] RewardCard.cs 생성
- [ ] RewardPopup.cs 생성
- [ ] RewardFullViewPopup.cs 생성

프리팹/씬:
- [ ] MVPSceneSetup에 팝업 프리팹 추가
- [ ] 각 팝업별 프리팹 생성
```

### 1.7 상세 스펙 문서
- [Reward.md](../Specs/Common/Reward.md)
- [SystemPopup.md](../Specs/Common/Popups/SystemPopup.md)
- [RewardPopup.md](../Specs/Common/Popups/RewardPopup.md)

---

## Phase 2: 상점 (Shop)

> **목표**: 재화 소비 → 상품 구매 → 보상 획득 흐름 완성
> **상태**: 📝 설계 검토 중

### 2.1 마스터 데이터

#### 산출물

| 파일 | 위치 | 역할 |
|------|------|------|
| `ProductType.cs` | Data/Enums/ | 상품 타입 열거형 |
| `LimitType.cs` | Data/Enums/ | 구매 제한 타입 |
| `CurrencyIds.cs` | Data/Constants/ | 재화 ID 상수 |
| `ShopProductData.cs` | Data/ScriptableObjects/ | 상품 SO 정의 |
| `ShopProductDatabase.cs` | Data/ScriptableObjects/ | 상품 DB |
| `ShopProduct.json` | Data/MasterData/ | 샘플 데이터 |

#### ProductType 정의
```csharp
public enum ProductType
{
    Currency,   // 재화 교환 (젬 → 골드)
    Item,       // 단일 아이템 (장비, 재료, 조각, 티켓 등 - ItemCategory로 구분)
    Package,    // 복합 패키지 (여러 아이템 묶음)
    Character,  // 캐릭터 직접 판매 (스킨 포함)
}
```

#### 소비 재화 (CostCurrencyId)

> CostType enum 대신 **string Id** 사용 (이벤트마다 다른 코인 대응)

```csharp
// 예약된 CurrencyId
public static class CurrencyIds
{
    public const string Free = "";           // 무료 (빈 문자열)
    public const string Gold = "gold";
    public const string Gem = "gem";
    public const string Stamina = "stamina";
    // 이벤트 코인: "event_{eventId}_coin" 형식
}
```

#### LimitType 정의
```csharp
public enum LimitType
{
    None,           // 무제한
    Daily,          // 일일 (매일 UTC 0시 리셋)
    Weekly,         // 주간 (매주 월요일 UTC 0시)
    Monthly,        // 월간 (매월 1일 UTC 0시)
    Permanent,      // 영구 (계정당 1회, 리셋 없음)
    EventPeriod,    // 이벤트 기간 (이벤트 종료 시 리셋)
}
```

#### ShopProductData 필드
```csharp
[CreateAssetMenu]
public class ShopProductData : ScriptableObject
{
    public string Id;
    public string Name;
    public string Description;
    public ProductType ProductType;
    public string CostCurrencyId;       // "" = 무료, "gold", "gem", "event_xxx_coin"
    public int Price;
    public RewardInfo[] Rewards;        // Phase 1의 RewardInfo 사용
    public LimitType LimitType;
    public int LimitCount;
    public int DisplayOrder;
    public string IconPath;
    public bool IsHot;
    public bool IsNew;
}
```

### 2.2 유저 데이터

#### UserSaveData 확장
```csharp
// UserSaveData.cs에 추가
public List<ShopPurchaseRecord> ShopPurchaseHistory;

[Serializable]
public struct ShopPurchaseRecord
{
    public string ProductId;
    public int PurchaseCount;
    public long LastPurchaseAt;     // 마지막 구매 시각 (UTC timestamp)
    // ResetAt 제거 → TimeService.HasResetOccurred()로 판단
}
```

> **리셋 판단 로직**: `TimeService.HasResetOccurred(record.LastPurchaseAt, product.LimitType)`

### 2.3 Request/Response

#### ShopPurchaseRequest
```csharp
[Serializable]
public struct ShopPurchaseRequest : IRequest
{
    public string ProductId;
    public int Amount;              // 구매 수량 (기본 1)
    public long Timestamp { get; set; }
}
```

#### ShopPurchaseResponse
```csharp
[Serializable]
public struct ShopPurchaseResponse : IResponse, IGameActionResponse
{
    public bool IsSuccess { get; set; }
    public string ErrorCode { get; set; }
    public long ServerTime { get; set; }
    public UserDataDelta Delta { get; set; }
    public RewardInfo[] Rewards;    // 획득 보상 목록
}
```

### 2.4 이벤트

#### ShopEvents.cs
```csharp
public struct ProductPurchasedEvent
{
    public string ProductId;
    public RewardInfo[] Rewards;
}

public struct PurchaseFailedEvent
{
    public string ProductId;
    public string ErrorCode;
}
```

### 2.5 서버 로직 (Sc.LocalServer)

> Phase 1에서 확정된 Sc.LocalServer 구조 활용

#### ShopHandler.cs (Sc.LocalServer/Handlers/)
```csharp
public class ShopHandler
{
    public ShopPurchaseResponse HandlePurchase(ShopPurchaseRequest request, UserSaveData userData)
    {
        // 1. 상품 조회
        // 2. 구매 제한 검증
        // 3. 재화 검증 (CostType, Price)
        // 4. 재화 차감
        // 5. 보상 지급 (RewardService 활용)
        // 6. 구매 기록 저장
        // 7. Delta 생성 및 반환
    }
}
```

#### ShopService.cs (Sc.LocalServer/Services/)
```csharp
public class ShopService
{
    // 구매 제한 체크
    public bool CanPurchase(string productId, UserSaveData userData);
    
    // 리셋 시간 계산
    public long CalculateResetTime(LimitType limitType);
    
    // 구매 기록 업데이트
    public void UpdatePurchaseRecord(string productId, UserSaveData userData);
}
```

### 2.6 UI

#### 산출물

| 파일 | 위치 | 역할 |
|------|------|------|
| `ShopScreen.cs` | Contents/OutGame/Shop/ | 상점 메인 화면 |
| `ShopProductItem.cs` | Contents/OutGame/Shop/ | 상품 아이템 위젯 |
| `ShopTabGroup.cs` | Contents/OutGame/Shop/ | 탭 그룹 |

> 구매 확인은 Phase 1의 CostConfirmPopup 재사용

#### ShopScreen 구조
```
[ShopScreen]
├── ScreenHeader (공통)
├── ShopTabGroup (재화/아이템/패키지)
├── ProductScrollView
│   └── ShopProductItem[] (동적 생성)
└── CurrencyHUD (공통)
```

### 2.7 체크리스트

```
Phase 2 체크리스트:

마스터 데이터:
- [ ] ProductType.cs 생성
- [ ] LimitType.cs 생성
- [ ] CurrencyIds.cs 생성 (재화 ID 상수)
- [ ] ShopProductData.cs 생성
- [ ] ShopProductDatabase.cs 생성
- [ ] ShopProduct.json 샘플 데이터
- [ ] MasterDataImporter에 ShopProduct 추가

유저 데이터:
- [ ] ShopPurchaseRecord 구조체 추가
- [ ] UserSaveData 버전 마이그레이션

서버 로직 (Sc.LocalServer):
- [ ] ShopHandler.cs 생성
- [ ] ShopService.cs 생성
- [ ] LocalGameServer에 ShopHandler 등록

Request/Response:
- [ ] ShopPurchaseRequest 생성
- [ ] ShopPurchaseResponse 생성

이벤트:
- [ ] ShopEvents.cs 생성

UI:
- [ ] ShopScreen.cs 생성
- [ ] ShopProductItem.cs 생성
- [ ] ShopTabGroup.cs 생성
- [ ] MVPSceneSetup에 Shop 프리팹 추가

연동:
- [ ] LobbyScreen에 상점 버튼 추가
- [ ] NetworkManager 연동 테스트
```

### 2.8 상세 스펙 문서
- [Shop.md](../Specs/Shop.md)
- [ShopProductData.md](../Specs/Shop/ShopProductData.md)
- [ShopScreen.md](../Specs/Shop/ShopScreen.md)

---

## Phase 3: 스테이지 진입 (Stage Entry)

> **목표**: 아웃게임 → 인게임 브릿지 완성 (전투 시작 직전까지)

### 3.1 마스터 데이터 확장

#### StageData 확장 필드
```csharp
// 기존 StageData.cs에 추가
public StageType StageType;             // 스테이지 타입
public RewardInfo[] FirstClearRewards;  // 초회 클리어 보상
public StageUnlockCondition UnlockCondition; // 해금 조건

[Serializable]
public struct StageUnlockCondition
{
    public string RequiredStageId;      // 선행 스테이지
    public int RequiredPlayerLevel;     // 필요 레벨
}
```

#### StageType 정의
```csharp
public enum StageType
{
    Main,           // 메인 스토리
    Daily,          // 일일 던전
    Weekly,         // 주간 던전
    Event,          // 이벤트 스테이지
    Challenge,      // 챌린지
}
```

### 3.2 유저 데이터

#### PartyPreset 추가
```csharp
// UserSaveData.cs에 추가
public List<PartyPreset> PartyPresets;

[Serializable]
public struct PartyPreset
{
    public int SlotIndex;               // 프리셋 슬롯 (0~4)
    public string[] CharacterInstanceIds; // 캐릭터 4명
}
```

### 3.3 Request/Response

#### StageBattleRequest
```csharp
[Serializable]
public struct StageBattleRequest : IRequest
{
    public string StageId;
    public string[] PartyCharacterIds;  // 출전 캐릭터 InstanceId 배열
    public long Timestamp { get; set; }
}
```

#### StageBattleResponse
```csharp
[Serializable]
public struct StageBattleResponse : IResponse
{
    public bool IsSuccess { get; set; }
    public string ErrorCode { get; set; }
    public long ServerTime { get; set; }

    public string BattleId;             // 전투 세션 ID
    public BattleInitialData InitialData;
}

[Serializable]
public struct BattleInitialData
{
    public CharacterBattleData[] PlayerParty;
    public CharacterBattleData[] EnemyParty;
    public string[] TurnOrder;          // 행동 순서
}

[Serializable]
public struct CharacterBattleData
{
    public string InstanceId;
    public string CharacterId;
    public int Level;
    public int CurrentHp;
    public int MaxHp;
    public int Atk;
    public int Def;
    public int Spd;
    public string[] SkillIds;
}
```

### 3.4 이벤트

#### StageEvents.cs
```csharp
public struct StageSelectedEvent
{
    public string StageId;
}

public struct BattleStartRequestedEvent
{
    public string StageId;
    public string[] PartyCharacterIds;
}

public struct BattleReadyEvent
{
    public string BattleId;
    public BattleInitialData InitialData;
}
```

### 3.5 API 구현

#### LocalApiClient 확장
```csharp
public async UniTask<StageBattleResponse> StartBattleAsync(StageBattleRequest request)
{
    // 1. 스테이지 조회
    // 2. 해금 조건 검증
    // 3. 스태미나 검증 및 차감
    // 4. 파티 검증 (4명, 보유 여부)
    // 5. 적 파티 생성
    // 6. 전투 초기 데이터 생성
    // 7. BattleId 발급
    // 8. 응답 반환
}
```

### 3.6 UI

#### 산출물

| 파일 | 위치 | 역할 |
|------|------|------|
| `StageListScreen.cs` | Contents/OutGame/Stage/ | 스테이지 선택 화면 |
| `StageItem.cs` | Contents/OutGame/Stage/ | 스테이지 아이템 위젯 |
| `StageInfoPopup.cs` | Contents/OutGame/Stage/ | 스테이지 정보 팝업 |
| `PartySelectScreen.cs` | Contents/OutGame/Stage/ | 파티 편성 화면 |
| `PartySlotWidget.cs` | Contents/OutGame/Stage/ | 파티 슬롯 위젯 |

#### StageListScreen 구조
```
[StageListScreen]
├── ScreenHeader
├── ChapterTabGroup
├── StageScrollView
│   └── StageItem[] (동적 생성)
│       ├── StageName
│       ├── Stars (0~3)
│       └── LockIcon
└── SelectedStageInfo
    ├── StageName
    ├── RecommendedPower
    ├── Rewards Preview
    └── [출전하기] 버튼
```

#### PartySelectScreen 구조
```
[PartySelectScreen]
├── ScreenHeader
├── StageInfoHeader
│   ├── StageName
│   └── RecommendedPower
├── PartySlots (4개)
│   └── PartySlotWidget
│       ├── CharacterIcon
│       ├── Level
│       └── [X] 제거 버튼
├── CharacterScrollView
│   └── SelectableCharacterItem[]
├── TotalPowerDisplay
└── [전투 시작] 버튼
```

### 3.7 체크리스트

```
Phase 3 체크리스트:

마스터 데이터:
- [ ] StageType.cs 생성
- [ ] StageUnlockCondition 구조체 추가
- [ ] StageData.cs 확장 (FirstClearRewards, StageType, UnlockCondition)
- [ ] Stage.json 샘플 데이터 업데이트

유저 데이터:
- [ ] PartyPreset 구조체 추가
- [ ] UserSaveData에 PartyPresets 필드 추가
- [ ] StageProgress 확장 (별점 저장 구조 개선)

Request/Response:
- [ ] StageBattleRequest.cs 생성
- [ ] StageBattleResponse.cs 생성
- [ ] BattleInitialData.cs 생성
- [ ] CharacterBattleData.cs 생성

API:
- [ ] LocalApiClient.StartBattleAsync 구현
- [ ] 스태미나 차감 로직
- [ ] 적 파티 생성 로직

이벤트:
- [ ] StageEvents.cs 생성

UI:
- [ ] Sc.Contents.Stage Assembly 생성
- [ ] StageListScreen.cs 생성
- [ ] StageItem.cs 생성
- [ ] StageInfoPopup.cs 생성
- [ ] PartySelectScreen.cs 생성
- [ ] PartySlotWidget.cs 생성
- [ ] SelectableCharacterItem.cs 생성
- [ ] MVPSceneSetup에 Stage 프리팹 추가

연동:
- [ ] LobbyScreen에 스테이지 버튼 추가
- [ ] NetworkManager 연동
- [ ] Battle 시스템 진입 이벤트 연결 (Phase 5+)
```

### 3.8 상세 스펙 문서
- [Stage.md](../Specs/Stage.md)
- [StageListScreen.md](../Specs/Stage/StageListScreen.md)
- [PartySelectScreen.md](../Specs/Stage/PartySelectScreen.md)

---

## Phase 4: 라이브 이벤트 (LiveEvent)

> **목표**: 기간제 컨텐츠 컨테이너 구축

### 4.1 마스터 데이터

#### 산출물

| 파일 | 위치 | 역할 |
|------|------|------|
| `EventType.cs` | Data/Enums/ | 이벤트 타입 |
| `LiveEventData.cs` | Data/ScriptableObjects/ | 이벤트 SO 정의 |
| `LiveEventDatabase.cs` | Data/ScriptableObjects/ | 이벤트 DB |
| `EventMissionData.cs` | Data/ScriptableObjects/ | 이벤트 미션 SO |
| `LiveEvent.json` | Data/MasterData/ | 샘플 데이터 |

#### EventType 정의
```csharp
public enum EventType
{
    Mission,        // 미션 달성형
    Shop,           // 이벤트 상점
    Stage,          // 이벤트 스테이지
    Login,          // 출석 이벤트
    Collection,     // 수집 이벤트
    Composite,      // 복합형 (미션 + 상점 + 스테이지)
}
```

#### LiveEventData 필드
```csharp
[CreateAssetMenu]
public class LiveEventData : ScriptableObject
{
    public string Id;
    public string Name;
    public string Description;
    public EventType EventType;
    public long StartAt;                // Unix timestamp
    public long EndAt;
    public string BannerImagePath;
    public string DetailImagePath;

    // 이벤트 컨텐츠 (타입별 선택적)
    public string[] MissionIds;         // EventMissionData 참조
    public string[] ShopProductIds;     // 이벤트 전용 상품
    public string[] StageIds;           // 이벤트 전용 스테이지

    // 이벤트 재화
    public CostType EventCurrencyType;  // 이벤트 전용 재화
}
```

#### EventMissionData 필드
```csharp
[CreateAssetMenu]
public class EventMissionData : ScriptableObject
{
    public string Id;
    public string EventId;              // 소속 이벤트
    public string Description;          // "스테이지 10회 클리어"
    public MissionConditionType ConditionType;
    public string ConditionParam;       // 조건 파라미터
    public int RequiredCount;           // 목표 횟수
    public RewardInfo[] Rewards;        // 완료 보상
    public int DisplayOrder;
}

public enum MissionConditionType
{
    ClearStage,         // 스테이지 클리어
    ClearStageCount,    // 스테이지 N회 클리어
    GachaCount,         // 가챠 N회
    PurchaseCount,      // 구매 N회
    LoginCount,         // 출석 N일
    CollectItem,        // 아이템 수집
    // ... 확장 가능
}
```

### 4.2 유저 데이터

#### LiveEventProgress 추가
```csharp
// UserSaveData.cs에 추가
public List<LiveEventProgress> EventProgress;

[Serializable]
public struct LiveEventProgress
{
    public string EventId;
    public List<EventMissionProgress> Missions;
}

[Serializable]
public struct EventMissionProgress
{
    public string MissionId;
    public int CurrentCount;
    public bool IsCompleted;
    public bool IsRewardClaimed;
}
```

### 4.3 Request/Response

#### GetActiveEventsRequest/Response
```csharp
[Serializable]
public struct GetActiveEventsRequest : IRequest
{
    public long Timestamp { get; set; }
}

[Serializable]
public struct GetActiveEventsResponse : IResponse
{
    public bool IsSuccess { get; set; }
    public string ErrorCode { get; set; }
    public long ServerTime { get; set; }

    public LiveEventInfo[] ActiveEvents;
}

[Serializable]
public struct LiveEventInfo
{
    public string EventId;
    public string Name;
    public long EndAt;
    public string BannerImagePath;
}
```

#### ClaimEventMissionRequest/Response
```csharp
[Serializable]
public struct ClaimEventMissionRequest : IRequest
{
    public string EventId;
    public string MissionId;
    public long Timestamp { get; set; }
}

[Serializable]
public struct ClaimEventMissionResponse : IResponse, IGameActionResponse
{
    public bool IsSuccess { get; set; }
    public string ErrorCode { get; set; }
    public long ServerTime { get; set; }
    public UserDataDelta Delta { get; set; }

    public RewardInfo[] Rewards;
}
```

### 4.4 이벤트

#### LiveEventEvents.cs
```csharp
public struct EventStartedEvent
{
    public string EventId;
}

public struct EventEndedEvent
{
    public string EventId;
}

public struct EventMissionProgressedEvent
{
    public string EventId;
    public string MissionId;
    public int CurrentCount;
    public int RequiredCount;
}

public struct EventMissionCompletedEvent
{
    public string EventId;
    public string MissionId;
}

public struct EventRewardClaimedEvent
{
    public string EventId;
    public string MissionId;
    public RewardInfo[] Rewards;
}
```

### 4.5 UI

#### 산출물

| 파일 | 위치 | 역할 |
|------|------|------|
| `EventDashboardScreen.cs` | Contents/OutGame/Event/ | 이벤트 대시보드 |
| `EventBannerItem.cs` | Contents/OutGame/Event/ | 배너 아이템 |
| `EventDetailScreen.cs` | Contents/OutGame/Event/ | 이벤트 상세 |
| `EventMissionItem.cs` | Contents/OutGame/Event/ | 미션 아이템 |

#### EventDashboardScreen 구조
```
[EventDashboardScreen]
├── ScreenHeader
├── BannerScrollView (가로 스크롤)
│   └── EventBannerItem[]
│       ├── BannerImage
│       ├── EventName
│       ├── RemainingTime
│       └── [진입] 버튼
└── NoEventMessage (이벤트 없을 때)
```

#### EventDetailScreen 구조
```
[EventDetailScreen]
├── ScreenHeader
├── EventBannerImage
├── EventDescription
├── TabGroup (미션/상점/스테이지)
├── ContentArea
│   ├── MissionList
│   │   └── EventMissionItem[]
│   │       ├── Description
│   │       ├── Progress (n/m)
│   │       └── [보상 받기] 버튼
│   ├── EventShopList (상점 탭)
│   └── EventStageList (스테이지 탭)
└── EventCurrencyDisplay
```

### 4.6 체크리스트

```
Phase 4 체크리스트:

마스터 데이터:
- [ ] EventType.cs 생성
- [ ] MissionConditionType.cs 생성
- [ ] LiveEventData.cs 생성
- [ ] LiveEventDatabase.cs 생성
- [ ] EventMissionData.cs 생성
- [ ] EventMissionDatabase.cs 생성
- [ ] LiveEvent.json 샘플 데이터
- [ ] EventMission.json 샘플 데이터
- [ ] MasterDataImporter에 LiveEvent, EventMission 추가

유저 데이터:
- [ ] LiveEventProgress 구조체 추가
- [ ] EventMissionProgress 구조체 추가
- [ ] UserSaveData에 EventProgress 필드 추가

Request/Response:
- [ ] GetActiveEventsRequest.cs 생성
- [ ] GetActiveEventsResponse.cs 생성
- [ ] ClaimEventMissionRequest.cs 생성
- [ ] ClaimEventMissionResponse.cs 생성

API:
- [ ] LocalApiClient.GetActiveEventsAsync 구현
- [ ] LocalApiClient.ClaimEventMissionAsync 구현
- [ ] 이벤트 기간 검증 로직
- [ ] 미션 진행도 업데이트 로직

이벤트:
- [ ] LiveEventEvents.cs 생성

UI:
- [ ] Sc.Contents.Event Assembly 생성
- [ ] EventDashboardScreen.cs 생성
- [ ] EventBannerItem.cs 생성
- [ ] EventDetailScreen.cs 생성
- [ ] EventMissionItem.cs 생성
- [ ] MVPSceneSetup에 Event 프리팹 추가

연동:
- [ ] LobbyScreen에 이벤트 버튼 추가
- [ ] 이벤트 상점 연동 (ShopScreen 재사용)
- [ ] 이벤트 스테이지 연동 (StageListScreen 재사용)
```

### 4.7 상세 스펙 문서
- [LiveEvent.md](../Specs/LiveEvent.md) (기존 확장)
- [EventDashboardScreen.md](../Specs/LiveEvent/EventDashboardScreen.md)
- [EventDetailScreen.md](../Specs/LiveEvent/EventDetailScreen.md)

---

## Phase 5: 기존 기능 강화

> **목표**: MVP 완성도 향상

### 5.1 가챠 강화

#### 체크리스트
```
가챠 강화:
- [ ] 배너별 UI (GachaBannerItem)
- [ ] 소환 연출 애니메이션 (GachaAnimator)
- [ ] 가챠 히스토리 화면 (GachaHistoryScreen)
- [ ] 천장 카운트 표시
- [ ] 픽업 캐릭터 표시
```

### 5.2 캐릭터 강화

#### 체크리스트
```
캐릭터 강화:
- [ ] 필터링 시스템 (희귀도, 클래스, 속성)
- [ ] 정렬 시스템 (레벨, 획득일, 희귀도, 전투력)
- [ ] 캐릭터 레벨업 화면 (CharacterLevelUpScreen)
- [ ] 캐릭터 돌파 화면 (CharacterAscensionScreen)
- [ ] 장비 장착 화면 (CharacterEquipScreen)
```

### 5.3 NavigationManager 강화

#### 체크리스트
```
Navigation 강화:
- [ ] Shortcut API (직접 특정 화면 이동)
- [ ] DeepLink 지원 (외부 링크 → 특정 화면)
- [ ] 탭 그룹 관리 (로비 내 탭 전환)
- [ ] 화면 히스토리 관리 개선
```

### 5.4 상세 스펙 문서
- [GachaEnhancement.md](../Specs/Gacha/Enhancement.md)
- [CharacterEnhancement.md](../Specs/Character/Enhancement.md)
- [NavigationEnhancement.md](../Specs/Common/NavigationEnhancement.md)

---

## 검증 시나리오

### 시나리오 1: 상점 구매 흐름
```
1. 로비 → [상점] 버튼 클릭
2. ShopScreen 표시
3. 상품 탭 선택 (재화/패키지/패스)
4. 상품 아이템 클릭
5. PurchaseConfirmPopup 표시
6. [구매] 버튼 클릭
7. NetworkManager.Send(ShopPurchaseRequest)
8. 응답 수신 → DataManager.ApplyDelta
9. RewardPopup 표시
10. CurrencyHUD 갱신 확인
```

### 시나리오 2: 스테이지 진입 흐름
```
1. 로비 → [스테이지] 버튼 클릭
2. StageListScreen 표시
3. 챕터 선택
4. 스테이지 아이템 클릭
5. StageInfoPopup 표시 (보상, 추천 전투력)
6. [출전하기] 버튼 클릭
7. PartySelectScreen 표시
8. 캐릭터 4명 선택
9. [전투 시작] 버튼 클릭
10. NetworkManager.Send(StageBattleRequest)
11. 응답 수신 → BattleReadyEvent 발행
12. (Battle 시스템 진입 대기)
```

### 시나리오 3: 이벤트 미션 보상 수령
```
1. 로비 → [이벤트] 버튼 클릭
2. EventDashboardScreen 표시
3. 이벤트 배너 클릭
4. EventDetailScreen 표시
5. 미션 탭에서 완료된 미션 확인
6. [보상 받기] 버튼 클릭
7. NetworkManager.Send(ClaimEventMissionRequest)
8. 응답 수신 → DataManager.ApplyDelta
9. RewardPopup 표시
10. 미션 상태 "수령 완료"로 변경
```

---

## 산출물 요약

### 새로 생성할 파일 (총 ~50개)

#### Data 레이어 (~15개)
- Enums: RewardType, ProductType, LimitType, StageType, EventType, MissionConditionType
- Structs: RewardInfo, ShopPurchaseRecord, PartyPreset, LiveEventProgress, EventMissionProgress
- SO: ShopProductData, ShopProductDatabase, LiveEventData, LiveEventDatabase, EventMissionData, EventMissionDatabase

#### Packet 레이어 (~8개)
- Requests: StageBattleRequest, GetActiveEventsRequest, ClaimEventMissionRequest
- Responses: StageBattleResponse, GetActiveEventsResponse, ClaimEventMissionResponse
- Structs: BattleInitialData, CharacterBattleData

#### Event 레이어 (~4개)
- CommonPopupEvents, ShopEvents, StageEvents, LiveEventEvents

#### Core 레이어 (~1개)
- RewardProcessor

#### Common 레이어 (~2개)
- ConfirmPopup, RewardPopup

#### Contents 레이어 (~20개)
- Shop: ShopScreen, ShopProductItem, PurchaseConfirmPopup
- Stage: StageListScreen, StageItem, StageInfoPopup, PartySelectScreen, PartySlotWidget, SelectableCharacterItem
- Event: EventDashboardScreen, EventBannerItem, EventDetailScreen, EventMissionItem
- (기존 강화 파일들)

### 수정할 파일 (~15개)
- UserSaveData.cs (v3~v4 마이그레이션)
- LocalApiClient.cs (PurchaseAsync, StartBattleAsync, GetActiveEventsAsync 등)
- StageData.cs (확장)
- LobbyScreen.cs (버튼 추가)
- MVPSceneSetup.cs (프리팹 추가)
- MasterDataImporter.cs (새 SO 타입 추가)
- 각종 JSON 샘플 데이터

---

## 진행 상태 추적

| Phase | 상태 | 시작일 | 완료일 |
|-------|------|--------|--------|
| Phase 1: 공통 모듈 | ⬜ 대기 | - | - |
| Phase 2: 상점 | ⬜ 대기 | - | - |
| Phase 3: 스테이지 진입 | ⬜ 대기 | - | - |
| Phase 4: 라이브 이벤트 | ⬜ 대기 | - | - |
| Phase 5: 기존 강화 | ⬜ 대기 | - | - |

---

## 관련 문서

- [PROGRESS.md](../PROGRESS.md) - 전체 진행상황
- [ARCHITECTURE.md](../ARCHITECTURE.md) - 아키텍처 개요
- [CONVENTIONS.md](../CONVENTIONS.md) - 코딩 규칙
- [DOC_RULES.md](../DOC_RULES.md) - 문서 작성 규칙

### Phase별 상세 스펙
- [Phase 1: Reward.md](../Specs/Common/Reward.md)
- [Phase 2: Shop.md](../Specs/Shop.md)
- [Phase 3: Stage.md](../Specs/Stage.md)
- [Phase 4: LiveEvent.md](../Specs/LiveEvent.md)
