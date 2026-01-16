# 의사결정 로그

프로젝트의 주요 기술적 결정과 그 배경을 기록합니다.

---

## 작성 가이드

각 의사결정은 다음 형식으로 기록:

```markdown
## [결정 제목]

**일자**: YYYY-MM-DD
**상태**: 결정됨 / 검토중 / 폐기됨
**관련 커밋**: `해시` 또는 PR 링크

### 컨텍스트
- 어떤 상황에서 이 결정이 필요했는가?

### 선택지
1. **선택지 A** - 장점 / 단점
2. **선택지 B** - 장점 / 단점
3. **선택지 C** - 장점 / 단점

### 결정
- 어떤 선택을 했는가?
- 왜 이 선택을 했는가?

### 결과
- 결정 후 어떤 영향이 있었는가?
- 예상과 다른 점은?

### 회고 (선택)
- 다시 결정한다면?
- 배운 점?
```

---

## UI 아키텍처: MVP → Widget

**일자**: 2024-XX-XX
**상태**: 결정됨
**관련 커밋**: `6792d60`

### 컨텍스트
- 초기 UI 설계에서 MVP(Model-View-Presenter) 패턴 검토
- Unity의 GameObject 기반 UI와 전통적 MVP의 괴리 발견

### 선택지
1. **MVP 패턴 유지**
   - 장점: 테스트 용이, 관심사 분리 명확
   - 단점: Unity UI와 맞지 않는 추상화, 보일러플레이트 증가

2. **Widget 기반 시스템**
   - 장점: Unity 친화적, 계층 구조 자연스러움
   - 단점: 테스트가 MonoBehaviour에 종속

3. **MVVM 패턴**
   - 장점: 데이터 바인딩 강점
   - 단점: Unity에서 구현 복잡, 오버엔지니어링 우려

### 결정
**Widget 기반 시스템** 선택

**이유**:
- Unity의 GameObject 계층 구조와 자연스럽게 매핑
- Prefab 기반 UI 워크플로우와 호환
- 실용적인 수준의 복잡도 유지

### 결과
- Screen/Popup/Navigation 구조가 직관적으로 구현됨
- Prefab 기반 작업 흐름 유지
- 테스트는 PlayMode 테스트로 대응

---

## Assembly Definition 모듈화 전략

**일자**: 2024-XX-XX
**상태**: 결정됨
**관련 커밋**: `968b75d`

### 컨텍스트
- 프로젝트 규모 성장에 따른 컴파일 시간 증가 우려
- 코드 의존성 명시적 관리 필요

### 선택지
1. **Assembly 없이 진행**
   - 장점: 설정 단순
   - 단점: 컴파일 시간 증가, 순환 참조 위험

2. **기능별 Assembly 분리**
   - 장점: 컴파일 최적화, 의존성 명시
   - 단점: 초기 설정 비용

3. **Layer별 Assembly 분리**
   - 장점: 아키텍처 레이어 강제
   - 단점: 기능 추가 시 여러 Assembly 수정 필요

### 결정
**기능별 Assembly 분리** (`Sc.` 접두사)

**이유**:
- 각 기능이 독립적으로 컴파일/테스트 가능
- 의존성 그래프 명시적 관리
- 순환 참조 컴파일 타임 방지

### 결과
- Core, UI, Contents 등 명확한 모듈 경계
- 새 기능 추가 시 의존성 설계 강제
- Editor 전용 코드 완전 분리 가능

---

## 이벤트 기반 컨텐츠 통신

**일자**: 2024-XX-XX
**상태**: 결정됨
**관련 커밋**: `f5a5f11`

### 컨텍스트
- 컨텐츠(캐릭터, 인벤토리, 퀘스트 등) 간 통신 방식 필요
- 직접 참조 시 강한 결합 발생

### 선택지
1. **직접 참조**
   - 장점: 단순, IDE 지원 좋음
   - 단점: 강한 결합, 순환 참조 위험

