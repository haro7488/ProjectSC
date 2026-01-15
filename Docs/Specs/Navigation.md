---
type: spec
assembly: Sc.Common
class: NavigationManager, INavigationContext
category: UI
status: draft
version: "2.0"
dependencies: [Sc.Data, Sc.Event, UISystem]
created: 2025-01-14
updated: 2025-01-15
---

# Navigation 시스템

## 목적

Screen과 Popup 기반의 화면 전환, 히스토리 관리, 가시성 제어 시스템.

---

## 의존성

- **참조**: INavigationContext, ScreenWidget.Context, PopupWidget.Context, Transition
- **참조됨**: 모든 Screen/Popup

---

## 클래스 역할 정의

| 클래스 | 역할 | 책임 | 비책임 |
|--------|------|------|--------|
| NavigationManager | 화면 전환 관리자 | 통합 스택 관리, 전환 처리, 가시성 제어 | UI 렌더링, State 관리 |
| INavigationContext | 공통 인터페이스 | Screen/Popup Context 추상화 | 구체적 구현 |

---

## 아키텍처

### 통합 스택 구조

```
NavigationManager (Singleton)
│
└── NavigationStack: List<INavigationContext>
     └── Screen과 Popup이 Push 순서대로 단일 스택에 저장
     └── Pop 시 역순으로 제거
```

**핵심 원칙**: Screen과 Popup은 동일한 스택 레벨에서 관리됨.

```
Push 순서: S1 → P1 → P2 → S2 → P3
Stack:     [S1, P1, P2, S2, P3]
Pop 순서:  P3 → S2 → P2 → P1 → S1
```

### INavigationContext 인터페이스

| 멤버 | 타입 | 설명 |
|------|------|------|
| ContextType | NavigationContextType | Screen 또는 Popup |
| WidgetType | Type | 실제 Widget 타입 |
| Load() | UniTask | 리소스 로드 |
| Enter() | UniTask | 화면 진입 |
| Resume() | void | 활성화 (Show) |
| Pause() | void | 비활성화 (Hide) |
| Exit() | UniTask | 화면 퇴장 |
| HandleEscape() | bool | ESC 처리 허용 여부 |

---

## 가시성 규칙

### 핵심 규칙

**Screen은 가시성 경계, Popup은 투명 레이어**.

```
가시성 경계 = 마지막 Screen의 인덱스
- 경계 이상 (>=): Show
- 경계 미만 (<): Hide
```

### 규칙 적용 예시

```
Stack: [S1, P1, P2, S2, P3, P4]
                    ^
                    마지막 Screen = S2 (index 3)

가시성:
- S1 (0): 0 >= 3? No  → Hide
- P1 (1): 1 >= 3? No  → Hide
- P2 (2): 2 >= 3? No  → Hide
- S2 (3): 3 >= 3? Yes → Show
- P3 (4): 4 >= 3? Yes → Show
- P4 (5): 5 >= 3? Yes → Show
```

**결과**: S2, P3, P4만 화면에 표시됨.

### 가시성 변경 시점

| 이벤트 | 처리 |
|--------|------|
| Push (Screen/Popup) | RefreshVisibility() 호출 |
| Pop | RefreshVisibility() 호출 |
| ClosePopup | RefreshVisibility() 호출 |
| CloseAllPopups | RefreshVisibility() 호출 |

### RefreshVisibility 알고리즘

```
1. 마지막 Screen 인덱스 찾기 (뒤에서부터 탐색)
2. 전체 스택 순회:
   - index >= lastScreenIndex → Resume() (Show)
   - index < lastScreenIndex → Pause() (Hide)
```

### 가시성 제어 방식

**Canvas.enabled 사용** (GameObject.SetActive 아님)

| 방식 | 렌더링 | OnEnable/OnDisable | 코루틴 | 권장 |
|------|--------|-------------------|--------|------|
| SetActive(false) | 중단 | 호출됨 | 중단 | X |
| Canvas.enabled = false | 중단 | 호출 안됨 | 유지 | O |
| CanvasGroup.alpha = 0 | 부분 중단 | 호출 안됨 | 유지 | 보조용 |

**Widget.Show/Hide 구현**:
```csharp
Show(): Canvas.enabled = true
Hide(): Canvas.enabled = false
```

---

## 스택 중복 제거 정책

**Push 시 동일 Screen이 스택에 존재하면 기존 Context와 그 위의 모든 항목 제거** 후 새로 Push.

```
A → B → P1 → C → P2 → B 이동 시:

Push(A): [A]
Push(B): [A, B]
Push(P1): [A, B, P1]
Push(C): [A, B, P1, C]
Push(P2): [A, B, P1, C, P2]
Push(B): B 중복 → [A, B]  // 기존 B와 그 위(P1, C, P2) 모두 제거 후 Push
```

**목적**: 순환 이동 시 스택 무한 증가 방지, 일관된 Back 동선 보장

### 중복 제거 시 처리

1. 기존 Screen 인덱스 찾기
2. 해당 인덱스부터 끝까지 모든 Context Exit() 호출
3. 스택에서 제거
4. 새 Context Push 진행
5. RefreshVisibility() 호출

---

## Shortcut

**어디서든 특정 Screen으로 바로 이동하는 기능**. 내부 동작은 일반 Push와 동일.

```
현재: A → D (로비의 다른 탭)
Shortcut(C) 클릭
결과: A → D → C

Back: C → D → A
```

**특징**:
- 정상 루트를 거치지 않고 바로 이동
- 스택 중복 제거 정책 적용
- 뒤로가기 시 Shortcut을 누른 위치로 복귀

---

## NavigationManager 인터페이스

### Push 메서드

