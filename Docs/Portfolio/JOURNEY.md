# 프로젝트 여정

프로젝트의 진행 과정을 커밋 기준으로 기록합니다.

---

## Phase 1: 프로젝트 초기화

### 2024-XX - Initial Setup

**커밋**: `b9266e2` Initial commit
- Unity 프로젝트 생성
- 기본 디렉토리 구조 설정

**커밋**: `92c3520` Add initial project configuration files and settings
- Unity 프로젝트 설정 파일 추가
- .gitignore, EditorConfig 등 설정

**커밋**: `968b75d` Add initial meta files for various game components and systems
- 게임 컴포넌트 기본 구조 설계
- Assembly Definition 기반 모듈화 시작

---

## Phase 2: 아키텍처 설계

### 이벤트/패킷 시스템 분리

**커밋**: `f5a5f11` Separate Event and Packet layers with interface abstraction
- **문제**: 이벤트와 네트워크 패킷의 책임 혼재
- **해결**: 인터페이스 추상화를 통한 계층 분리
- **결과**: 각 레이어가 독립적으로 변경 가능

### Editor AI 도구 도입

**커밋**: `d688bfd` Add Editor-only AI development tools assembly
- **목적**: AI와 협업 시 Unity Editor 작업 자동화
- **결정**: Editor 전용 Assembly로 분리하여 빌드 영향 제거

---

## Phase 3: UI 시스템 구축

### UI 아키텍처 진화

**커밋**: `0d89977` Update UIComponents: Unity 기본 컴포넌트 우선 사용 원칙 추가
- Unity 기본 컴포넌트 우선 사용 원칙 확립
- 커스텀 컴포넌트는 필요시에만 추가

**커밋**: `8160eef` Add UISystem, LiveEvent, Navigation spec documents
- UI 시스템 스펙 문서화
- Navigation 시스템 설계

**커밋**: `6792d60` Refactor UI architecture: MVP → Widget System
- **중요 결정**: MVP 패턴에서 Widget 기반 시스템으로 전환
- **이유**: Unity UI의 특성에 맞는 더 유연한 구조 필요
- **상세**: [DECISIONS.md - UI 아키텍처: MVP → Widget](DECISIONS.md) 참조

### UI 구현

**커밋**: `5f13c68` Update Unity settings and add script meta files
- Unity 설정 최적화
- 스크립트 메타 파일 정리

**커밋**: `fcd51db` Add UniTask package dependency
- UniTask 패키지 추가
- 비동기 처리 기반 마련

**커밋**: `73ac0c7` Implement UI system core: Widget, Screen, Popup, Navigation
- Widget 시스템 핵심 구현
- Screen/Popup/Navigation 기본 동작

**커밋**: `28160c2` Add UI test environment: TestScreen, TestPopup, Prefabs
- 테스트 환경 구축
- 검증용 Screen/Popup 프리팹

---

## Phase 4: 개발 도구 강화

**커밋**: `c422391` Add Editor AI tools: NavigationDebugWindow, UITestSceneSetup
- Navigation 디버그 윈도우
- UI 테스트 씬 자동 셋업 도구

**커밋**: `3c19b09` Update specs: Navigation v2.0, UISystem v3.1
- 스펙 문서 버전 업데이트
- 구현 결과 반영

---

## Phase 5: 프로세스 개선

**커밋**: `30359c4` Update CLAUDE.md: Progress 추적 필수 지침 추가
- Progress 추적 필수화
- AI 협업 프로세스 정립

**커밋**: `a861c85` Add tmpclaude-* to .gitignore
- 임시 파일 관리 개선

---

## Phase 6: 데이터 아키텍처 v2.0

### 아키텍처 재설계

**커밋**: `8b6aae0` (리셋 기준점)
- **문제**: v1.0 로컬 중심 설계로는 라이브 서비스 대응 불가
- **결정**: 서버 중심(Server Authority) 아키텍처로 재설계
- **상세**: [DECISIONS.md - 데이터 아키텍처: 로컬 중심 → 서버 중심](DECISIONS.md) 참조