2. **이벤트 버스**
   - 장점: 느슨한 결합, 확장 용이
   - 단점: 흐름 추적 어려움, 타입 안전성 감소

3. **메시지 브로커 패턴**
   - 장점: 중앙 집중 관리
   - 단점: 복잡도 증가

### 결정
**이벤트 버스** (타입 안전 이벤트)

**이유**:
- 컨텐츠 간 의존성 제거
- 새 컨텐츠 추가 시 기존 코드 수정 최소화
- 제네릭 기반 타입 안전성 확보

### 결과
- Contents 간 Assembly 참조 없음
- 기능 추가/제거 시 영향 범위 최소화
- 이벤트 정의를 통한 계약 명시

---

## 데이터 아키텍처: 로컬 중심 → 서버 중심

**일자**: 2025-01-15
**상태**: 결정됨
**관련 커밋**: `0da3d36`

### 컨텍스트
- 초기 데이터 아키텍처(v1.0)는 로컬 중심 설계
- IDataService로 클라이언트에서 직접 데이터 수정 가능
- 라이브 서비스 기준으로 보면 서버 검증 없는 설계는 치팅에 취약
- 포트폴리오 목적상 실제 서비스와 유사한 아키텍처 필요

### 선택지
1. **로컬 중심 유지 (v1.0)**
   - 장점: 구현 단순, 빠른 개발
   - 단점: 서버 검증 불가, 치팅 취약, 실무와 괴리

2. **서버 중심 + 로컬 시뮬레이션 (v2.0)**
   - 장점: 라이브 서비스 아키텍처, 인터페이스만 교체하면 서버 연동
   - 단점: 초기 구현 복잡도 증가

3. **실제 서버 구현**
   - 장점: 완전한 서버-클라이언트 구조
   - 단점: 범위 초과, 백엔드 인프라 필요

### 결정
**서버 중심 + 로컬 시뮬레이션 (v2.0)** 선택

**이유**:
- 설계는 서버 중심(Server Authority)으로 하되, 구현은 LocalApiClient로 시뮬레이션
- 인터페이스(IApiClient)만 교체하면 실제 서버 연동 가능
- Delta 패턴으로 부분 갱신 효율화
- 포트폴리오에서 "라이브 서비스 고려한 설계" 어필 가능

### 결과
- DataManager: 읽기 전용 뷰만 제공 (직접 수정 메서드 제거)
- SetUserData: 로그인 시 전체 데이터 설정
- ApplyDelta: 이후 액션은 변경분만 적용
- LocalApiClient: 서버 응답 시뮬레이션 (100ms 지연, JSON 저장)

### 회고
- v1.0 구현 후 리셋하는 비용 발생 (git reset 사용)
- 초기 설계 단계에서 라이브 서비스 기준으로 검토했으면 리워크 방지 가능
- **배운 점**: 포트폴리오여도 실제 서비스 기준으로 설계해야 가치 있음

---

## Delta 패턴 도입

**일자**: 2025-01-15
**상태**: 결정됨
**관련 커밋**: `0da3d36`

### 컨텍스트
- 서버 중심 아키텍처에서 유저 데이터 갱신 방식 결정 필요
- 매번 전체 데이터 동기화는 비효율적

### 선택지
1. **전체 동기화**
   - 장점: 구현 단순, 데이터 일관성 보장
   - 단점: 네트워크/메모리 비효율, 대역폭 낭비

2. **Delta 패턴 (변경분만 전달)**
   - 장점: 네트워크 효율, 부분 갱신으로 성능 향상
   - 단점: 적용 로직 복잡, Nullable 처리 필요

3. **이벤트 소싱**
   - 장점: 변경 이력 추적, 롤백 가능
   - 단점: 오버엔지니어링, 구현 복잡도 높음

### 결정
**Delta 패턴** 선택

