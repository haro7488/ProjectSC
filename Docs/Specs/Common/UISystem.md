---
type: spec
assembly: Sc.Common
class: Widget, Screen, Popup, Panel, IScreenState, IPopupState
category: Pattern
status: draft
version: "3.0"
dependencies: [NavigationManager, EventManager]
created: 2025-01-14
updated: 2025-01-15
---

# UI 시스템

## 개요

Widget 기반 Composition 패턴과 State/Context 기반 화면 관리 시스템.

- **Widget**: 모든 UI의 기본 단위 (Composition)
- **Screen**: 전체 화면 (State 기반 상태 관리)
- **Popup**: 모달 팝업 (State 기반 상태 관리)
- **Panel**: Screen/Popup 내부 컨테이너

---

## 계층 구조

```
Widget (Base)
│
├── 일반 Widget (접미사 Widget)
│   ├── ButtonWidget, TextWidget, ImageWidget
│   ├── CurrencyWidget, ProfileWidget
│   └── ListWidget, TabWidget, ScrollWidget
│
├── Panel (컨테이너 역할)
│   └── InventoryPanel, SettingsPanel
│
├── Screen<TScreen, TState> (전체 화면)
│   └── LobbyScreen, BattleScreen, ShopScreen
│
└── Popup<TPopup, TState> (모달 팝업)
    └── ConfirmPopup, RewardPopup, SettingsPopup
```

### 역할 구분

| 타입 | 역할 | Navigation 대상 | 히스토리 |
|------|------|----------------|----------|
| Widget | UI 기본 단위 | X | X |
| Panel | Screen/Popup 내부 컨테이너 | X | X (State로 복구) |
| Screen | 전체 화면 | O | O |
| Popup | 모달 팝업 | O | O |

> **Note**: Overlay 개념 없음. 전면을 덮는 상황은 새로운 Screen으로 처리.

---

## Widget 기본 구조

### Widget 클래스

```csharp
public abstract class Widget : MonoBehaviour
{
    // 계층 관리
    public Widget Parent { get; private set; }
    public IReadOnlyList<Widget> Children => _children;

    // 상태
    public bool IsVisible { get; private set; }
    public bool IsInitialized { get; private set; }

    // 생명주기
    public virtual void OnInitialize() { }
    public virtual void OnBind(object data) { }
    public virtual void OnShow() { IsVisible = true; }
    public virtual void OnHide() { IsVisible = false; }
    public virtual void OnRefresh() { }
    public virtual void OnRelease() { }

    // 계층 관리
    public void AddChild(Widget child);
    public void RemoveChild(Widget child);
    protected T FindChild<T>() where T : Widget;
}
```

### 네이밍 규칙

| 분류 | 접미사 | 예시 |
|------|--------|------|
| 일반 Widget | `~Widget` | CurrencyWidget, ButtonWidget |
| 컨테이너 | `~Panel` | InventoryPanel, SettingsPanel |
| 전체 화면 | `~Screen` | LobbyScreen, BattleScreen |
| 팝업 | `~Popup` | ConfirmPopup, RewardPopup |

---

## Screen 시스템

### 구조

```csharp
// State 인터페이스
public interface IScreenState { }

// Screen 베이스 (비제네릭)
public abstract class Screen : Widget
{
    public abstract class Context
    {
        public abstract Screen View { get; }
        public abstract Type ScreenType { get; }

        public abstract UniTask Load();
        public abstract UniTask Enter();
        public abstract void Resume();
        public abstract void Pause();
        public abstract UniTask Exit();
    }
}

// Screen 제네릭 베이스
public abstract class Screen<TScreen, TState> : Screen
    where TScreen : Screen<TScreen, TState>
    where TState : class, IScreenState
{
    // 생명주기 - 개별 Screen에서 오버라이드
    protected virtual void OnBind(TState state) { }
    public virtual TState GetState() => default;

    // Context - 베이스에서 완전히 정의
    public new class Context : Screen.Context
    {
        private TScreen _screen;
        private TState _state;
        private Transition _transition;

        public override Screen View => _screen;
        public override Type ScreenType => typeof(TScreen);
        public TState State => _state;

        internal Context(TState state, Transition transition) { ... }

        public override async UniTask Enter()
        {
            _screen.OnBind(_state);
        }

        public override async UniTask Exit()
        {
            var currentState = _screen.GetState();
            if (currentState != null)
                _state = currentState;
            // ...
        }

        // Builder (struct) - 베이스에서 완전히 정의
        public struct Builder
        {
            private TState _state;
            private Transition _transition;

            public Builder(TState state) { _state = state; _transition = null; }

            public Builder SetTransition(Transition transition)
            {
                _transition = transition;
                return this;
            }

            public Context Build() => new(_state, _transition ?? Transition.Default);

            public static implicit operator Context(Builder b) => b.Build();
        }
    }

    // 정적 진입점 - 베이스에서 정의
    public static Context.Builder CreateContext(TState state) => new(state);

    public static void Open(TState state = default, Transition transition = null)
    {
        var builder = CreateContext(state);
        if (transition != null)
            builder.SetTransition(transition);
        NavigationManager.Instance.Push(builder);
    }
}
```