### 마스터 데이터 파이프라인

**커밋**: 여러 커밋 통합
- Excel/JSON → Python Export → JSON → AssetPostprocessor → ScriptableObject
- MasterDataImporter: JSON 변경 감지 시 자동 SO 생성
- MasterDataGeneratorWindow: 수동 생성 UI

### 서버 중심 데이터 흐름

**커밋**: 여러 커밋 통합
- IApiClient 인터페이스 정의
- LocalApiClient: 서버 응답 시뮬레이션 (로컬 JSON 저장)
- UserDataDelta: 부분 갱신 패턴
- DataManager: 읽기 전용 뷰, SetUserData/ApplyDelta

### 네트워크 아키텍처 개선

**커밋**: `0750591` Refactor PacketDispatcher to callback pattern
- PacketDispatcher 콜백 패턴으로 변경
- 순환 참조 해결을 위한 Sc.Foundation Assembly 분리

**커밋**: `8f7d28e` Optimize RequestQueue by removing reflection
- RequestQueue 리플렉션 제거 (성능 최적화)

### 통합 테스트 도구

**커밋**: `0da3d36` Add DataFlowTestWindow and update docs to v2.0
- DataFlowTestWindow 에디터 도구
  - Login/Gacha 흐름 통합 테스트
  - DataManager 자동 생성 및 Database 할당
- 문서 v2.0 업데이트 (Data.md, Packet.md, Core.md)

---

## Phase 7: MVP 기반 컴포넌트

### 기본 Widget 컴포넌트

**커밋**: `548caf3` Add base UI Widget components (8 types)
- Unity 기본 UI 컴포넌트를 Widget 시스템에 통합
- **목적**: MVP 화면 구현 전 일관된 UI 패턴 확립
- **결정**: Widget 래퍼로 라이프사이클 통합
- **상세**: [DECISIONS.md - Unity 기본 컴포넌트 Widget화](DECISIONS.md) 참조

구현된 Widget:
| Widget | 역할 |
|--------|------|
| TextWidget | 텍스트 표시/스타일링 |
| ButtonWidget | 클릭 이벤트, 라벨/아이콘 |
| ImageWidget | Sprite 표시 |
| SliderWidget | 값 조절 (볼륨 등) |
| ToggleWidget | ON/OFF 스위치 |
| InputFieldWidget | 텍스트 입력 |
| ProgressBarWidget | 진행률 표시 |
| ScrollViewWidget | 스크롤 컨테이너 |

---

## Phase 8: MVP 화면 구현

### MVP 화면 구현

**커밋**: `677b606` Add MVP screen implementations
- TitleScreen, LobbyScreen, GachaScreen, CharacterListScreen 구현
- CurrencyHUD, GachaResultPopup 컴포넌트 추가
- DataManager/NetworkManager 연동

**커밋**: `87b25d6` Add MVP Editor tools
- MVPSceneSetup: SC Tools/MVP 메뉴로 씬/프리팹 자동 생성
- ProjectEditorSettings: 에디터 공통 설정 (기본 폰트 등)
- **상세**: [DECISIONS.md - 에디터 설정 ScriptableObject 도입](DECISIONS.md) 참조

### 게임 초기화 흐름

**커밋**: `9d296ac` Document architecture decisions
- GameBootstrap → NetworkManager 초기화 → DataManager 로드 → Login
- GameInitializedEvent로 초기화 완료 알림
- GameFlowController가 이벤트 수신 후 TitleScreen Push

### 캐릭터 상세 화면 & API 간소화

**커밋**: `1eb467b` Add CharacterDetailScreen and simplify Navigation API
- **CharacterDetailScreen 추가**
  - 캐릭터 기본 정보, 스탯, 레벨/돌파, 설명 표시
  - CharacterListScreen에서 아이템 클릭 시 상세 화면 이동

- **Navigation API 간소화**
  - `Screen.Open(state)` / `Popup.Open(state)` 패턴 도입
  - 기존 장황한 형식에서 간결한 형식으로 개선
  - **상세**: [DECISIONS.md - Navigation API 간소화: Open() 패턴](DECISIONS.md) 참조