**이유**:
- 실제 모바일 게임에서 보편적으로 사용하는 패턴
- Nullable 타입으로 변경 여부 명확히 표현
- 로그인: 전체 데이터, 이후 액션: Delta만

### 결과
- UserDataDelta 클래스 설계 (Profile?, Currency?, AddedCharacters, RemovedCharacterIds, ...)
- DataManager.ApplyDelta() 구현
- GachaResponse, ShopPurchaseResponse 등에 Delta 포함

---

## Unity 기본 컴포넌트 Widget화

**일자**: 2025-01-15
**상태**: 결정됨
**관련 커밋**: `548caf3`

### 컨텍스트
- MVP 화면 구현 전, UI 컴포넌트 사용 패턴 정립 필요
- Unity 기본 컴포넌트(Button, Text 등)를 Screen/Popup에서 일관되게 사용할 방법 필요
- Widget 시스템의 라이프사이클(Initialize, Bind, Show/Hide, Release)과 통합 필요

### 선택지
1. **Unity 컴포넌트 직접 사용**
   - 장점: 추가 코드 없음, 단순
   - 단점: 라이프사이클 불일치, 리스너 관리 분산, 일관성 부족

2. **Widget 래퍼 클래스 생성**
   - 장점: 일관된 라이프사이클, 이벤트 관리 중앙화, 재사용 용이
   - 단점: 래퍼 코드 작성 비용, 간접 레이어 추가

3. **확장 메서드로 기능 추가**
   - 장점: 기존 컴포넌트 그대로 사용
   - 단점: Widget 계층 구조와 통합 어려움

### 결정
**Widget 래퍼 클래스 생성** 선택

**이유**:
- Widget 시스템의 Composition 패턴과 자연스럽게 통합
- OnInitialize에서 리스너 등록, OnRelease에서 정리 → 메모리 누수 방지
- OnBind로 데이터 바인딩 표준화
- Screen/Popup에서 일관된 방식으로 UI 컴포넌트 사용 가능

### 결과
- 8개 기본 Widget 구현: Text, Button, Image, Slider, Toggle, InputField, ProgressBar, ScrollView
- 각 Widget이 Unity 컴포넌트를 래핑하고 Widget 라이프사이클 준수
- MVP 화면 구현 시 일관된 패턴으로 UI 구성 가능

### 회고
- 초기에는 "오버엔지니어링 아닌가?" 고민
- 실제 구현해보니 Screen/Popup 코드가 훨씬 깔끔해짐
- **배운 점**: 기반 컴포넌트에 투자하면 상위 레이어 구현이 단순해짐

---

## Screen/Popup 인스턴스 로딩 방식

**일자**: 2025-01-16
**상태**: 결정됨
**관련 커밋**: `a8e5807`

### 컨텍스트
- NavigationManager가 Screen/Popup을 Push할 때 인스턴스를 어떻게 얻을지 결정 필요
- 기존 Context.Load()는 TODO 상태로 구현되지 않음
- MVP 테스트를 위해 빠른 구현 필요

### 선택지
1. **ScreenProvider 패턴 (Addressables)**
   - 장점: 런타임 로딩, 메모리 효율
   - 단점: 구현 복잡, Addressables 설정 필요

2. **Resources.Load 방식**
   - 장점: 구현 단순
   - 단점: Resources 폴더 제약, 빌드 크기 증가

3. **씬 배치 + FindObjectOfType**
   - 장점: 즉시 구현 가능, 에디터에서 확인 용이
   - 단점: 모든 UI가 씬에 존재해야 함, 메모리 사용

### 결정
**씬 배치 + FindObjectOfType** 선택 (MVP 단계)

**이유**:
- MVP 테스트가 우선 목표
- 에디터에서 UI 구조 즉시 확인 가능
- 나중에 ScreenProvider로 마이그레이션 가능 (인터페이스 동일)

