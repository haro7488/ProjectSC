---
type: spec
assembly: Sc.Contents.Stage
category: System
status: draft
version: "2.0"
dependencies: [Sc.Common, Sc.Packet, Sc.Data, Sc.Event, Sc.Contents.Character]
created: 2026-01-17
updated: 2026-01-18
---

# Sc.Contents.Stage

## 목적

스테이지 선택, 파티 편성, 전투 시작까지의 아웃게임 → 인게임 브릿지 시스템

## 의존성

### 참조
- `Sc.Common` - UI 시스템, Navigation
- `Sc.Packet` - NetworkManager, Request/Response
- `Sc.Data` - 마스터/유저 데이터
- `Sc.Event` - 이벤트 발행
- `Sc.Contents.Character` - 캐릭터 정보

### 참조됨
- `Sc.Contents.Lobby` - 스테이지 대시보드 진입
- `Sc.Contents.Battle` - 전투 시스템 (Phase 5+)
- `Sc.Contents.Event` - 이벤트 스테이지 (StageListScreen 재사용)

---

## 화면 흐름

```
로비
  │
  ├─────────────────────────────────────────────────┐
  │                                                 │
  ▼                                                 ▼
┌─────────────────────────┐         ┌─────────────────────────┐
│  StageDashboardScreen   │         │   LiveEventScreen       │
│  (상시 컨텐츠)           │         │   (기간 한정 컨텐츠)     │
│                         │         │                         │
│  ├─ 메인 스토리          │         │  ├─ 이벤트 미션         │
│  ├─ 하드 모드            │         │  ├─ 이벤트 상점         │
│  ├─ 일일 던전            │         │  ├─ 이벤트 스테이지 ────┼──┐
│  ├─ 보스 레이드          │         │  └─ 미니게임            │  │
│  └─ 무한의 탑            │         │                         │  │
└───────────┬─────────────┘         └─────────────────────────┘  │
            │                                                    │
            │ 타입 선택                                          │
            ▼                                                    │
┌─────────────────────────┐                                      │
│    StageListScreen      │ ◄────────────────────────────────────┘
│    (재사용 컴포넌트)     │   재사용
│                         │
└───────────┬─────────────┘
            │ 스테이지 선택
            ▼
┌─────────────────────────┐
│    StageInfoPopup       │
│    (스테이지 상세)       │
└───────────┬─────────────┘
            │ 출전하기
            ▼
┌─────────────────────────┐
│   PartySelectScreen     │
│   (재사용 컴포넌트)      │
└───────────┬─────────────┘
            │ 전투 시작
            ▼
        BattleReadyEvent
```

---

## 클래스 역할 정의

### 마스터 데이터

| 클래스 | 역할 | 책임 | 비책임 |
|--------|------|------|--------|
| `StageType` | 스테이지 타입 열거형 | 스테이지 분류 | - |
| `StageData` | 스테이지 SO (확장) | 스테이지 정보 저장 | 전투 로직 |
| `StageUnlockCondition` | 해금 조건 구조체 | 해금 조건 정의 | 해금 검증 |
| `StarCondition` | 별점 조건 구조체 | 별점 획득 조건 정의 | - |

### 유저 데이터

| 클래스 | 역할 | 책임 | 비책임 |
|--------|------|------|--------|
| `StageProgress` | 스테이지 진행 (기존) | 클리어/별점 상태 저장 | 전투 처리 |
| `PartyPreset` | 파티 편성 프리셋 | 편성 저장 | 전투력 계산 |

### Request/Response

| 클래스 | 역할 | 책임 | 비책임 |
|--------|------|------|--------|
| `StageBattleRequest` | 전투 시작 요청 | 스테이지, 파티 정보 전달 | 전투 로직 |
| `StageBattleResponse` | 전투 초기 데이터 응답 | 전투 시작 정보 전달 | 전투 진행 |

### UI

| 클래스 | 역할 | 책임 | 비책임 |
|--------|------|------|--------|
| `StageDashboardScreen` | 스테이지 타입 선택 | 타입별 진입점, 상태 표시 | 스테이지 목록 |
| `StageListScreen` | 스테이지 목록 화면 | 스테이지 선택 | 전투 시작 |
| `StageItem` | 스테이지 아이템 위젯 | 개별 스테이지 표시 | 선택 처리 |
| `StageInfoPopup` | 스테이지 정보 팝업 | 상세 정보 표시 | - |
| `PartySelectScreen` | 파티 편성 화면 | 캐릭터 선택 | 전투 로직 |
| `PartySlotWidget` | 파티 슬롯 위젯 | 슬롯 표시/선택 | - |
| `SelectableCharacterItem` | 선택 가능 캐릭터 | 캐릭터 선택 UI | - |