### State 정의

State는 **class**로 정의 (단순 데이터, GC 영향 미미).

```csharp
public class LobbyState : IScreenState
{
    public int ActiveTabIndex;
    public float ScrollPosition;
    public string SelectedItemId;
}
```

### Screen 구현 예시

```csharp
public class LobbyScreen : Screen<LobbyScreen, LobbyState>
{
    [SerializeField] private TabWidget _tabWidget;
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private HeaderWidget _header;

    protected override void OnBind(LobbyState state)
    {
        if (state == null) return;

        _tabWidget.SetActiveIndex(state.ActiveTabIndex);
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

### 사용 패턴

```csharp
// 간단한 열기
LobbyScreen.Open(new LobbyState { ActiveTabIndex = 2 });

// 기본 State로 열기
LobbyScreen.Open();

// Transition 지정
LobbyScreen.Open(
    new LobbyState { ActiveTabIndex = 2 },
    new FadeTransition()
);

// Builder 방식 (세밀한 제어)
NavigationManager.Instance.Push(
    LobbyScreen.CreateContext(new LobbyState { ActiveTabIndex = 2 })
        .SetTransition(new FadeTransition())
);
```

---

## Popup 시스템

Screen과 동일한 패턴. Screen별로 Popup 스택 그룹화.

### 구조

```csharp
public interface IPopupState { }

public abstract class Popup<TPopup, TState> : Widget
    where TPopup : Popup<TPopup, TState>
    where TState : class, IPopupState
{
    protected virtual void OnBind(TState state) { }
    public virtual TState GetState() => default;

    public new class Context : Popup.Context
    {
        // Screen.Context와 동일한 구조

        public struct Builder
        {
            private TState _state;
            private int _sortingOrder;

            public Builder(TState state) { ... }

            public Builder SetOrder(int order)
            {
                _sortingOrder = order;
                return this;
            }

            public Context Build() => new(_state, _sortingOrder);
            public static implicit operator Context(Builder b) => b.Build();
        }
    }

    public static Context.Builder CreateContext(TState state) => new(state);

    public static void Open(TState state = default)
    {
        NavigationManager.Instance.PushPopup(CreateContext(state));
    }
}
```

### Popup과 Screen 관계

```
LobbyScreen (활성)
├── [Popup] SettingsPopup     ← LobbyScreen 소속
└── [Popup] ConfirmPopup      ← LobbyScreen 소속

       │ Screen 전환
       ▼

BattleScreen (활성)
└── [Popup] PausePopup        ← BattleScreen 소속

※ LobbyScreen의 Popup들은 자동 Pause
※ LobbyScreen으로 돌아오면 자동 Resume
```

### Popup 구현 예시

```csharp
public class ConfirmState : IPopupState
{
    public string Title;
    public string Message;
    public Action OnConfirm;
    public Action OnCancel;
}

public class ConfirmPopup : Popup<ConfirmPopup, ConfirmState>
{
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _messageText;
    [SerializeField] private Button _confirmButton;
    [SerializeField] private Button _cancelButton;

    private ConfirmState _state;

    protected override void OnBind(ConfirmState state)
    {
        _state = state;
        _titleText.text = state.Title;
        _messageText.text = state.Message;

        _confirmButton.onClick.AddListener(() =>
        {
            _state.OnConfirm?.Invoke();
            Close();
        });

        _cancelButton.onClick.AddListener(() =>
        {
            _state.OnCancel?.Invoke();
            Close();
        });
    }

    private void Close()
    {
        NavigationManager.Instance.ClosePopup(this);
    }
}

