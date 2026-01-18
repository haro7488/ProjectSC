---
type: architecture
category: Testing
status: approved
version: "1.0"
created: 2026-01-18
updated: 2026-01-18
---

# 테스트 아키텍처

## 개요

시스템 단위 테스트 가능한 구조 설계. 마일스톤/Phase와 독립적으로 각 시스템별 테스트 환경 제공.

---

## 핵심 원칙

| 원칙 | 설명 |
|------|------|
| **시스템 단위** | Phase가 아닌 시스템 기준 테스트 |
| **독립 테스트** | 각 기능을 격리하여 단독 테스트 가능 |
| **통합 테스트** | 전체 시스템 통합 후 흐름 테스트 |
| **에디터 기반** | Unity 에디터에서 직접 확인 가능 |
| **자동화 연동** | Unity Test Framework와 시나리오 공유 |

---

## 테스트 계층

```
┌─────────────────────────────────────────────────────────────┐
│                       테스트 계층                            │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  Level 4: E2E Test                                          │
│  └── 전체 시나리오 (가챠 → 결과 → 보상 수령)                   │
│                                                             │
│  Level 3: Integration Test                                  │
│  └── 시스템 간 연동 (Navigation + Popup + Data)              │
│                                                             │
│  Level 2: System Test (핵심)                                │
│  └── 시스템 단위 (Navigation만, RewardPopup만)               │
│                                                             │
│  Level 1: Unit Test                                         │
│  └── 클래스/함수 단위 (Result<T>, TimeHelper)                │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

---

## 시스템 분류

### 계층 구조

```
Foundation Systems (의존성 없음)
├── Log System
├── Result<T> System
└── EventManager System

Infrastructure Systems (Foundation 의존)
├── Save System (ISaveStorage)
├── Time System (ITimeService)
└── Loading System (LoadingIndicator)

Data Systems (Infrastructure 의존)
├── MasterData System
├── UserData System
└── Network System (IApiClient)

UI Systems (Data 의존)
├── Navigation System
├── Popup System (SystemPopup, RewardPopup)
└── Widget System

Content Systems (UI + Data 의존)
├── Gacha System
├── Shop System
├── Character System
├── Stage System
└── LiveEvent System
```

### 테스트 매트릭스

| 시스템 | 독립 테스트 | 필요 Mock | 우선순위 |
|--------|------------|-----------|---------|
| Log | ✅ | 없음 | 중간 |
| Result<T> | ✅ | 없음 | 높음 |
| EventManager | ✅ | 없음 | 중간 |
| Save | ✅ | ISaveStorage | 높음 |
| Time | ✅ | 없음 | 높음 |
| Loading | ✅ | 없음 | 높음 |
| Navigation | ✅ | 없음 | **최우선** |
| Popup | ⚠️ | IRewardHelper | 높음 |
| Gacha | ⚠️ | IApiClient | 중간 |
| Shop | ⚠️ | IApiClient, ITimeService | 중간 |

---

## 의존성 관리

### 패턴: SO + ServiceLocator 혼합

```csharp
// 1. 인터페이스 정의
public interface ITimeService
{
    long ServerTimeUtc { get; }
    DateTime ServerDateTime { get; }
}

// 2. 실제 구현 (ScriptableObject)
[CreateAssetMenu(menuName = "SC/Services/TimeService")]
public class TimeServiceAsset : ScriptableObject, ITimeService
{
    public long ServerTimeUtc => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    public DateTime ServerDateTime => DateTime.UtcNow;
}

// 3. Mock 구현 (테스트용)
public class MockTimeService : ITimeService
{
    public long FixedTime { get; set; } = 1700000000;
    public long ServerTimeUtc => FixedTime;
    public DateTime ServerDateTime => DateTimeOffset.FromUnixTimeSeconds(FixedTime).DateTime;
}

// 4. ServiceLocator (런타임 폴백)
public static class Services
{
    private static readonly Dictionary<Type, object> _services = new();

    public static void Register<T>(T service) where T : class
        => _services[typeof(T)] = service;