---

## 상세 정의

### StageType

**위치**: `Assets/Scripts/Data/Enums/StageType.cs`

```csharp
public enum StageType
{
    // === 상시 컨텐츠 (StageDashboard) ===
    MainStory,      // 메인 스토리
    HardMode,       // 하드 모드 (챕터별 노말 완료 시 해금)
    DailyDungeon,   // 일일 던전 (요일별)
    BossRaid,       // 보스 레이드
    Tower,          // 무한의 탑

    // === 기간 한정 (LiveEvent에서 사용) ===
    Event,          // 이벤트 스테이지
}
```

### UnlockConditionType

**위치**: `Assets/Scripts/Data/Enums/UnlockConditionType.cs`

```csharp
public enum UnlockConditionType
{
    None,           // 처음부터 해금
    StageClear,     // 특정 스테이지 클리어
    ChapterClear,   // 챕터 전체 클리어 (하드 해금용)
    PlayerLevel,    // 플레이어 레벨
}
```

### StageUnlockCondition

**위치**: `Assets/Scripts/Data/Structs/MasterData/StageUnlockCondition.cs`

```csharp
[Serializable]
public struct StageUnlockCondition
{
    public UnlockConditionType Type;
    public string TargetId;     // 스테이지ID 또는 챕터ID
    public int RequiredValue;   // 레벨 등
}
```

**예시**:
| 스테이지 | 조건 | 설정 |
|----------|------|------|
| 1-1 | 없음 | `{ None, "", 0 }` |
| 1-2 | 1-1 클리어 | `{ StageClear, "stage_1_1", 0 }` |
| 1-1 Hard | 1장 노말 클리어 | `{ ChapterClear, "chapter_1", 0 }` |
| 일일던전 | Lv.15 이상 | `{ PlayerLevel, "", 15 }` |

### StarConditionType

**위치**: `Assets/Scripts/Data/Enums/StarConditionType.cs`

```csharp
public enum StarConditionType
{
    Clear,          // 클리어
    NoDeaths,       // 아군 전멸 없음
    TurnLimit,      // N턴 내 클리어
    FullHP,         // 아군 전원 HP 100%
    NoBossSkill,    // 보스 스킬 발동 전 클리어
    ElementParty,   // 특정 속성 파티로 클리어
}
```

### StarCondition

**위치**: `Assets/Scripts/Data/Structs/MasterData/StarCondition.cs`

```csharp
[Serializable]
public struct StarCondition
{
    public StarConditionType Type;
    public int Value;           // TurnLimit의 턴 수, ElementParty의 속성 등
    public string DescriptionKey; // StringData 키 (UI 표시용)
}
```

### StageData 확장

**위치**: `Assets/Scripts/Data/ScriptableObjects/StageData.cs`

```csharp
[CreateAssetMenu(fileName = "StageData", menuName = "SC/Data/StageData")]
public class StageData : ScriptableObject
{
    [Header("기본 정보")]
    public string Id;
    public string ChapterId;
    public StageType StageType;
    public int StageNumber;         // 1-1, 1-2 등의 번호
    public string NameKey;          // StringData 키
    public string DescriptionKey;

    [Header("해금 조건")]
    public StageUnlockCondition UnlockCondition;

    [Header("전투 정보")]
    public int RecommendedPower;    // 추천 전투력
    public int StaminaCost;         // 스태미나 소모
    public List<string> EnemyIds;   // 적 캐릭터 ID 목록
    public Difficulty Difficulty;

    [Header("보상")]
    public List<RewardInfo> ClearRewards;       // 클리어 보상
    public List<RewardInfo> FirstClearRewards;  // 초회 클리어 보상

    [Header("파티 프리셋")]
    public string PresetGroupId;        // 파티 프리셋 그룹 ("main", "daily_fire" 등)

    [Header("별점 조건")]
    public StarCondition Star1Condition;  // 1별 (보통 Clear)
    public StarCondition Star2Condition;  // 2별
    public StarCondition Star3Condition;  // 3별
}
```

