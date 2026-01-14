---
type: spec
assembly: Sc.Core
class: NavigationManager, ScreenStack, PopupStack
category: System
status: draft
version: "1.0"
dependencies: [Sc.Data, Sc.Event, UISystem]
created: 2025-01-14
updated: 2025-01-15
---

# Navigation 시스템

## 개요

Screen과 Popup 기반의 화면 전환 및 히스토리 관리 시스템.

- **Screen 전환**: 전체 화면 스택 관리
- **Popup 관리**: Screen별 Popup 스택 그룹화
- **히스토리**: Context 기반 상태 보존 및 복구

---

## 핵심 개념

| 개념 | 설명 |
|------|------|
| **NavigationManager** | Screen/Popup 스택 관리, 화면 전환 처리 |
| **ScreenStack** | Screen Context 스택 (LIFO, 중복 제거) |
| **PopupStack** | Screen별 Popup Context 스택 (그룹화) |
| **Screen.Context** | Screen 상태 캡슐화 (State + Transition) |
| **Popup.Context** | Popup 상태 캡슐화 (State + Order) |

### 스택 중복 제거 정책

**Push 시 동일 Screen이 스택에 존재하면 기존 Context를 제거**하고 새로 Push.

```
A → B → C → B → C → D → C 이동 시:

Push(A): [A]
Push(B): [A, B]
Push(C): [A, B, C]
Push(B): B 중복 → [A, C, B]      // 기존 B 제거 후 Push
Push(C): C 중복 → [A, B, C]      // 기존 C 제거 후 Push
Push(D): [A, B, C, D]
Push(C): C 중복 → [A, B, D, C]   // 기존 C 제거 후 Push

결과: [A, B, D, C]
Back 순서: C → D → B → A
```

**목적**: 순환 이동 시 스택 무한 증가 방지, 직관적인 뒤로가기 경로 제공

### Shortcut

**어디서든 특정 Screen으로 바로 이동하는 기능**. 내부 동작은 일반 Push와 동일.

```csharp
// Shortcut 버튼 예시 (로비 어디서든 접근 가능)
public void OnShopShortcutClick()
{
    ShopScreen.Open();  // 일반 Push와 동일
}
```

```
현재: A → D (로비의 다른 탭)
Shortcut(C) 클릭
결과: A → D → C

Back: C → D → A
```

**특징**:
- 정상 루트(A→B→C)를 거치지 않고 바로 이동
- 스택 중복 제거 정책에 따라 자동 정리
- 뒤로가기 시 Shortcut을 누른 위치로 복귀

---

## 아키텍처

```
NavigationManager (Singleton)
│
├── ScreenStack
│    └── List<Screen.Context>
│         ├── LobbyScreen.Context (State 포함)
│         ├── ShopScreen.Context
│         └── BattleScreen.Context
│
└── PopupStack
     └── Dictionary<ScreenName, List<Popup.Context>>
              │
              ├── "LobbyScreen"
              │    ├── SettingsPopup.Context
              │    └── ConfirmPopup.Context
              │
              └── "BattleScreen"
                   └── PausePopup.Context
```

---

## NavigationManager

### 인터페이스

```csharp
public class NavigationManager : MonoSingleton<NavigationManager>
{
    // Screen 관리
    public void Push(Screen.Context context);
    public UniTask PushAsync(Screen.Context context);
    public void Pop();
    public UniTask PopAsync();
    public void PopTo<TScreen>() where TScreen : Screen;

    // Popup 관리
    public void PushPopup(Popup.Context context);
    public UniTask PushPopupAsync(Popup.Context context);
    public void ClosePopup(Popup popup);
    public void CloseAllPopups();

    // 상태 조회
    public Screen CurrentScreen { get; }
    public bool HasPopup { get; }
    public int ScreenStackCount { get; }

    // 뒤로가기
    public bool Back();  // Popup 있으면 Popup 닫기, 없으면 Screen Pop
}
```