// 사용
ConfirmPopup.Open(new ConfirmState
{
    Title = "삭제",
    Message = "정말 삭제하시겠습니까?",
    OnConfirm = () => DeleteItem(itemId),
    OnCancel = () => { }
});
```

---

## Panel (컨테이너)

Panel은 Screen/Popup 내부의 컨테이너 역할. Navigation 대상이 아님.

### 역할

- Screen/Popup 내부에서 섹션 구분
- 탭 전환 등 내부 상태 관리
- State의 일부로 상태 복구됨

### 예시

```csharp
public class LobbyScreen : Screen<LobbyScreen, LobbyState>
{
    [SerializeField] private InventoryPanel _inventoryPanel;
    [SerializeField] private CharacterPanel _characterPanel;
    [SerializeField] private TabWidget _tabWidget;

    private Panel _activePanel;

    protected override void OnBind(LobbyState state)
    {
        // Tab 상태 복구
        _tabWidget.SetActiveIndex(state.ActiveTabIndex);

        // 활성 Panel 설정
        SetActivePanel(state.ActiveTabIndex);
    }

    private void SetActivePanel(int index)
    {
        _inventoryPanel.gameObject.SetActive(index == 0);
        _characterPanel.gameObject.SetActive(index == 1);
    }

    public override LobbyState GetState()
    {
        return new LobbyState
        {
            ActiveTabIndex = _tabWidget.ActiveIndex,
            // Panel의 내부 상태도 포함 가능
        };
    }
}
```

---

## 생명주기

### Screen 생명주기

```
[Push]
   │
   ▼
Load ──▶ Enter(OnBind) ──▶ Resume
                              │
                    ┌─────────┴─────────┐
                    │                   │
                 [활성]              Pause (다른 Screen으로 전환)
                    │                   │
                    │                   ▼
                    │              [비활성 대기]
                    │                   │
                    │               Resume (돌아옴)
                    │                   │
                    └─────────┬─────────┘
                              │
                           [Pop]
                              │
                              ▼
                     Exit (GetState 호출)
                              │
                              ▼
                           Unload
```

### 생명주기 메서드

| 메서드 | 시점 | 용도 |
|--------|------|------|
| Load | 인스턴스 로드 | 리소스 로딩, Provider에서 가져오기 |
| Enter (OnBind) | 화면 진입 | State로 UI 초기화 |
| Resume | 활성화 | 이벤트 구독, 갱신 시작 |
| Pause | 비활성화 | 이벤트 해제, 갱신 중지 |
| Exit | 화면 퇴장 | GetState로 상태 저장, Provider로 반환 |
| Unload | 리소스 해제 | 메모리 정리 |

---

## Context 흐름

### 화면 전환 시 상태 보존

```
[LobbyScreen 활성]
├── State: { ActiveTabIndex: 2, ScrollPosition: 0.7 }
│
│   NavigateTo(BattleScreen)
│         │
│         ▼
│   LobbyScreen.Exit()
│   └── GetState() → State 저장
│   └── Context에 State 보관
│
│   BattleScreen.Enter()
│
│         │
│   NavigationManager.Back()
│         │
│         ▼
│   BattleScreen.Exit()
│
│   LobbyScreen.Enter()
│   └── OnBind(savedState) → 상태 복구
│
[LobbyScreen 활성]
└── 이전 상태 복구됨 (Tab 2, Scroll 0.7)
```

---

## 새 Screen/Popup 작성 가이드

### 1단계: State 정의

```csharp
public class MyScreenState : IScreenState
{
    public int SomeValue;
    public string SomeText;
}
```

### 2단계: Screen 구현

```csharp
public class MyScreen : Screen<MyScreen, MyScreenState>
{
    [SerializeField] private SomeWidget _widget;

    protected override void OnBind(MyScreenState state)
    {
        if (state == null) return;
        _widget.SetValue(state.SomeValue);
    }

    public override MyScreenState GetState()
    {
        return new MyScreenState
        {
            SomeValue = _widget.Value,
            SomeText = _widget.Text
        };
    }
}
```

### 3단계: 사용

```csharp
MyScreen.Open(new MyScreenState { SomeValue = 10 });
```

> Context, Builder, 정적 메서드는 **베이스 클래스에서 모두 제공**되므로 재정의 불필요.

---

## 관련

- [Common.md](../Common.md)
- [UIComponents.md](UIComponents.md)
- [Navigation.md](../Navigation.md)