### PresetGroupId

스테이지 타입 내에서도 세부 컨텐츠별로 별도 프리셋이 필요한 경우를 지원.

**명명 규칙**: `{type}` 또는 `{type}_{subtype}`

| 컨텐츠 | PresetGroupId | 설명 |
|--------|---------------|------|
| 메인 스토리 | `main` | 전체 공유 |
| 하드 모드 | `hard` | 전체 공유 |
| 일일던전 - 불 | `daily_fire` | 속성별 분리 |
| 일일던전 - 물 | `daily_water` | 속성별 분리 |
| 일일던전 - 풀 | `daily_grass` | 속성별 분리 |
| 일일던전 - 번개 | `daily_thunder` | 속성별 분리 |
| 일일던전 - 빛 | `daily_light` | 속성별 분리 |
| 일일던전 - 어둠 | `daily_dark` | 속성별 분리 |
| 보스레이드 - 드래곤 | `boss_dragon` | 보스별 분리 |
| 보스레이드 - 거인 | `boss_giant` | 보스별 분리 |
| 무한의 탑 | `tower` | 전체 공유 |
| 이벤트 | `event_{eventId}` | 이벤트별 분리 |

### PartyPreset

**위치**: `Assets/Scripts/Data/Structs/UserData/PartyPreset.cs`

```csharp
[Serializable]
public struct PartyPreset
{
    public string PresetGroupId;        // "main", "daily_fire", "boss_dragon" 등
    public int SlotIndex;               // 0~4 (5개 슬롯)
    public string Name;                 // 유저 지정 이름 (선택)
    public List<string> CharacterInstanceIds;  // 최대 5인
}
```

**프리셋 관리 구조**:
- PresetGroupId별 최대 5개 프리셋
- 스테이지 진입 시 해당 스테이지의 PresetGroupId로 프리셋 조회
- 새로운 컨텐츠 추가 시 새 PresetGroupId만 정의하면 자동 지원

### StageProgress 확장

**위치**: `Assets/Scripts/Data/Structs/UserData/StageProgress.cs`

```csharp
[Serializable]
public struct StageClearInfo
{
    public string StageId;
    public bool IsCleared;
    public int StarCount;       // 0~3
    public bool[] StarAchieved; // [star1, star2, star3] 개별 달성 여부
    public int ClearCount;      // 총 클리어 횟수
    public DateTime FirstClearTime;
}

[Serializable]
public struct StageProgress
{
    public Dictionary<string, StageClearInfo> ClearInfos;

    // 헬퍼 메서드
    public bool IsCleared(string stageId);
    public int GetStarCount(string stageId);
    public bool IsStarAchieved(string stageId, int starIndex);
}
```

### UserSaveData 확장

```csharp
// UserSaveData에 추가
public Dictionary<string, List<PartyPreset>> PartyPresets;  // Key: PresetGroupId
public StageProgress StageProgress;
```

**예시**:
```csharp
PartyPresets = {
    ["main"] = [preset0, preset1, ...],       // 메인 스토리용 5개
    ["daily_fire"] = [preset0, preset1, ...], // 일일던전 불속성용 5개
    ["daily_water"] = [preset0, ...],         // 일일던전 물속성용 5개
    ["boss_dragon"] = [preset0, ...]          // 드래곤 레이드용 5개
}
```

---

## Request/Response

### StageBattleRequest

**위치**: `Assets/Scripts/Packet/Requests/StageBattleRequest.cs`

```csharp
[Serializable]
public struct StageBattleRequest : IRequest
{
    public long Timestamp { get; set; }
    public string StageId;
    public List<string> PartyCharacterIds;  // InstanceId 목록 (최대 5)
}
```

### StageBattleResponse

**위치**: `Assets/Scripts/Packet/Responses/StageBattleResponse.cs`