| 멤버 | 시그니처 | 설명 |
|------|----------|------|
| Push | void Push(ScreenWidget.Context) | Screen Push (중복 제거 적용) |
| Push | void Push(PopupWidget.Context) | Popup Push |
| PushAsync | UniTask PushAsync(ScreenWidget.Context) | 비동기 Screen Push |
| PushAsync | UniTask PushAsync(PopupWidget.Context) | 비동기 Popup Push |

### Pop 메서드

| 멤버 | 시그니처 | 설명 |
|------|----------|------|
| Pop | void Pop() | 최상위 항목 제거 |
| PopAsync | UniTask PopAsync() | 비동기 Pop |
| PopTo\<T\> | void PopTo\<T\>() | 특정 Screen까지 Pop |
| ClosePopup | void ClosePopup(PopupWidget) | 특정 Popup 닫기 |
| CloseAllPopups | void CloseAllPopups() | 모든 Popup 닫기 |

### 상태 조회

| 멤버 | 타입 | 설명 |
|------|------|------|
| CurrentScreen | ScreenWidget | 최상위 Screen (Popup 제외) |
| CurrentContext | INavigationContext | 최상위 항목 |
| StackCount | int | 전체 스택 개수 |
| ScreenCount | int | Screen 개수 |
| HasPopupOnTop | bool | 최상위가 Popup인지 여부 |

### 뒤로가기

| 멤버 | 시그니처 | 설명 |
|------|----------|------|
| Back | bool Back() | HandleEscape() 확인 후 Pop |

---

## 화면 전환 흐름

### Push (새 항목 추가)

```
Push(Context)
     │
     ▼
[Screen인 경우 중복 검사]
     │
     ├── 중복 있음: 해당 Screen~끝까지 Exit + 제거
     │
     ▼
[Load → Enter]
     │
     ▼
[스택에 Context 저장]
     │
     ▼
[RefreshVisibility]
     └── 마지막 Screen 이후만 Show
```

### Pop (최상위 제거)

```
Pop()
     │
     ▼
[최상위 Context Exit]
     │
     ▼
[스택에서 제거]
     │
     ▼
[RefreshVisibility]
     └── 마지막 Screen 이후만 Show (자동 복원)
```

### RefreshVisibility

```
RefreshVisibility()
     │
     ▼
[마지막 Screen 인덱스 찾기]
     │
     ▼
[전체 스택 순회]
     │
     ├── index >= lastScreenIndex → Resume()
     │
     └── index < lastScreenIndex → Pause()
```

---

## Back() 로직

```
Back() 호출
     │
     ▼
[CurrentContext 존재?]
     │
     └── No: return false
     │
     ▼
[HandleEscape() 호출]
     │
     └── false 반환 시: return false (닫기 불허)
     │
     ▼
[ScreenCount == 1 && !HasPopupOnTop?]
     │
     └── Yes: return false (루트 Screen 보호)
     │
     ▼
[Pop()]
     │
     ▼
return true
```

---

## 가시성 흐름 예시

```
[초기 상태]

1. Push S1
   Stack: [S1]
   Visibility: S1 ✓

2. Push P1
   Stack: [S1, P1]
   Visibility: S1 ✓, P1 ✓  (Popup은 뒤가 보여야 함)

3. Push P2
   Stack: [S1, P1, P2]
   Visibility: S1 ✓, P1 ✓, P2 ✓

4. Push S2
   Stack: [S1, P1, P2, S2]
   Visibility: S1 ✗, P1 ✗, P2 ✗, S2 ✓  (새 Screen이 경계)

5. Push P3
   Stack: [S1, P1, P2, S2, P3]
   Visibility: S2 ✓, P3 ✓

6. Pop (P3)
   Stack: [S1, P1, P2, S2]
   Visibility: S2 ✓

7. Pop (S2)
   Stack: [S1, P1, P2]
   Visibility: S1 ✓, P1 ✓, P2 ✓  (자동 복원!)
```

---

## Transition

### 인터페이스

| 멤버 | 타입 | 설명 |
|------|------|------|
| OutScreen | Screen | 나가는 Screen |
| InScreen | Screen | 들어오는 Screen |
| Out() | UniTask | 아웃 애니메이션 |
| In() | UniTask | 인 애니메이션 |
| Default | Transition | 기본 Transition (static) |

### 제공 Transition

| 클래스 | 설명 |
|--------|------|
| FadeTransition | 페이드 인/아웃 |
| SlideTransition | 슬라이드 |
| EmptyTransition | 즉시 전환 (애니메이션 없음) |

---

## 사용 패턴

```csharp
// Screen 이동
LobbyScreen.Open(new LobbyState { ActiveTabIndex = 2 });

// Popup 열기
ConfirmPopup.Open(new ConfirmState { Title = "확인", Message = "진행?" });

// 뒤로가기
NavigationManager.Instance.Back();
```

---

## 설계 원칙

1. **통합 스택**: Screen과 Popup을 동일 레벨에서 관리
2. **Screen = 가시성 경계**: 새 Screen Push 시 이전 항목들 숨김
3. **Popup = 투명 레이어**: Popup Push 시 이전 항목들 유지
4. **Canvas.enabled 제어**: OnEnable/OnDisable 부작용 없음
5. **자동 복원**: Pop 시 RefreshVisibility로 가시성 자동 결정

---

## 주의사항

- 루트 Screen은 Pop 불가 (ScreenCount == 1일 때)
- HandleEscape()가 false 반환하면 Back()으로 닫히지 않음
- Screen/Popup에 Canvas 컴포넌트 필수 (가시성 제어용)
- Screen 전환 중 추가 전환 요청 시 무시됨 (_isTransitioning 플래그)

---

## 관련

- [UISystem.md](Common/UISystem.md)
- [Common.md](Common.md)