    public static T Get<T>() where T : class
        => _services.TryGetValue(typeof(T), out var s) ? (T)s : null;

    public static void Clear() => _services.Clear();
}

// 5. 사용처 (Inspector 우선, 없으면 ServiceLocator)
public class ShopScreen : ScreenWidget
{
    [SerializeField] private ScriptableObject _timeServiceAsset;
    private ITimeService _timeService;

    protected override void OnInitialize()
    {
        _timeService = (_timeServiceAsset as ITimeService)
                       ?? Services.Get<ITimeService>();
    }
}
```

### 장점

- Inspector에서 Mock 교체 가능
- 테스트 시 Services.Register로 Mock 주입
- 기존 Singleton 코드와 공존 가능
- 별도 DI 프레임워크 불필요

---

## 폴더 구조

```
Assets/
├── Scripts/
│   ├── Tests/                          # 런타임 (빌드 포함)
│   │   ├── Mocks/
│   │   │   ├── MockTimeService.cs
│   │   │   ├── MockSaveStorage.cs
│   │   │   └── MockApiClient.cs
│   │   ├── Scenarios/                  # 테스트 시나리오 (공유)
│   │   │   ├── NavigationTestScenarios.cs
│   │   │   ├── PopupTestScenarios.cs
│   │   │   └── GachaTestScenarios.cs
│   │   ├── Runners/                    # 수동 테스트 러너
│   │   │   ├── SystemTestRunner.cs     # 베이스
│   │   │   ├── NavigationTestRunner.cs
│   │   │   └── PopupTestRunner.cs
│   │   └── Helpers/
│   │       ├── TestEnvironment.cs
│   │       ├── TestCanvasFactory.cs
│   │       └── MockDataFactory.cs
│   │
│   └── Editor/
│       └── Tests/
│           ├── SystemTestMenu.cs       # 메뉴 등록
│           └── SystemTestWindow.cs     # 통합 윈도우 (선택)
│
├── Data/
│   └── TestData/                       # 테스트용 SO, JSON
│       ├── MockCharacterDatabase.asset
│       └── MockRewards.json
│
└── Tests/                              # Unity Test Framework (빌드 제외)
    ├── EditMode/
    │   ├── EditMode.asmdef
    │   └── ResultTests.cs
    └── PlayMode/
        ├── PlayMode.asmdef
        └── NavigationTests.cs
```

---

## 에디터 도구 구조

### 기본 개념

```
┌────────────────────────────────────────────────────────────┐
│                        씬 구조                              │
├────────────────────────────────────────────────────────────┤
│                                                            │
│   [기본 게임 씬] Game.unity                                 │
│   ├── 전체 시스템 통합                                      │
│   ├── 실제 게임과 동일하게 동작                              │
│   └── MVPSceneSetup으로 구성                               │
│                                                            │
│   [시스템 테스트]                                           │
│   ├── 현재 씬 유지 (Game.unity에서도 실행 가능)              │
│   ├── 테스트 오브젝트 동적 생성                              │
│   ├── 기존 씬 오브젝트 비활성화 (선택적 격리)                 │
│   └── 테스트 완료 후 정리                                   │
│                                                            │
└────────────────────────────────────────────────────────────┘
```

### 메뉴 구조

```
SC Tools/
├── Game Scene/
│   ├── Setup Full Game Scene       # 전체 게임 씬 구성
│   └── Clear Scene                 # 정리
│
└── System Tests/
    ├── Foundation/
    │   ├── Test Log System
    │   ├── Test Result Pattern
    │   └── Test EventManager
    │
    ├── Infrastructure/
    │   ├── Test Save System
    │   ├── Test Time System
    │   └── Test Loading System
    │
    ├── UI/
    │   ├── Test Navigation         # 첫 번째
    │   ├── Test SystemPopups
    │   ├── Test RewardPopup
    │   └── Test Widgets
    │
    └── Contents/
        ├── Test Gacha System
        ├── Test Shop System
        ├── Test Character System
        ├── Test Stage System
        └── Test Event System