- **ScrollView 개선**
  - RectMask2D 사용 (Image+Mask보다 효율적)
  - LayoutElement 추가 (VerticalLayoutGroup 높이 인식)

---

## Phase 9: Transition 애니메이션

### Screen/Popup Transition 구현

**커밋**: `45ae44e` 등 8개
- Screen/Popup 전환 시 애니메이션 지원
- DOTween 기반 구현
- **상세**: [DECISIONS.md - Screen/Popup Transition 애니메이션 설계](DECISIONS.md) 참조

**구현 내용**:
| 클래스 | 역할 |
|--------|------|
| Transition | 베이스 추상 클래스 (Enter/Exit) |
| FadeTransition | Screen용 페이드 인/아웃 |
| PopupTransition | Popup용 베이스 클래스 |
| PopupScaleTransition | Popup용 스케일+페이드 |

**Widget 개선**:
- CachedCanvasGroup 프로퍼티 추가 (성능 최적화)
- CanvasGroup 컴포넌트 자동 캐싱

**Context 통합**:
- ScreenWidget.Context.Enter/Exit에서 Transition 호출
- PopupWidget.Context.Enter/Exit에서 Transition 호출
- Builder 패턴으로 Transition 설정 가능

**Assembly Definition 수정**:
- Sc.Common.asmdef에 DOTween.Modules, UniTask.DOTween 참조 추가
- DOTween 확장 메서드 사용 가능

---

## Phase 10: OUTGAME-V1 설계

### Foundation & Stage 설계 (Phase 0, 3)

**커밋**: `1b1d122` Add spec documents and update CLAUDE.md
- Phase 0 Foundation 시스템 상세 설계
  - Logging: LogLevel/LogCategory, ILogOutput 확장성
  - Error: ErrorCode 체계, Result<T> 패턴
  - StringData: 자체 구현 문자열 관리 (경량화)
  - SaveManager: 버전 마이그레이션 시스템
  - LoadingService: 레퍼런스 카운팅 기반
- Phase 3 Stage 시스템 설계
  - StageDashboardScreen: 파티편성 + 스테이지 정보 통합
  - PresetGroupId: 컨텐츠별 파티 프리셋 그룹핑
  - 3-Star 시스템: 스테이지별 커스텀 조건
- **상세**: [DECISIONS.md - PresetGroupId 기반 파티 프리셋](DECISIONS.md) 참조

### LiveEvent 설계 (Phase 4)

**커밋**: `e4cdc93` Add Phase 4 LiveEvent detailed spec
- **중요 결정**: 모듈형 서브컨텐츠 구조
  - EventSubContent 배열로 유연한 이벤트 구성
  - Mission, Stage, Shop, Minigame, Story 모듈
  - 탭 순서/해금 조건 데이터 제어
- **중요 결정**: 이벤트 재화 정책
  - 유예 기간 (GracePeriodDays) 후 자동 전환
  - 범용 재화로 ConversionRate 비율 전환
- LiveEventData/LiveEventProgress 구조
- EventMissionData/EventMissionProgress 구조
- 16가지 MissionConditionType 정의
- Request/Response 패턴 설계
- Error 코드 6001-6007 범위 할당
- **상세**: [DECISIONS.md - 모듈형 이벤트 서브컨텐츠 (EventSubContent)](DECISIONS.md) 참조

---

## Phase 11: 테스트 아키텍처 설계

### 테스트 인프라 설계

**문서**: `Docs/Specs/Testing/TestArchitecture.md`
- **문제**: Phase별 구현에 앞서 테스트 가능한 환경 필요
- **핵심 원칙**: 테스트는 마일스톤 Phase가 아닌 시스템 단위로 구성
- **상세**: [DECISIONS.md - 테스트 아키텍처: 시스템 단위 테스트 설계](DECISIONS.md) 참조

**주요 결정**:
| 항목 | 결정 |
|------|------|
| 의존성 관리 | SO + ServiceLocator 혼합 패턴 |
| 테스트 구조 | 시스템 단위 (5계층: Foundation → Content) |
| 자동화 시점 | 2차 구축 (수동 테스트 안정화 후) |
| 첫 대상 | Navigation 시스템 |