```csharp
[Serializable]
public struct StageBattleResponse : IGameActionResponse
{
    public bool IsSuccess { get; set; }
    public ErrorCode ErrorCode { get; set; }
    public long ServerTime { get; set; }
    public UserDataDelta Delta { get; set; }  // 스태미나 차감 등

    public string BattleId;                   // 전투 세션 ID
    public BattleInitialData BattleData;      // 전투 초기 데이터
}

[Serializable]
public struct BattleInitialData
{
    public string StageId;
    public List<CharacterBattleData> PlayerParty;
    public List<CharacterBattleData> EnemyParty;
    public int TurnLimit;  // 3별 조건용 (없으면 0)
}

[Serializable]
public struct CharacterBattleData
{
    public string CharacterId;
    public string InstanceId;   // 플레이어 캐릭터만
    public int Level;
    public int HP, MaxHP;
    public int ATK, DEF, SPD;
    public float CritRate, CritDamage;
    public List<string> SkillIds;
}
```

---

## UI 상세

### StageDashboardScreen

**역할**: 스테이지 타입 선택 대시보드

**UI 형태**: 리스트형 (초기) → 자유형 (아트 확정 후)

```
┌─────────────────────────────────────────┐
│            스테이지 선택                 │
├─────────────────────────────────────────┤
│ 🗡️ 메인 스토리                          │
│    진행: 3장 5스테이지  ★★★ 42개        │
├─────────────────────────────────────────┤
│ ⚔️ 하드 모드                   🔒        │
│    1장 클리어 시 해금                    │
├─────────────────────────────────────────┤
│ 📅 일일 던전                            │
│    오늘 남은 횟수: 2/5                   │
├─────────────────────────────────────────┤
│ 👹 보스 레이드                          │
│    주간 남은 횟수: 1/3                   │
├─────────────────────────────────────────┤
│ 🗼 무한의 탑                            │
│    현재 층: 25F                         │
└─────────────────────────────────────────┘
```

**표시 정보**:
- 타입 이름, 아이콘
- 진행 상황 (현재 스테이지, 별점 수)
- 잠금 상태 및 해금 조건
- 남은 횟수 (일일/주간 제한 있는 경우)
- 알림 뱃지 (보상 수령 가능 등)

### StageListScreen

**역할**: 선택된 타입의 스테이지 목록

**파라미터**: `StageType` (어떤 타입 스테이지를 표시할지)

```
┌─────────────────────────────────────────┐
│  ← 메인 스토리                          │
├─────────────────────────────────────────┤
│  [1장] [2장] [3장🔒] ...                │
├─────────────────────────────────────────┤
│  ┌─────────┐ ┌─────────┐ ┌─────────┐   │
│  │  1-1    │ │  1-2    │ │  1-3    │   │
│  │  ★★★   │ │  ★★☆   │ │  ★☆☆   │   │
│  └─────────┘ └─────────┘ └─────────┘   │
│  ┌─────────┐ ┌─────────┐ ┌─────────┐   │
│  │  1-4    │ │  1-5 🔒 │ │  1-6 🔒 │   │
│  │   -     │ │         │ │         │   │
│  └─────────┘ └─────────┘ └─────────┘   │
└─────────────────────────────────────────┘
```

### PartySelectScreen

**역할**: 파티 편성 (최대 5인)

**파라미터**: `StageData` (PresetGroupId 포함)

```
┌─────────────────────────────────────────┐
│  ← 파티 편성            [프리셋 ▼]      │
├─────────────────────────────────────────┤
│  파티 슬롯 (5칸)                         │
│  ┌────┐ ┌────┐ ┌────┐ ┌────┐ ┌────┐    │
│  │Char│ │Char│ │Char│ │ + │ │ + │    │
│  └────┘ └────┘ └────┘ └────┘ └────┘    │
├─────────────────────────────────────────┤
│  보유 캐릭터                             │
│  ┌────┐ ┌────┐ ┌────┐ ┌────┐ ...       │
│  │    │ │ ✓ │ │    │ │ ✓ │           │
│  └────┘ └────┘ └────┘ └────┘           │
├─────────────────────────────────────────┤
│  총 전투력: 15,200   추천: 12,000       │
│                        [전투 시작]       │
└─────────────────────────────────────────┘
```

**프리셋 로드 흐름**:
```
StageData.PresetGroupId ("daily_fire")
           │
           ▼
UserSaveData.PartyPresets["daily_fire"]
           │
           ▼
List<PartyPreset> (5개 슬롯)
```

---

## 설계 원칙

1. **서버 중심 전투 시작**
   - 전투 시작은 서버(LocalApiClient) 검증 후 처리
   - 스태미나 차감, 해금 조건 검증

2. **분리된 화면 흐름**
   - StageDashboardScreen: 타입 선택
   - StageListScreen: 스테이지 선택
   - PartySelectScreen: 파티 편성
   - 단일 책임 원칙 준수