```

---

## 핵심 클래스 설계

### Services (ServiceLocator)

```csharp
// Scripts/Foundation/Services.cs
public static class Services
{
    private static readonly Dictionary<Type, object> _services = new();

    public static void Register<T>(T service) where T : class
    {
        _services[typeof(T)] = service;
    }

    public static T Get<T>() where T : class
    {
        return _services.TryGetValue(typeof(T), out var service)
            ? (T)service
            : null;
    }

    public static bool TryGet<T>(out T service) where T : class
    {
        service = Get<T>();
        return service != null;
    }

    public static void Unregister<T>() where T : class
    {
        _services.Remove(typeof(T));
    }

    public static void Clear()
    {
        _services.Clear();
    }
}
```

### SystemTestRunner (베이스)

```csharp
// Scripts/Tests/Runners/SystemTestRunner.cs
public abstract class SystemTestRunner : MonoBehaviour
{
    [Header("Test Configuration")]
    [SerializeField] protected bool _useMockServices = true;
    [SerializeField] protected bool _isolateFromScene = false;

    protected GameObject _testRoot;
    protected Canvas _testCanvas;
    protected GameObject _controlPanel;

    private List<GameObject> _disabledObjects = new();

    public virtual void SetupTest()
    {
        // 1. 테스트 루트 생성
        _testRoot = new GameObject($"[TEST] {GetSystemName()}");

        // 2. 기존 씬 격리 (선택적)
        if (_isolateFromScene)
            DisableSceneObjects();

        // 3. Mock 서비스 등록
        if (_useMockServices)
            RegisterMockServices();

        // 4. 테스트 Canvas 생성
        _testCanvas = TestCanvasFactory.Create(_testRoot.transform);

        // 5. 컨트롤 패널 생성
        CreateControlPanel();

        // 6. 시스템별 셋업
        OnSetup();

        Debug.Log($"[SystemTest] {GetSystemName()} 테스트 시작");
    }

    public virtual void TeardownTest()
    {
        OnTeardown();
        Services.Clear();
        RestoreSceneObjects();

        if (_testRoot != null)
            DestroyImmediate(_testRoot);

        Debug.Log($"[SystemTest] {GetSystemName()} 테스트 종료");
    }

    protected abstract string GetSystemName();
    protected abstract void OnSetup();
    protected abstract void OnTeardown();
    protected virtual void RegisterMockServices() { }
    protected abstract void CreateControlPanel();

    private void DisableSceneObjects()
    {
        foreach (var go in FindObjectsOfType<GameObject>())
        {
            if (go.activeInHierarchy && go != gameObject && go.transform.parent == null)
            {
                go.SetActive(false);
                _disabledObjects.Add(go);
            }
        }
    }

    private void RestoreSceneObjects()
    {
        foreach (var go in _disabledObjects)
        {
            if (go != null)
                go.SetActive(true);
        }
        _disabledObjects.Clear();
    }
}
```

### TestCanvasFactory

```csharp
// Scripts/Tests/Helpers/TestCanvasFactory.cs
public static class TestCanvasFactory
{
    public static TestCanvas Create(Transform parent)
    {
        // Canvas
        var canvasGO = new GameObject("TestCanvas");
        canvasGO.transform.SetParent(parent);

        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;  // 최상위

        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        // Containers
        var screenContainer = CreateContainer(canvasGO.transform, "ScreenContainer", 0);
        var popupContainer = CreateContainer(canvasGO.transform, "PopupContainer", 100);
        var controlContainer = CreateContainer(canvasGO.transform, "ControlContainer", 200);

        return new TestCanvas
        {
            Canvas = canvas,
            ScreenContainer = screenContainer,
            PopupContainer = popupContainer,
            ControlContainer = controlContainer
        };
    }

    private static RectTransform CreateContainer(Transform parent, string name, int sortingOrder)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent);

        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;

        var canvas = go.AddComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = sortingOrder;

        go.AddComponent<GraphicRaycaster>();

        return rect;
    }
}