**설계된 구조**:
```
Assets/Scripts/
├── Core/
│   └── Services.cs              # ServiceLocator
├── Tests/
│   ├── Base/
│   │   └── SystemTestRunner.cs  # 테스트 러너 기반 클래스
│   ├── Mocks/
│   │   ├── MockTimeService.cs
│   │   └── MockSaveStorage.cs
│   └── Runners/
│       └── NavigationTestRunner.cs
└── Data/TestData/               # 테스트용 SO
```

**5계층 시스템 분류**:
- Foundation (Log, Error, Services) - 의존성 없음
- Infrastructure (Time, Save, Loading) - Foundation만 의존
- Data (Master, User, Network) - Infrastructure까지 의존
- UI (Widget, Navigation, Popup) - Data까지 의존
- Content (Gacha, Character, Stage) - 전체 의존 가능

**배운 점**:
- 테스트 구조는 마일스톤(What to build)이 아닌 아키텍처(How it's built)를 따라야 함
- 수동 테스트 시나리오를 먼저 검증하고 자동화하면 안정적
- ServiceLocator + SO 혼합 패턴이 Unity에서 실용적

---

## Phase 12: Foundation 시스템 구현

### 테스트 인프라 Phase 1

**커밋**: `16d33ee` Add test infrastructure phase 1
- 테스트 인프라 기본 구조 구현
- SystemTestRunner 베이스 클래스
- ITestInterfaces (ITimeService, ISaveStorage)

**커밋**: `65eb307` Refactor to system-based implementation approach
- 시스템 기반 구현 접근법으로 리팩터링
- Locator 패턴 설정 방식 개선

### Foundation 시스템 구현

**커밋**: `4815dee` Add Foundation systems with unit tests
- **Log 시스템**: Log, LogLevel, LogCategory (Unity Debug.Log 래퍼)
- **Error 시스템**: ErrorCode, Result<T> 패턴
- **SaveManager**: 저장 추상화, 버전 마이그레이션
- **상세**: [DECISIONS.md - SaveManager 저장소 추상화 (ISaveStorage)](DECISIONS.md) 참조

**핵심 구현**:
| 컴포넌트 | 역할 | 위치 |
|----------|------|------|
| ISaveStorage | 저장소 추상화 인터페이스 | Foundation |
| FileSaveStorage | 파일 기반 저장소 구현체 | Foundation |
| MockSaveStorage | 테스트용 메모리 저장소 | Tests/Mocks |
| SaveMigrator | 버전 마이그레이션 체인 | Core/Services |
| SaveManager | Singleton, 자동 저장, 마이그레이션 | Core/Managers |

**테스트 인프라**:
| 테스트 | 대상 | 방식 |
|--------|------|------|
| SaveStorageTests | FileSaveStorage | NUnit (17개 테스트) |
| SaveMigratorTests | SaveMigrator | NUnit (5개 테스트) |
| MockSaveStorageTests | MockSaveStorage | NUnit (12개 테스트) |
| SaveManagerTestRunner | SaveManager 통합 | 런타임 시나리오 |

**설계 결정**:
- ISaveStorage를 Foundation에 배치 (순환 참조 방지)
- 테스트 가능성을 위한 생성자 DI 패턴 도입
- NUnit 기반 에디터 테스트와 런타임 시나리오 테스트 병행

**배운 점**:
- 순환 참조 문제는 인터페이스 위치 조정으로 해결 가능
- NUnit 테스트는 빠른 피드백에 효과적, 런타임 테스트는 실제 환경 검증에 효과적
- Mock 객체를 별도로 테스트하면 테스트 신뢰도 향상

---

## Phase 13: LoadingIndicator 시스템 구현

### 로딩 인디케이터 구현

**커밋**: `4b0e92a` Add LoadingIndicator system with editor tools
- LoadingService: 레퍼런스 카운팅 기반 로딩 상태 관리
- LoadingWidget: DOTween 기반 페이드/스피너 애니메이션
- LoadingConfig: ScriptableObject 설정 (타임아웃, 애니메이션)

**핵심 구현**:
| 컴포넌트 | 역할 | 위치 |
|----------|------|------|
| LoadingType | 로딩 타입 (FullScreen, Indicator, Progress) | Common/Enums |
| LoadingConfig | 타임아웃, 애니메이션 설정 SO | Common/Configs |
| LoadingService | 레퍼런스 카운팅, 타임아웃 관리 | Common/Services |
| LoadingWidget | UI 표시, 애니메이션 | Common/Widgets |

**테스트 인프라**:
- LoadingServiceTests.cs (NUnit) - 레퍼런스 카운팅, 상태 전환
- LoadingConfigTests.cs (NUnit) - 기본값 검증

---

## Phase 14: Reward 시스템 구현

### 보상 시스템 구현

**커밋**: `924eb88` Add Reward system with unit tests
- **중요 결정**: 처리 방식 기준 분류 (RewardType 4개 + ItemCategory 6개)
- 수집형 RPG 표준 패턴 분석 (블루아카, FGO 등) 기반 설계
- **상세**: [DECISIONS.md - 보상 시스템 (RewardType) 설계](DECISIONS.md) 참조

**핵심 구현**:
| 컴포넌트 | 역할 | 위치 |
|----------|------|------|
| RewardType | 보상 타입 (Currency, Item, Character, PlayerExp) | Data/Enums |
| ItemCategory | 아이템 분류 (Equipment, Consumable, Material, ...) | Data/Enums |
| RewardInfo | 보상 정보 구조체 (팩토리 메서드 포함) | Data/Structs |
| RewardProcessor | Delta 생성, 검증 (서버 로직) | Core/Utility |
| RewardHelper | UI 헬퍼 (포맷팅, 아이콘, 희귀도 색상) | Core/Utility |

**테스트 인프라** (61개 테스트):
- RewardInfoTests.cs (16개) - 생성자, 팩토리 메서드, ToString
- RewardProcessorTests.cs (28개) - CreateDelta, ValidateRewards, CanApplyRewards
- RewardHelperTests.cs (17개) - FormatText, GetIconPath, GetRarityColor, SortByRarity

**설계 결정**:
- RewardType: 처리 로직 기준 분류 (4개)
- ItemCategory: UI 표시용 세분화 (6개)
- 장비: 인벤토리=수량 기반, 장착=인스턴스 기반
- 스킨: 별도 캐릭터로 처리 (CharacterData.BaseCharacterId)

---

## Phase 15: TimeService 시스템 구현

### 시간 서비스 구현

**커밋**: `924eb88` 이후 작업
- **중요 결정**: 서버 전환 비용 최소화 설계
- 인터페이스에 SyncServerTime, TimeOffset 미리 포함
- 로컬 구현에서도 서버와 동일한 인터페이스 사용
- **상세**: [DECISIONS.md - TimeService 서버 전환 비용 최소화 설계](DECISIONS.md) 참조

**핵심 구현**:
| 컴포넌트 | 역할 | 위치 |
|----------|------|------|
| LimitType | 제한 타입 (Daily, Weekly, Monthly, Permanent, EventPeriod) | Data/Enums |
| ITimeService | 시간 서비스 인터페이스 (서버 동기화 고려) | Core/Interfaces |
| TimeService | 로컬 구현 (UTC 기반, Offset 지원) | Core/Services |
| TimeHelper | UI 표시 헬퍼 (FormatRemainingTime, FormatRelativeTime) | Core/Utility |

**테스트 인프라** (45개 테스트):
- TimeServiceTests.cs (25개) - ServerTimeUtc, SyncServerTime, GetNextResetTime, HasResetOccurred
- TimeHelperTests.cs (20개) - FormatRemainingTime, FormatRelativeTime, GetLimitTypeDisplayName

**설계 결정**:
- 서버 전환 시 구현체만 교체하면 됨 (인터페이스/호출부 변경 없음)
- UTC 0시 기준 리셋 (Daily, Weekly, Monthly)
- MockTimeService로 테스트 시 시간 조작 가능

---

## Phase 16: SystemPopup 구현

### 시스템 팝업 구현

**커밋**: 여러 커밋 통합
- **중요 결정**: 하이브리드 구조 (기존 패턴 + State.Validate())
- GachaResultPopup과 동일한 PopupWidget<TPopup, TState> 패턴 유지
- Alert 모드: ShowCancelButton = false로 처리 (별도 AlertPopup 없음)
- **상세**: [DECISIONS.md - SystemPopup 하이브리드 구조 (v2)](DECISIONS.md) 참조

**핵심 구현**:
| 컴포넌트 | 역할 | 위치 |
|----------|------|------|
| ConfirmState | 확인/취소 팝업 상태 (ShowCancelButton, Alert 모드) | Common/Popups |
| CostConfirmState | 재화 소비 확인 상태 (CostType, IsInsufficient) | Common/Popups |
| ConfirmPopup | 확인/취소 팝업 (배경터치=취소) | Common/Popups |
| CostConfirmPopup | 재화 아이콘+수량 표시 팝업 | Common/Popups |

**테스트 인프라** (34개 테스트):
- ConfirmStateTests.cs (12개) - 기본값, 검증, 콜백
- CostConfirmStateTests.cs (22개) - 재화 검증, IsInsufficient

**에디터 도구**:
- SystemPopupSetup.cs (프리팹 자동 생성, SC Tools/Setup/Prefabs/Dialog/)
- SystemPopupTestController.cs (런타임 테스트, 키보드 1~7)

**배운 점**:
- 클린 아키텍처의 모든 것을 적용하기보다 핵심 개념(검증 로직)만 흡수
- 기존 패턴과의 일관성이 팀 생산성에 중요

---

## Phase 17: RewardPopup 구현

### 보상 팝업 구현

**커밋**: 여러 커밋 통합
- **중요 결정**: IItemSpawner 추상화로 풀링 준비
- **중요 결정**: RewardIconCache 프리로드 방식으로 UI 튀는 현상 방지
- **상세**: [DECISIONS.md - RewardPopup 동적 아이템 관리 설계](DECISIONS.md) 참조

**핵심 구현**:
| 컴포넌트 | 역할 | 위치 |
|----------|------|------|
| IItemSpawner<T> | 동적 아이템 생성 인터페이스 | Common/Interfaces |
| SimpleItemSpawner<T> | Instantiate/Destroy 기반 구현 | Common/Services |
| RewardIconCache | Addressables 비동기 아이콘 프리로드 | Common/Services |
| RewardItem | 개별 보상 표시 위젯 | Common/Widgets |
| RewardPopup | 보상 목록 팝업 (State nested class) | Common/Popups |

**테스트 인프라** (33개 테스트):
- RewardPopupStateTests.cs (13개) - 기본값, 검증, 콜백
- SimpleItemSpawnerTests.cs (12개) - Spawn, Despawn, DespawnAll
- IPopupStateTests.cs (8개) - 기본값, 오버라이드, 호환성

**설계 결정**:
- IItemSpawner로 추상화하여 풀링 도입 시 구현체만 교체
- 프리로드 캐시로 팝업 열기 전 아이콘 미리 로드
- ConfigureLayout으로 보상 개수에 따른 레이아웃 동적 변경

**배운 점**:
- "구현은 단순하게, 인터페이스는 확장 가능하게"
- YAGNI 원칙을 따르면서도 확장성 확보 가능

---

## Phase 18: PlayMode 테스트 인프라 구축

### PlayMode 테스트 환경 재구성

**커밋**: 여러 커밋 통합
- **중요 결정**: 실용적 균형 방식 선택 (기존 헬퍼 재사용 + 새 PlayMode 인프라)
- 3가지 접근법 비교 (최소 변경, 클린 아키텍처, 실용적 균형)
- **상세**: [DECISIONS.md - PlayMode 테스트 인프라 설계 결정](DECISIONS.md) 참조

**핵심 구현**:
| 컴포넌트 | 역할 | 위치 |
|----------|------|------|
| PlayModeTestBase | Addressables 초기화, TestCanvas, 자동 정리 | Tests/PlayMode |
| PrefabTestHelper | Addressables 프리팹 로드/인스턴스 관리 | Tests/PlayMode |
| PlayModeAssert | Unity 오브젝트 전용 어서션 헬퍼 | Tests/PlayMode |

**샘플 테스트**:
- NavigationPlayModeTests.cs (기존 시나리오 NUnit 래핑)
- PrefabLoadPlayModeTests.cs (Addressables 프리팹 로드 검증)

**에디터 도구**:
- PlayModeTestSetup.cs (SC Tools/Setup/Prefabs/ 메뉴)
  - Create All Test Prefabs
  - Create Simple Screen/Popup Prefabs
  - Verify Test Scene
  - Delete All Test Prefabs

**Assembly 설정**:
- Sc.Tests.asmdef에 UnityEngine.TestRunner, UnityEditor.TestRunner 참조 추가

**배운 점**:
- 기존 수동 테스트 시나리오를 NUnit으로 래핑하면 자동화 테스트로 전환 용이
- PlayModeTestBase로 Addressables 초기화 패턴 표준화

---

## Phase 19: 에디터 도구 리팩토링

### SC Tools 메뉴 재구성

**커밋**: 여러 커밋 통합
- **중요 결정**: Bootstrap 레벨 체계화 (None/Partial/Full)
- 기존 SC Tools 메뉴 구조 개선 (6개 카테고리 → 기능별 통합)
- 미완성/중복 코드 제거
- **상세**: [DECISIONS.md - 에디터 도구 Bootstrap 레벨 체계화](DECISIONS.md) 참조

**핵심 변경**:
| 작업 | 내용 |
|------|------|
| 파일 삭제 | SystemTestMenu.cs, SaveManagerTestRunner/Scenarios, AssetManagerTestRunner/Scenarios |
| 메뉴 이동 | PlayModeTestSetup → SC Tools/Setup/Prefabs/, SystemPopupSetup → SC Tools/Setup/Prefabs/Dialog/ |
| 신규 생성 | EditorUIHelpers.cs (공용 UI 생성 헬퍼) |
| 문서 대체 | AITools.md → EditorTools.md v2.1 |

**Bootstrap 레벨 체계**:
| 레벨 | 설명 | 도구 예시 |
|------|------|----------|
| None | 프리팹 생성 전용, 씬 오브젝트 없음 | PlayModeTestSetup, SystemPopupSetup |
| Partial | EventSystem + 일부 매니저 | UITestSceneSetup, LoadingSetup |
| Full | 모든 매니저 생성 | MVPSceneSetup |

**EditorUIHelpers 기능**:
- CreateCanvas, CreatePanel, CreateText, CreateButton
- CreateEventSystem, EnsureFolder

**배운 점**:
- 에디터 도구도 일관된 패턴과 공용 헬퍼로 중복 제거 가능
- Bootstrap 레벨 명시로 각 도구의 역할 명확화

---

## Phase 20: AssetManager 통합

### AssetManager 코드 개선 및 통합

**커밋**: `c7acadc`, `0d8f399`
- **중요 결정**: IAssetHandle 인터페이스 추가 (Reflection 제거)
- RewardIconCache → AssetManager Scope 기반 로딩으로 대체
- 127줄 코드 제거
- **상세**: [DECISIONS.md - AssetManager RewardIconCache 대체 결정](DECISIONS.md) 참조

**핵심 변경**:
| 작업 | 내용 |
|------|------|
| IAssetHandle | RefCount, IsReleasable 등 인터페이스화 (Reflection 제거) |
| Debug.Log → Log | 전체 6개 파일 로그 시스템 전환 |
| GameBootstrap | AssetManager.Initialize() 호출 추가 (초기화 순서 1번) |
| RewardPopup | AssetScope 기반 아이콘 로딩 구현 |
| RewardIconCache.cs | 삭제 (127줄) |

**GameBootstrap 초기화 순서**:
```
1. AssetManager.Initialize()
2. NetworkManager 생성
3. DataManager 생성
4. Login
```

**배운 점**:
- Reflection 대신 인터페이스로 타입 안전성 확보
- 중앙 집중화된 에셋 관리로 개별 캐시 클래스 제거 가능

---

## Phase 21: AssetManager 테스트 구현

### AssetManager 단위 테스트 작성

**커밋**: `c45e3d1`
- IAssetHandle 인터페이스 기반 테스트
- AssetCacheManager LRU 트리밍 검증
- IsReleasable 플래그 리셋 누락 수정

**테스트 구현**:
| 테스트 파일 | 테스트 수 | 검증 대상 |
|------------|----------|----------|
| AssetHandleTests.cs | 6개 추가 | IAssetHandle 인터페이스 동작 |
| AssetScopeTests.cs | 업데이트 | IAssetHandle 기반 ForceRelease |
| AssetCacheManagerTests.cs | 14개 신규 | 캐시 등록/조회/제거, RefCount, LRU 트리밍, IsReleasable 리셋 |

**버그 수정**:
- IsReleasable 플래그가 리셋되지 않는 문제 발견 및 수정
- 테스트 작성 과정에서 엣지 케이스 발견

**배운 점**:
- 테스트 작성이 버그 발견에 효과적
- 인터페이스 기반 설계가 테스트 작성을 용이하게 함

---

## Phase 22: LocalServer 분리

### 서버 로직 Assembly 분리

**커밋**: 여러 커밋 통합
- **중요 결정**: LocalApiClient에서 서버 로직을 Sc.LocalServer Assembly로 분리
- 354줄 → 157줄로 56% 코드 감소
- Handler/Service/Validator 계층 분리로 확장성 확보
- **상세**: [DECISIONS.md - LocalServer 서버 로직 분리 설계](DECISIONS.md) 참조

**핵심 구현**:
| 컴포넌트 | 역할 | 위치 |
|----------|------|------|
| LocalGameServer | 요청 라우팅 진입점 | LocalServer/ |
| IRequestHandler<TReq, TRes> | 핸들러 인터페이스 | LocalServer/Handlers/ |
| LoginHandler | 로그인/신규 유저 생성 | LocalServer/Handlers/ |
| GachaHandler | 가챠 확률, 천장 시스템 | LocalServer/Handlers/ |
| ShopHandler | 상점 구매 처리 | LocalServer/Handlers/ |
| ServerValidator | 서버측 재화/조건 검증 | LocalServer/Validators/ |
| ServerTimeService | 서버 시간 관리 | LocalServer/Services/ |
| GachaService | 가챠 확률 계산 | LocalServer/Services/ |
| RewardService | Delta 생성, 보상 적용 | LocalServer/Services/ |
| ResponseValidator | 클라이언트측 2차 검증 | Core/Validation/ |

**아키텍처 개선**:
```
Before:
LocalApiClient (354줄)
├── 저장/로드 로직
├── 로그인 로직
├── 가챠 로직 (확률, 천장)
└── 상점 로직

After:
LocalApiClient (157줄)              Sc.LocalServer Assembly
├── IApiClient 구현          →      LocalGameServer
├── 저장/로드                       ├── LoginHandler
└── 지연 시뮬레이션                  ├── GachaHandler
                                    ├── ShopHandler
                                    ├── ServerValidator
                                    └── Services (Time, Gacha, Reward)
```

**ResponseValidator (클라이언트측 2차 검증)**:
- 요청-응답 일관성 검증 (ProductId 일치, 결과 개수 등)
- Delta 유효성 검증 (음수 재화 체크)
- Core Assembly에 배치 (순환 참조 방지)

**배운 점**:
- 서버 로직 분리로 "서버 교체 시 어디를 바꿔야 하는가?" 명확해짐
- Handler/Service/Validator 계층 분리로 단일 책임 원칙 적용
- 클라이언트측 2차 검증으로 비정상 응답 탐지 가능

---

## 진행 중

현재 진행 중인 작업은 [PROGRESS.md](../PROGRESS.md) 참조
