# 시스템 스펙

> **최종 수정**: 2026-01-28
> **상태**: 실제 구현 반영

## 핵심 시스템 및 디자인 패턴

### 구현 완료

| 시스템 | 패턴 | 설명 | 상태 |
|--------|------|------|------|
| **Foundation** | Singleton | 전역 매니저 베이스 | ✅ |
| **Asset** | Scope + LRU | Addressables 래핑, 생명주기 관리 | ✅ |
| **Save** | Repository | 저장/로드, 마이그레이션, 자동저장 | ✅ |
| **Pool** | Object Pool | 오브젝트 재사용 | ✅ |
| **UI** | Widget-State | Screen/Popup 상태 관리 | ✅ |
| **Navigation** | Stack + State | 화면 전환, 히스토리 관리 | ✅ |
| **LocalServer** | Request-Response | 서버 시뮬레이션 레이어 | ✅ |
| **Gacha** | Service | 확률 계산, 천장 시스템 | ✅ |

### 미구현 (FUTURE)

| 시스템 | 패턴 | 설명 | 우선순위 |
|--------|------|------|----------|
| **Battle** | State + Command | 턴제 전투 시스템 | FUTURE |
| **Skill** | Decorator | 스킬/버프 시스템 | FUTURE |
| **Quest** | Composite | 퀘스트 조건 시스템 | P2 |
| **Event** | Observer | 전역 이벤트 시스템 | P1 |

---

## 시스템별 상세

### Foundation

```
Assets/Scripts/Foundation/
├── Singleton.cs           # MonoBehaviour 싱글톤 베이스
├── GameBootstrap.cs       # 게임 초기화 진입점
└── GameFlowController.cs  # 게임 플로우 제어
```

- `Singleton<T>`: MonoBehaviour 기반, DontDestroyOnLoad 지원
- `GameBootstrap`: 초기화 순서 관리, 씬 전환 진입점
- `GameFlowController`: 로딩 → 로비 → 인게임 플로우

### Asset (AssetManager)

```
Assets/Scripts/Core/Managers/AssetManager.cs
```

- **Scope 기반 생명주기**: Global, Scene, Screen, Popup
- **LRU 캐싱**: 메모리 관리, 자동 해제
- **Addressables 통합**: 비동기 로드, 의존성 관리

```csharp
// 사용 예시
var sprite = await AssetManager.Instance.LoadAsync<Sprite>("Icons/Character", AssetScope.Screen);
AssetManager.Instance.ReleaseScope(AssetScope.Screen);
```

### Save (SaveManager)

```
Assets/Scripts/Core/Managers/SaveManager.cs
Assets/Scripts/Data/UserSaveData.cs
```

- `SaveManager`: 저장/로드 관리, 마이그레이션 지원, 자동저장
- `UserSaveData`: 유저 데이터 구조 (GameState 역할)

```csharp
// 사용 예시
SaveManager.Instance.Save();
var data = SaveManager.Instance.Load<UserSaveData>();
```

### Pool

```
Assets/Scripts/Common/Pool/
├── IPoolable.cs           # 풀링 인터페이스
├── ObjectPool.cs          # 제네릭 오브젝트 풀
└── PoolManager.cs         # 풀 중앙 관리
```

- `IPoolable`: OnSpawn, OnDespawn 인터페이스
- `ObjectPool<T>`: Stack 기반, 제네릭 구현
- `PoolManager`: Singleton, Dictionary로 풀 관리

```csharp
// 사용 예시
var bullet = PoolManager.Instance.Spawn<Bullet>(prefab);
PoolManager.Instance.Despawn(bullet);
```

### UI (Widget-State)

```
Assets/Scripts/Common/UI/
├── Widget.cs                    # UI 기본 단위
├── ScreenWidget.cs              # Screen + State 통합
├── PopupWidget.cs               # Popup + State 통합
├── IScreenState.cs              # Screen 상태 인터페이스
└── IPopupState.cs               # Popup 상태 인터페이스
```

**MVP 대신 Widget-State 패턴 적용**

- `Widget`: 모든 UI의 기본 단위, 초기화/해제 라이프사이클
- `ScreenWidget<TScreen, TState>`: Screen과 State 통합
- `PopupWidget<TPopup, TState>`: Popup과 State 통합
- `IScreenState`, `IPopupState`: 화면 전환 파라미터

```csharp
// Screen 정의
public class GachaScreen : ScreenWidget<GachaScreen, GachaState> { }
public class GachaState : IScreenState { public string BannerId; }

// Popup 정의
public class RewardPopup : PopupWidget<RewardPopup, RewardPopupState> { }
public class RewardPopupState : IPopupState { public List<RewardItem> Items; }
```

### Navigation

```
Assets/Scripts/Core/Services/NavigationManager.cs
Assets/Scripts/Core/Services/NavigationStack.cs
```

- `NavigationManager`: 화면 전환 관리, Screen/Popup 스택
- `NavigationStack`: 히스토리 관리, Back 처리
- **Transition 지원**: Fade, Slide 등 전환 효과