public struct TestCanvas
{
    public Canvas Canvas;
    public RectTransform ScreenContainer;
    public RectTransform PopupContainer;
    public RectTransform ControlContainer;
}
```

### TestEnvironment (자동화 테스트용)

```csharp
// Scripts/Tests/Helpers/TestEnvironment.cs
public static class TestEnvironment
{
    private static GameObject _testRoot;

    public static IEnumerator SetupMinimal<T>() where T : SystemTestRunner
    {
        _testRoot = new GameObject("[TestEnvironment]");
        Object.DontDestroyOnLoad(_testRoot);

        var runner = _testRoot.AddComponent<T>();
        runner.SetupTest();

        yield return null;
    }

    public static IEnumerator SetupWithMockServices()
    {
        Services.Clear();
        Services.Register<ITimeService>(new MockTimeService());
        Services.Register<ISaveStorage>(new MockSaveStorage());
        Services.Register<IApiClient>(new MockApiClient());

        yield return null;
    }

    public static IEnumerator Cleanup()
    {
        Services.Clear();

        if (_testRoot != null)
        {
            var runner = _testRoot.GetComponent<SystemTestRunner>();
            runner?.TeardownTest();
            Object.DestroyImmediate(_testRoot);
            _testRoot = null;
        }

        yield return null;
    }
}
```

---

## 예시: NavigationTestRunner

```csharp
// Scripts/Tests/Runners/NavigationTestRunner.cs
public class NavigationTestRunner : SystemTestRunner
{
    private NavigationManager _navigation;
    private NavigationTestScenarios _scenarios;

    protected override string GetSystemName() => "Navigation";

    protected override void OnSetup()
    {
        // NavigationManager 생성
        var navGO = new GameObject("NavigationManager");
        navGO.transform.SetParent(_testRoot.transform);
        _navigation = navGO.AddComponent<NavigationManager>();

        // 컨테이너 연결
        // _navigation.Initialize(_testCanvas.ScreenContainer, _testCanvas.PopupContainer);

        // 시나리오 생성
        _scenarios = new NavigationTestScenarios(_navigation);
    }

    protected override void OnTeardown()
    {
        _navigation = null;
        _scenarios = null;
    }

    protected override void CreateControlPanel()
    {
        var panel = TestUIBuilder.CreatePanel(_testCanvas.ControlContainer, "NavigationTestPanel");

        TestUIBuilder.AddButton(panel, "Push Screen", OnPushScreenClicked);
        TestUIBuilder.AddButton(panel, "Push Popup", OnPushPopupClicked);
        TestUIBuilder.AddButton(panel, "Pop", OnPopClicked);
        TestUIBuilder.AddButton(panel, "Pop All", OnPopAllClicked);

        TestUIBuilder.AddSeparator(panel);
        TestUIBuilder.AddLabel(panel, "Scenarios:");
        TestUIBuilder.AddButton(panel, "Run: Push 3 → Pop All", OnScenarioPushPopAll);
        TestUIBuilder.AddButton(panel, "Run: Visibility Check", OnScenarioVisibility);

        TestUIBuilder.AddSeparator(panel);
        TestUIBuilder.AddButton(panel, "Exit Test", () => TeardownTest());

        _controlPanel = panel;
    }

    // 액션들
    private async void OnPushScreenClicked()
    {
        await _navigation.PushAsync<TestScreen>();
        LogState();
    }

    private async void OnPopClicked()
    {
        await _navigation.PopAsync();
        LogState();
    }

    private async void OnScenarioPushPopAll()
    {
        var result = await _scenarios.RunPushPopAllScenario();
        Debug.Log($"[Scenario] PushPopAll: {(result.Success ? "PASS" : "FAIL")} - {result.Message}");
    }

    private void LogState()
    {
        Debug.Log($"[Navigation] Screens: {_navigation.ScreenCount}, Popups: {_navigation.PopupCount}");
    }
}
```

### NavigationTestScenarios (공유)

```csharp
// Scripts/Tests/Scenarios/NavigationTestScenarios.cs
public class NavigationTestScenarios
{
    private readonly NavigationManager _navigation;

    public NavigationTestScenarios(NavigationManager navigation)
    {
        _navigation = navigation;
    }