### 결과
- Context.Load()에서 FindObjectOfType<TScreen>(true) 사용
- MVPSceneSetup에서 모든 Screen/Popup을 씬에 미리 배치
- Canvas.enabled로 가시성 제어 (SetActive 대신)

### 회고
- 빠른 프로토타이핑에 효과적
- 추후 Addressables 기반 Provider로 전환 예정
- **배운 점**: MVP 단계에서는 완벽한 구조보다 동작하는 코드가 우선

---

## 에디터 설정 ScriptableObject 도입

**일자**: 2025-01-16
**상태**: 결정됨
**관련 커밋**: `87b25d6`

### 컨텍스트
- MVP 프리팹 생성 시 기본 폰트(PretendardVariable) 적용 필요
- 에디터 도구에서 공통 설정 관리 필요

### 선택지
1. **하드코딩**
   - 장점: 단순
   - 단점: 변경 시 코드 수정 필요

2. **EditorPrefs**
   - 장점: 빌트인 기능
   - 단점: 프로젝트 간 공유 어려움, 타입 제한

3. **ScriptableObject 기반 설정**
   - 장점: Inspector에서 편집, 버전 관리 가능
   - 단점: 에셋 파일 필요

### 결정
**ScriptableObject 기반 설정** 선택

**이유**:
- 기본 폰트, 색상 등 시각적 설정은 Inspector에서 편집이 편리
- 프로젝트와 함께 버전 관리 가능
- 싱글턴 접근으로 어디서든 사용 가능

### 결과
- ProjectEditorSettings SO 생성
- SC Tools > Settings 메뉴 추가
- MVPSceneSetup에서 TMP 생성 시 기본 폰트 자동 적용

---

## Navigation API 간소화: Open() 패턴

**일자**: 2026-01-16
**상태**: 결정됨
**관련 커밋**: `1eb467b`

### 컨텍스트
- Screen/Popup 이동 시 코드가 장황함
- `NavigationManager.Instance?.Push(LobbyScreen.CreateContext(new LobbyState()))` 형식
- 반복적인 보일러플레이트 코드 발생
- 개발자 경험(DX) 개선 필요

### 선택지
1. **현행 유지 (장황한 형식)**
   - 장점: 명시적, Context 빌더 패턴 활용 가능
   - 단점: 코드 장황, 오타 가능성, 반복 작업

2. **static Open() 메서드 추가**
   - 장점: 간결한 API, 직관적, 타이핑 감소
   - 단점: 내부에서 NavigationManager 직접 접근

3. **확장 메서드**
   - 장점: 기존 구조 유지
   - 단점: 발견성 낮음, IDE 지원 제한적

### 결정
**static Open() 메서드** 선택

**이유**:
- `LobbyScreen.Open(state)` 형식이 가장 직관적
- Flutter의 `Navigator.push()` 대신 `Screen.go()` 패턴과 유사
- Transition 필요 시 두 번째 인자로 전달 가능
- 기존 CreateContext 빌더 패턴도 유지 (고급 사용)

### 결과
```csharp
// Before (장황)
NavigationManager.Instance?.Push(LobbyScreen.CreateContext(new LobbyState()));

// After (간결)
LobbyScreen.Open(new LobbyState());
```

- ScreenWidget/PopupWidget에 Open(), Push() static 메서드 추가
- 내부에서 NavigationManager.Instance?.Push() 호출
- 모든 기존 코드 마이그레이션 완료

### 회고
- API 설계에서 "사용하는 쪽의 코드"를 먼저 상상하면 좋은 설계가 나옴
- **배운 점**: 간결한 API가 생산성과 가독성 모두 향상시킴

---

## Screen/Popup Transition 애니메이션 설계

**일자**: 2025-01-17
**상태**: 결정됨
**관련 커밋**: `45ae44e` 등 8개

### 컨텍스트
- Screen/Popup 전환 시 애니메이션 없이 즉시 전환되어 사용자 경험 부족
- CrossFade 형태의 부드러운 화면 전환 필요
- DOTween이 이미 프로젝트에 포함되어 있음