```csharp
// 사용 예시
await NavigationManager.Instance.PushScreen<GachaScreen>(new GachaState { BannerId = "banner_001" });
await NavigationManager.Instance.ShowPopup<RewardPopup>(new RewardPopupState { Items = rewards });
await NavigationManager.Instance.Back();
```

### LocalServer

```
Assets/Scripts/LocalServer/
├── LocalGameServer.cs           # 서버 진입점
├── Handlers/                    # 요청 핸들러
│   ├── GachaHandler.cs
│   ├── ShopHandler.cs
│   └── EventHandler.cs
├── Services/                    # 비즈니스 로직
│   ├── GachaService.cs
│   ├── RewardService.cs
│   └── TimeService.cs
├── Validators/                  # 요청 검증
│   └── RequestValidator.cs
└── Models/                      # Request/Response
    ├── GachaRequest.cs
    └── GachaResponse.cs
```

**서버 시뮬레이션 레이어** (실서버 전환 대비)

- `LocalGameServer`: 요청 라우팅, 핸들러 등록
- `Handler`: 요청 처리, Service 호출
- `Service`: 비즈니스 로직 (가챠, 보상, 시간)
- `Validator`: 요청 유효성 검증

```csharp
// 사용 예시
var response = await LocalGameServer.Instance.Request<GachaRequest, GachaResponse>(
    new GachaRequest { BannerId = "banner_001", Count = 10 }
);
```

### Gacha

```
Assets/Scripts/LocalServer/Services/GachaService.cs
Assets/Scripts/Data/ScriptableObjects/GachaPoolData.cs
```

- `GachaService`: 확률 계산, 소프트/하드 천장 시스템
- `GachaPoolData`: ScriptableObject 기반 가챠 풀 정의

**Strategy 패턴 대신 Service 통합** (단순화)

```csharp
// 천장 시스템
- 소프트 천장: 74회부터 SSR 확률 점진적 증가
- 하드 천장: 90회에 SSR 확정
```

### Character (Data)

```
Assets/Scripts/Data/ScriptableObjects/CharacterData.cs
```

- `CharacterData`: ScriptableObject 기반 캐릭터 정보
- 기본 스탯, 스킬 ID, 레어리티 정의

**Factory/Flyweight 패턴 미적용** (ScriptableObject 직접 참조로 충분)

---

## 미구현 시스템 (FUTURE)

### Battle (계획)

```
Contents/InGame/Battle/
├── BattleManager.cs             # 전투 관리
├── TurnManager.cs               # 턴 관리
├── States/                      # State 패턴
│   ├── IBattleState.cs
│   ├── BattleReadyState.cs
│   ├── PlayerTurnState.cs
│   ├── EnemyTurnState.cs
│   └── BattleEndState.cs
└── Commands/                    # Command 패턴
    ├── ICommand.cs
    ├── AttackCommand.cs
    └── SkillCommand.cs
```

### Skill/Buff (계획)

```
Contents/InGame/Skill/
├── SkillExecutor.cs             # 스킬 실행
├── IStatModifier.cs             # Decorator 인터페이스
├── BuffDecorator.cs             # 버프 래핑
└── DebuffDecorator.cs           # 디버프 래핑
```

### Quest (계획)

```
Contents/OutGame/Quest/
├── QuestManager.cs              # 퀘스트 관리
├── Conditions/                  # Composite 패턴
│   ├── IQuestCondition.cs
│   ├── AndCondition.cs
│   ├── OrCondition.cs
│   ├── KillCondition.cs
│   └── CollectCondition.cs
└── Views/
    ├── QuestListView.cs
    └── QuestDetailView.cs
```

### Event (계획)

```
Core/Events/
├── EventManager.cs              # 전역 이벤트 관리
├── IGameEvent.cs                # 이벤트 인터페이스
└── GameEvents.cs                # 이벤트 정의
```

---

## 아키텍처 결정 사항

### 채택된 패턴

| 결정 | 이유 |
|------|------|
| Widget-State > MVP | Unity 컴포넌트 모델에 더 적합, State를 통한 파라미터 전달 |
| Service 통합 > Strategy | 단일 서비스로 충분, 오버엔지니어링 방지 |
| ScriptableObject > Factory | Unity 에디터 친화적, 데이터 관리 용이 |
| LocalServer 레이어 | 실서버 전환 대비, 클라이언트 로직 분리 |

### 미채택 패턴

| 패턴 | 이유 |
|------|------|
| MVP | Widget-State로 대체 |
| Memento | SaveManager 직접 직렬화로 충분 |
| Factory/Flyweight (Character) | ScriptableObject로 충분 |
| Strategy (Gacha) | Service 통합으로 단순화 |

---

## 참조

| 문서 | 용도 |
|------|------|
| [ARCHITECTURE.md](ARCHITECTURE.md) | 폴더 구조, 의존성 |
| [STATUS_REPORT.md](STATUS_REPORT.md) | 구현 현황 |
| [Specs/SPEC_INDEX.md](Specs/SPEC_INDEX.md) | Assembly별 상세 스펙 |