### 사용 예시

```csharp
// Screen 이동
NavigationManager.Instance.Push(
    LobbyScreen.CreateContext(new LobbyState { ActiveTabIndex = 2 })
);

// 또는 Screen 정적 메서드 사용
LobbyScreen.Open(new LobbyState { ActiveTabIndex = 2 });

// Popup 열기
ConfirmPopup.Open(new ConfirmState
{
    Title = "확인",
    Message = "진행하시겠습니까?",
    OnConfirm = () => DoSomething()
});

// 뒤로가기
NavigationManager.Instance.Back();
```

---

## Screen 전환 흐름

### Push (새 Screen으로 이동)

```
Push(BattleScreen.Context)
        │
        ▼
[중복 검사]
   └── 스택에 동일 Screen 존재?
        │
        ├── Yes: 기존 Context 제거
        │        └── 해당 Screen의 Popup들도 제거
        │
        └── No: 진행
        │
        ▼
[현재 Screen Pause]
   └── CurrentScreen.Pause()
   └── 현재 Screen의 Popup들 Pause
        │
        ▼
[Transition Out]
        │
        ▼
[새 Screen Load]
   └── Provider에서 Screen 가져오기
        │
        ▼
[새 Screen Enter]
   └── OnBind(State) 호출
        │
        ▼
[새 Screen Resume]
        │
        ▼
[Transition In]
        │
        ▼
[스택에 Context 저장]
```

### Pop (이전 Screen으로 복귀)

```
Pop()
        │
        ▼
[현재 Screen Exit]
   └── GetState() → State 저장
   └── Provider로 반환
   └── 현재 Screen의 Popup들 Exit
        │
        ▼
[Transition Out]
        │
        ▼
[이전 Screen Resume]
   └── 스택에서 Context 꺼내기
   └── Screen이 Exit 상태면 다시 Load → Enter
   └── Pause 상태면 Resume만
        │
        ▼
[이전 Screen의 Popup들 Resume]
        │
        ▼
[Transition In]
```

---

## Popup 관리

### Screen별 그룹화

```csharp
// Popup은 현재 Screen에 소속됨
public void PushPopup(Popup.Context context)
{
    var screenName = CurrentScreen.GetType().Name;

    if (!_popupStacks.ContainsKey(screenName))
        _popupStacks[screenName] = new List<Popup.Context>();

    _popupStacks[screenName].Add(context);
    // ...
}
```

### Screen 전환 시 Popup 처리

```
[LobbyScreen 활성]
├── SettingsPopup (활성)
└── ConfirmPopup (활성)

        │ Push(BattleScreen)
        ▼

[BattleScreen 활성]
└── (Popup 없음)

[LobbyScreen의 Popup들]
├── SettingsPopup (Pause 상태)
└── ConfirmPopup (Pause 상태)

        │ Pop() - LobbyScreen으로 복귀
        ▼

[LobbyScreen 활성]
├── SettingsPopup (Resume → 다시 활성)
└── ConfirmPopup (Resume → 다시 활성)
```

---

## 뒤로가기 처리

### Back() 로직

```csharp
public bool Back()
{
    // 1. 현재 Screen에 Popup이 있으면 최상위 Popup 닫기
    if (HasPopupInCurrentScreen())
    {
        var topPopup = GetTopPopup();
        if (topPopup.OnEscape())
        {
            ClosePopup(topPopup);
            return true;
        }
    }

    // 2. Popup이 없으면 Screen Pop
    if (ScreenStackCount > 1)
    {
        Pop();
        return true;
    }

    // 3. 더 이상 뒤로갈 곳이 없음
    return false;
}
```

### ESC 키 처리

```csharp
void Update()
{
    if (Input.GetKeyDown(KeyCode.Escape))
    {
        if (!Back())
        {
            // 앱 종료 확인 팝업 등
            ShowExitConfirmPopup();
        }
    }
}
```

---