### 선택지
1. **Unity Animation/Animator**
   - 장점: Unity 네이티브, Timeline 연동 가능
   - 단점: 설정 복잡, 프리팹마다 Animator 필요, 코드 제어 어려움

2. **DOTween 직접 사용**
   - 장점: 코드 기반으로 유연, 이미 프로젝트에 포함
   - 단점: Transition 추상화 없이 사용하면 코드 분산

3. **Transition 추상화 + DOTween**
   - 장점: 일관된 인터페이스, 확장 용이, Context와 자연스럽게 통합
   - 단점: 추상화 레이어 추가

### 결정
**Transition 추상화 + DOTween** 선택

**이유**:
- Context 패턴과 자연스럽게 통합 (Context.Enter/Exit에서 Transition 호출)
- 새로운 Transition 타입 추가 용이 (SlideTransition, ScaleTransition 등)
- Screen과 Popup이 동일한 인터페이스로 Transition 사용 가능
- Widget.CachedCanvasGroup으로 성능 최적화

### 결과
```csharp
// Transition 베이스 클래스
public abstract class Transition
{
    public abstract UniTask Enter(Widget widget);
    public abstract UniTask Exit(Widget widget);
}

// 사용 예시
LobbyScreen.Open(state, new FadeTransition(0.3f));
ConfirmPopup.Open(state, new PopupScaleTransition());
```

- Transition, FadeTransition (Screen용)
- PopupTransition, PopupScaleTransition (Popup용)
- Widget.CachedCanvasGroup 프로퍼티 추가

### 회고
- Assembly Definition 문제로 DOTween.Modules 참조 추가 필요
- UniTask.DOTween 통합에서 UNITASK_DOTWEEN_SUPPORT define 이슈 발생
- **배운 점**: 외부 플러그인 사용 시 asmdef 의존성 확인 필수

---

## 보상 시스템 (RewardType) 설계

**일자**: 2026-01-17
**상태**: 결정됨
**관련 커밋**: (Phase 1 구현 시 추가)

### 컨텍스트
- 아웃게임 아키텍처(상점, 스테이지, 이벤트)에서 공통으로 사용할 보상 시스템 필요
- 수집형 RPG의 다양한 보상 타입(재화, 아이템, 캐릭터 등) 처리 방법 결정 필요
- 기획 데이터(장비, 아이템, 재화 등 분리)와 런타임(Thing 통합) 간 구조 정립 필요

### 선택지
1. **세분화된 RewardType (8개+)**
   - Currency, Character, CharacterShard, Equipment, Consumable, Material, Skin, PlayerExp...
   - 장점: RewardType만으로 UI 분기 가능
   - 단점: 타입 많아짐, 처리 로직 중복

2. **Thing 기반 통합**
   - Currency, Thing, Character, PlayerExp (4개)
   - 장점: 단순, 런타임 Thing 개념과 매핑
   - 단점: 세부 구분을 위해 추가 조회 필요

3. **처리 방식 기준 분류**
   - Currency, Item, Character, PlayerExp (4개)
   - Item 내부에서 ItemCategory로 세분화 (Equipment, Consumable, Material, CharacterShard, Furniture, Ticket)
   - 장점: 처리 로직 기준 분류 + UI용 세부 분류 분리
   - 단점: 2레벨 구조

### 결정
**처리 방식 기준 분류 (선택지 3)** 선택

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

**이유**:
- 보상 처리 로직이 동일한 것들을 같은 RewardType으로 분류 (모든 Item은 인벤토리에 수량 추가)
- UI 표시용 세부 구분은 ItemCategory로 처리 (ItemData에서 조회)
- 확장 시 ItemCategory만 추가하면 됨 (RewardType 변경 불필요)

### 추가 결정사항