    public async UniTask<TestResult> RunPushPopAllScenario()
    {
        // Push 3개
        await _navigation.PushAsync<TestScreenA>();
        await _navigation.PushAsync<TestScreenB>();
        await _navigation.PushAsync<TestScreenC>();

        var countAfterPush = _navigation.ScreenCount;

        // Pop All
        await _navigation.PopAllAsync();

        var countAfterPop = _navigation.ScreenCount;

        return new TestResult
        {
            Success = countAfterPush == 3 && countAfterPop == 0,
            Message = $"PushCount={countAfterPush}, AfterPopAll={countAfterPop}"
        };
    }

    public async UniTask<TestResult> RunVisibilityScenario()
    {
        await _navigation.PushAsync<TestScreenA>();
        await _navigation.PushAsync<TestScreenB>();

        var screenA = _navigation.GetScreen<TestScreenA>();
        var screenB = _navigation.GetScreen<TestScreenB>();

        var aHidden = !screenA.IsVisible;
        var bVisible = screenB.IsVisible;

        return new TestResult
        {
            Success = aHidden && bVisible,
            Message = $"ScreenA.Visible={screenA.IsVisible}, ScreenB.Visible={screenB.IsVisible}"
        };
    }
}

public struct TestResult
{
    public bool Success;
    public string Message;
}
```

---

## 구축 순서

### 1차: 베이스 + 수동 테스트

```
필수 (베이스):
├── Services.cs
├── SystemTestRunner.cs
├── TestCanvasFactory.cs
├── TestUIBuilder.cs
└── TestResult.cs

Mock:
├── MockTimeService.cs
├── MockSaveStorage.cs
└── MockApiClient.cs

첫 번째 시스템:
├── NavigationTestScenarios.cs
├── NavigationTestRunner.cs
└── SystemTestMenu.cs (메뉴 등록)
```

### 2차: 자동화 테스트 연동

```
추가:
├── TestEnvironment.cs
├── Tests/EditMode/
│   ├── EditMode.asmdef
│   └── ResultTests.cs
└── Tests/PlayMode/
    ├── PlayMode.asmdef
    └── NavigationTests.cs
```

### 3차: 시스템 확장

```
Infrastructure:
├── LoadingTestRunner.cs
├── SaveTestRunner.cs
└── TimeTestRunner.cs

UI:
├── PopupTestRunner.cs
├── PopupTestScenarios.cs
└── ...

Contents:
├── GachaTestRunner.cs
└── ...
```

---

## 자동화 테스트 연동 (2차)

### Edit Mode Test 예시

```csharp
// Tests/EditMode/ResultTests.cs
using NUnit.Framework;

[TestFixture]
public class ResultTests
{
    [Test]
    public void Success_ShouldContainValue()
    {
        var result = Result<int>.Success(42);

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(42, result.Value);
    }

    [Test]
    public void Failure_ShouldContainErrorCode()
    {
        var result = Result<int>.Failure("ERR_001");

        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("ERR_001", result.ErrorCode);
    }
}
```

### Play Mode Test 예시

```csharp
// Tests/PlayMode/NavigationTests.cs
using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

[TestFixture]
public class NavigationPlayModeTests
{
    private NavigationTestScenarios _scenarios;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        yield return TestEnvironment.SetupMinimal<NavigationTestRunner>();
        _scenarios = new NavigationTestScenarios(NavigationManager.Instance);
    }

    [UnityTearDown]
    public IEnumerator Teardown()
    {
        yield return TestEnvironment.Cleanup();
    }

    [UnityTest]
    public IEnumerator Scenario_PushPopAll_ShouldPass()
    {
        yield return _scenarios.RunPushPopAllScenario().ToCoroutine(out var result);
        Assert.IsTrue(result.Success, result.Message);
    }
}
```

---

## 관련 문서

- [AITools.md](../Editor/AITools.md) - 기존 에디터 도구
- [UISystem.md](../Common/UISystem.md) - UI 시스템
- [Navigation.md](../Navigation.md) - Navigation 시스템