3. **재사용 가능한 구조**
   - StageListScreen, PartySelectScreen은 LiveEvent에서도 재사용
   - StageType으로 구분

4. **PresetGroupId 기반 프리셋 관리**
   - 컨텐츠별 PresetGroupId로 프리셋 그룹 분리
   - 스테이지 타입 내에서도 세부 분류 가능 (속성, 보스 등)
   - 각 그룹당 최대 5개 프리셋
   - 새 컨텐츠 추가 시 PresetGroupId만 정의하면 자동 지원

---

## 전투력 계산

### 캐릭터 전투력
```
캐릭터 전투력 = (HP/10) + (ATK*5) + (DEF*3) + (SPD*2)
            + (CritRate*100) + (CritDamage*50)
```

### 파티 전투력
```
파티 전투력 = Σ(캐릭터 전투력)   // 최대 5명
```

### UI 표시
| 비율 | 표시 색상 | 메시지 |
|------|-----------|--------|
| ≥120% | 녹색 | "충분한 전투력" |
| 100~119% | 흰색 | "적정 전투력" |
| 80~99% | 노랑 | "주의 필요" |
| <80% | 빨강 | "전투력 부족" |

---

## 에러 코드

| ErrorCode | 값 | 설명 |
|-----------|-----|------|
| `StageNotFound` | 5101 | 스테이지 없음 |
| `StageLocked` | 5102 | 스테이지 잠김 |
| `StageInsufficientStamina` | 5103 | 스태미나 부족 |
| `StageInvalidParty` | 5104 | 잘못된 파티 구성 |
| `StageCharacterNotOwned` | 5105 | 미보유 캐릭터 |
| `StagePartySizeInvalid` | 5106 | 파티 인원 부족 (최소 1명) |
| `StageDailyLimitReached` | 5107 | 일일 도전 횟수 초과 |

---

## 상태

| 분류 | 상태 |
|------|------|
| 마스터 데이터 확장 | ✅ 설계 완료 |
| 유저 데이터 | ✅ 설계 완료 |
| Request/Response | ✅ 설계 완료 |
| UI | ✅ 설계 완료 |
| 구현 | ⬜ 대기 |

---

## 구현 체크리스트

```
Phase 3: 스테이지 진입 구현

Enums:
- [ ] StageType.cs
- [ ] UnlockConditionType.cs
- [ ] StarConditionType.cs

마스터 데이터:
- [ ] StageUnlockCondition.cs
- [ ] StarCondition.cs
- [ ] StageData.cs 확장 (PresetGroupId 필드 포함)
- [ ] Stage.json 샘플 데이터 업데이트 (PresetGroupId 포함)

유저 데이터:
- [ ] PartyPreset.cs
- [ ] StageClearInfo.cs
- [ ] StageProgress.cs 확장
- [ ] UserSaveData 확장 (PartyPresets, StageProgress)

Request/Response:
- [ ] StageBattleRequest.cs
- [ ] StageBattleResponse.cs
- [ ] BattleInitialData.cs
- [ ] CharacterBattleData.cs

이벤트:
- [ ] StageEvents.cs
  - [ ] StageSelectedEvent
  - [ ] BattleStartRequestedEvent
  - [ ] BattleReadyEvent

API:
- [ ] LocalApiClient.StartBattleAsync 구현

UI:
- [ ] Sc.Contents.Stage Assembly 생성
- [ ] StageDashboardScreen.cs
- [ ] StageListScreen.cs
- [ ] StageItem.cs
- [ ] StageInfoPopup.cs
- [ ] PartySelectScreen.cs
- [ ] PartySlotWidget.cs
- [ ] SelectableCharacterItem.cs

연동:
- [ ] LobbyScreen에 [스테이지] 버튼 추가
- [ ] NetworkManager 연동 테스트
```

---

## 관련 문서

- [Data.md](Data.md) - 데이터 구조 개요
- [Packet.md](Packet.md) - 네트워크 패턴
- [Character.md](Character.md) - 캐릭터 시스템
- [Navigation.md](Navigation.md) - 화면 전환
- [Common/Reward.md](Common/Reward.md) - 보상 시스템
- [LiveEvent.md](LiveEvent.md) - 이벤트 시스템 (StageListScreen 재사용)