**장비 시스템**:
- 인벤토리 장비: 수량 기반 (Thing)
- 장착된 장비: 인스턴스 기반 (강화 상태 등 고유 데이터)
- 장착 시 인벤토리에서 차감 → 캐릭터에 인스턴스 생성

**스킨 시스템**:
- 스킨 = 별도 캐릭터로 처리 (수영복 아스나 ≠ 기본 아스나)
- CharacterData에 BaseCharacterId 필드로 원본 연결
- ItemCategory에서 Skin 제외

**캐릭터 경험치**:
- 직접 지급 없음
- 경험치 아이템(Item/Consumable)으로 처리

### 결과
- RewardType 4개로 단순화
- ItemCategory 6개로 세분화
- 기획 데이터 분류와 런타임 처리 로직 분리
- 수집형 RPG 표준 패턴(블루아카, FGO 등) 참고하여 검증 완료

---

## 서버/클라이언트 영역 분리 (Sc.LocalServer)

**일자**: 2026-01-17
**상태**: 결정됨
**관련 커밋**: (Phase 1 구현 시 추가)

### 컨텍스트
- 현재 LocalApiClient에 서버 로직(가챠 확률, 재화 검증 등)과 클라이언트 코드가 혼재
- 프로젝트 규모 확대 시 서버/클라이언트 영역 구분이 모호해질 우려
- 2중 검증 필요: 서버 검증 + 클라이언트에서 응답 검증 (불일치 시 에러 처리)

### 선택지
1. **현행 유지 (LocalApiClient에 통합)**
   - 장점: 단순, 파일 적음
   - 단점: 영역 구분 모호, 규모 확대 시 유지보수 어려움

2. **Sc.LocalServer Assembly 분리**
   - 장점: 서버/클라 명확 분리, 실제 서버 교체 시 명확한 경계
   - 단점: 초기 구조 설정 비용

3. **폴더로만 분리**
   - 장점: Assembly 추가 없이 구조화
   - 단점: 의존성 강제 불가, 실수로 경계 침범 가능

### 결정
**Sc.LocalServer Assembly 분리 (선택지 2)** 선택

**구조**:
```
Sc.LocalServer (서버 모사 영역)
├── LocalGameServer.cs      # 진입점
├── Handlers/
│   ├── LoginHandler.cs
│   ├── GachaHandler.cs
│   ├── ShopHandler.cs
│   └── StageHandler.cs
├── Validators/
│   └── ServerValidator.cs  # 서버 측 검증
└── Services/
    ├── RewardService.cs    # Delta 생성
    └── GachaService.cs     # 확률 계산

Sc.Core (클라이언트)
├── ResponseValidator.cs    # 2차 검증 (요청-응답 일관성)
└── Utility/
    └── RewardHelper.cs     # UI 헬퍼 (포맷팅, 아이콘)
```

**이유**:
- Assembly 경계로 서버/클라 코드 분리 강제
- 실제 서버 연동 시 LocalApiClient만 RemoteApiClient로 교체
- 클라이언트 2차 검증으로 응답 신뢰성 확보 (불일치 시 타이틀 이동)

### 2차 검증 (ResponseValidator) 범위
- **요청-응답 일관성 검증** (선택)
  - 요청한 PoolId와 응답 결과 매칭
  - 요청 PullType(1회/10회)과 결과 개수 일치
  - 재화 차감량과 상품 가격 일치
- 검증 실패 시 ErrorHandler → 타이틀 화면 이동

### 결과
- 서버 로직: Sc.LocalServer (Delta 생성, 게임 로직)
- 클라 로직: Sc.Core (응답 검증, UI 헬퍼)
- RewardProcessor → RewardService(서버) + RewardHelper(클라)로 분리

---

## SystemPopup 하이브리드 구조

**일자**: 2026-01-17
**상태**: 결정됨
**관련 커밋**: (Phase 1 구현 시 추가)

### 컨텍스트
- 확인/취소, 알림, 입력, 재화 소비 확인 등 다양한 시스템 팝업 필요
- ConfirmPopup 하나로 모든 케이스 처리 vs 기능별 분리 결정 필요