## State 기반 복구

### Panel 상태 복구

Panel은 Navigation 대상이 아님. Screen의 State에 포함되어 복구.

```csharp
public class LobbyState : IScreenState
{
    public int ActiveTabIndex;      // 어떤 Panel이 활성화되어 있었는지
    public float ScrollPosition;    // 스크롤 위치
    public string SelectedItemId;   // 선택된 아이템
}

public class LobbyScreen : Screen<LobbyScreen, LobbyState>
{
    protected override void OnBind(LobbyState state)
    {
        if (state == null) return;

        // Panel 상태 복구
        _tabWidget.SetActiveIndex(state.ActiveTabIndex);
        SetActivePanel(state.ActiveTabIndex);

        // 스크롤 위치 복구
        _scrollRect.verticalNormalizedPosition = state.ScrollPosition;
    }

    public override LobbyState GetState()
    {
        return new LobbyState
        {
            ActiveTabIndex = _tabWidget.ActiveIndex,
            ScrollPosition = _scrollRect.verticalNormalizedPosition,
            SelectedItemId = _selectedItemId
        };
    }
}
```

### 상태 복구 흐름

```
[LobbyScreen]
├── ActiveTabIndex: 2 (CharacterPanel)
├── ScrollPosition: 0.7
│
│   Push(ShopScreen)
│         │
│         ▼
│   LobbyScreen.Exit()
│   └── GetState() 호출
│   └── Context에 State 저장
│         │
│         ▼
│   ... ShopScreen 사용 ...
│         │
│   Pop()
│         │
│         ▼
│   LobbyScreen.Resume() 또는 Enter()
│   └── OnBind(savedState) 호출
│         │
│         ▼
[LobbyScreen 복구됨]
├── ActiveTabIndex: 2 (CharacterPanel 활성)
├── ScrollPosition: 0.7 (스크롤 위치 복구)
```

---

## Transition

### 기본 구조

```csharp
public abstract class Transition
{
    public Screen OutScreen { get; set; }
    public Screen InScreen { get; set; }

    public abstract UniTask Out();
    public abstract UniTask In();

    public static Transition Default => new FadeTransition();
}

public class FadeTransition : Transition
{
    public override async UniTask Out()
    {
        // 페이드 아웃 애니메이션
    }

    public override async UniTask In()
    {
        // 페이드 인 애니메이션
    }
}

public class EmptyTransition : Transition
{
    public override UniTask Out() => UniTask.CompletedTask;
    public override UniTask In() => UniTask.CompletedTask;
}
```

### 사용

```csharp
// 기본 Transition
LobbyScreen.Open(state);

// 커스텀 Transition
LobbyScreen.Open(state, new SlideTransition());

// Builder 방식
NavigationManager.Instance.Push(
    LobbyScreen.CreateContext(state)
        .SetTransition(new FadeTransition())
);
```

---

## 전체 흐름 예시

```
[앱 시작]
     │
     ▼
NavigationManager.Push(TitleScreen.Context)
     │
     ▼
[TitleScreen 활성]
     │ 터치
     ▼
NavigationManager.Push(LobbyScreen.Context)
     │
     ▼
[LobbyScreen 활성]
     │ 캐릭터 탭 선택 (Tab 2)
     │ 설정 버튼 클릭
     ▼
SettingsPopup.Open()
     │
     ▼
[LobbyScreen + SettingsPopup 활성]
     │ 전투 시작 버튼
     ▼
NavigationManager.Push(BattleScreen.Context)
     │
     ▼
[LobbyScreen Pause + Popup들 Pause]
[BattleScreen 활성]
     │ 전투 종료
     ▼
NavigationManager.Pop()
     │
     ▼
[LobbyScreen Resume + SettingsPopup Resume]
[Tab 2 (캐릭터) 상태 복구됨]
```

---

## 관련

- [UISystem.md](Common/UISystem.md)
- [Common.md](Common.md)