### 선택지
1. **분리형 (기능별 별도 팝업)**
   - 장점: 단순한 책임, 타입 안전
   - 단점: 팝업 개수 증가, 공통 로직 중복

2. **통합형 (SystemPopup + Mode)**
   - 장점: 일관된 API, 공통 로직 한 곳
   - 단점: State 비대화, 모드별 분기 복잡

3. **하이브리드 (Base + 특화)**
   - 장점: 공통 로직 Base 관리 + 타입 안전
   - 단점: 클래스 여러 개

### 결정
**하이브리드 (선택지 3)** 선택

**구조**:
```
SystemPopupBase (추상)
├── ConfirmPopup      # 확인/취소
├── AlertPopup        # 알림 (Info/Warning/Error)
├── InputPopup        # 텍스트 입력
└── CostConfirmPopup  # 재화 소비 확인
```

**추가 결정**:
- ButtonStyle enum 추가 (Default, Primary, Danger, Secondary)
- 위험한 작업(삭제 등)에 Danger 스타일 적용
- 프리팹은 팝업별 별도 관리
- SelectPopup은 추후 필요 시 추가

### 결과
- SystemPopupBase: 공통 요소 (Title, Message, 애니메이션)
- 각 팝업: 특화 필드만 보유
- ButtonStyle로 시각적 구분 (Danger = 빨간 버튼)

---

## RewardPopup 좌우 스크롤 카드형

**일자**: 2026-01-17
**상태**: 결정됨
**관련 커밋**: (Phase 1 구현 시 추가)

### 컨텍스트
- 보상 획득 팝업 UI 방식 결정 필요
- 수집형 RPG 5개 게임 비교 분석 (원신, 블루아카, 명일방주, FGO, 니케)

### 선택지
1. **리스트형 (원신/명일방주)**
   - 장점: 심플, 정보 전달 중심
   - 단점: 특별한 보상도 평범하게 보임

2. **그리드형 (블루아카)**
   - 장점: 시각적, 카드 컬렉션 느낌
   - 단점: 많은 보상 시 스크롤 필요

3. **좌우 스크롤 카드형 (캐러셀)**
   - 장점: 각 보상 크게 강조, 모바일 UX 자연스러움
   - 단점: 많은 보상 시 스와이프 번거로움

### 결정
**좌우 스크롤 카드형 (선택지 3)** 선택

**레이아웃**:
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

**추가 결정**:
- 정렬 순서: 희귀도 순 (캐릭터 > 장비 > 재료 > 재화)
- 전체 보기: 그리드 형태 서브 팝업 (4열)
- Skip: 빠른 스와이프로 넘기기 (별도 버튼 없음)
- 신규 아이템: NEW 뱃지 + 금색 프레임

### 결과
- RewardPopup: 메인 (좌우 스크롤 카드)
- RewardFullViewPopup: 전체 보기 (그리드)
- RewardCard: 개별 카드 위젯

---

## 시즌패스 구현 방향 및 범위

**일자**: 2026-01-17
**상태**: 결정됨
**관련 커밋**: (이후 마일스톤에서 추가)

### 컨텍스트
- Phase 2 상점 설계 중 시즌패스(Pass) 포함 여부 논의
- 시즌패스 구현 방법론 검토 필요

### 선택지

**Q1. 시즌패스 유형**
1. **Type A: 단순 구독형** (블루아카 월간 패키지)
   - 장점: 구현 간단, Shop 확장으로 충분
   - 단점: 컨텐츠 빈약, 참여 유도 부족

2. **Type B: 레벨형 보상트랙** (명일방주 월간 카드)
   - 장점: 무료/프리미엄 트랙 분리, 중간 복잡도
   - 단점: 별도 시스템 필요

3. **Type C: 미션 기반형** (원신 기행, 니케 미션패스)
   - 장점: 높은 참여 유도, 플레이 목표 제공
   - 단점: 미션 시스템 + 포인트 시스템 + 보상트랙 필요 (높은 복잡도)

**Q2. 현재 마일스톤(OUTGAME-V1) 포함 여부**
1. **포함 (Type A만)**: Shop에서 단순 구독형 처리
2. **제외**: 별도 마일스톤으로 분리
3. **설계만**: 구조만 잡고 구현은 이후로

### 결정
- **Q1**: **Type C (미션 기반형)** 선택
- **Q2**: **제외 (별도 마일스톤)** 선택

**이유**:
- 시즌패스는 유저 참여 유도가 핵심 → 미션 기반이 효과적
- 미션 기반형은 별도 시스템 필수 (미션, 포인트, 보상트랙)
- 현재 OUTGAME-V1 범위에 포함 시 복잡도 과다
- 핵심 기능(상점, 스테이지, 이벤트) 우선 완성 후 확장

### 결과
- ProductType에서 `Pass` 제외 (Currency, Item, Package, Character만)
- 시즌패스는 별도 마일스톤 `PASS-V1`으로 분리 예정
- 예상 구조:
  ```
  Sc.Contents.Pass/
  ├── Data/
  │   ├── PassData.cs           # 패스 정의
  │   ├── PassTierData.cs       # 보상 트랙 티어
  │   └── PassMissionData.cs    # 미션 정의
  ├── PassManager.cs            # 패스 상태 관리
  ├── PassScreen.cs             # 패스 전용 UI
  └── PassRewardClaimer.cs      # 보상 수령 로직
  ```

---

## TimeService 시간 처리 설계

**일자**: 2026-01-17
**상태**: 결정됨
**관련 커밋**: (Phase 1 구현 시 추가)

### 컨텍스트
- 상점 구매 제한 (Daily/Weekly/Monthly) 리셋 시간 처리 필요
- 이벤트 시작/종료, 출석 체크, 스태미나 회복 등 시간 기반 로직 다수
- 일관된 시간 처리 방식 필요

### 선택지

**구현 방식**
1. **TimeService (중앙 집중)**
   - 장점: 일관성, 테스트 용이 (시간 Mock), 서버 시간 동기화 한 곳 관리
   - 단점: 초기 설정 필요

2. **분산 처리 (각 시스템에서 직접)**
   - 장점: 각 시스템 단순
   - 단점: 구현 불일치 가능, 중복 코드

**UI 타임존**
- A. UTC 표시
- B. 로컬 시간 변환

**TimeService 위치**
- A. Sc.Core만 (클라이언트)
- B. Sc.LocalServer만 (서버)
- C. 둘 다 (인터페이스 공유, 구현 분리)

### 결정
- 구현 방식: **TimeService (선택지 1)**
- UI 타임존: **로컬 시간 변환 (B)**
- 위치: **둘 다 (C)** - ITimeService 인터페이스 공유

**구조**:
```
Sc.Core/
├── Interfaces/ITimeService.cs    # 공유 인터페이스
├── Services/TimeService.cs       # 클라이언트 구현
└── Utility/TimeHelper.cs         # UI 표시 헬퍼

Sc.LocalServer/
└── Services/ServerTimeService.cs # 서버 구현
```

**리셋 기준**: UTC 0시
- Daily: 매일 UTC 00:00
- Weekly: 매주 월요일 UTC 00:00
- Monthly: 매월 1일 UTC 00:00

### 결과
- 시간 관련 로직 일관성 확보
- 테스트 시 ITimeService Mock으로 시간 조작 가능
- UI는 로컬 시간으로 표시하여 사용자 친화적

---

## [템플릿] 새 의사결정

**일자**: YYYY-MM-DD
**상태**: 검토중
**관련 커밋**: ``

### 컨텍스트
-

### 선택지
1. **선택지 A**
   - 장점:
   - 단점:

### 결정


### 결과

