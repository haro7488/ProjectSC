---
type: architecture
category: Testing
status: approved
version: "1.1"
created: 2026-01-18
updated: 2026-01-19
---

# 테스트 아키텍처

## 개요

시스템 단위 테스트 가능한 구조. 마일스톤/Phase와 독립적으로 각 시스템별 테스트 환경 제공.

**현재 상태**: 4차 구축 완료 (NUnit 149개+, PlayMode 인프라)

---

## 구축 현황

| 단계 | 내용 | 상태 |
|------|------|------|
| 1차 | 베이스 인프라 + 수동 테스트 | ✅ 완료 |
| 2차 | NUnit 단위 테스트 (Foundation, Core) | ✅ 완료 |
| 3차 | 시스템 확장 (Common, Reward) | ✅ 완료 |
| 4차 | PlayMode 테스트 인프라 | ✅ 완료 |
| 5차 | 컨텐츠 시스템 테스트 | ⬜ 대기 |

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

## 테스트 매트릭스

### 시스템별 구현 상태

| 시스템 | NUnit | 시나리오 러너 | PlayMode | Mock 필요 |
|--------|-------|--------------|----------|-----------|
| **Foundation** |
| Log | ✅ 11개 | - | - | 없음 |
| Result<T> | ✅ 14개 | - | - | 없음 |
| ErrorMessages | ✅ 11개 | - | - | 없음 |
| **Core** |
| SaveStorage | ✅ 17개 | ✅ | - | ISaveStorage |
| SaveMigrator | ✅ 완료 | ✅ | - | 없음 |
| TimeService | ✅ 25개 | - | - | 없음 |
| TimeHelper | ✅ 20개 | - | - | 없음 |
| AssetManager | ✅ 완료 | ✅ | - | 없음 |
| **Common** |
| LoadingService | ✅ 완료 | - | - | 없음 |
| LoadingConfig | ✅ 완료 | - | - | 없음 |
| RewardInfo | ✅ 16개 | - | - | 없음 |
| RewardProcessor | ✅ 28개 | - | - | 없음 |
| RewardHelper | ✅ 17개 | - | - | 없음 |
| ConfirmState | ✅ 12개 | - | - | 없음 |
| CostConfirmState | ✅ 22개 | - | - | 없음 |
| RewardPopupState | ✅ 13개 | - | - | 없음 |
| SimpleItemSpawner | ✅ 12개 | - | - | 없음 |
| IPopupState | ✅ 8개 | - | - | 없음 |
| **UI** |
| Navigation | - | ✅ | ✅ | 없음 |
| **Contents** |
| Gacha | ⬜ | ⬜ | ⬜ | IApiClient |
| Shop | ⬜ | ⬜ | ⬜ | IApiClient, ITimeService |

**총계**: NUnit 149개+, 시나리오 러너 3개, PlayMode 샘플 2개

---

## 폴더 구조

### 실제 구현된 구조

```
Assets/Scripts/
├── Tests/                              # 런타임 테스트 (Sc.Tests)
│   ├── Sc.Tests.asmdef
│   │
│   ├── Helpers/                        # 테스트 유틸리티
│   │   ├── TestCanvasFactory.cs        # Canvas 동적 생성
│   │   ├── TestResult.cs               # 테스트 결과 구조체
│   │   └── TestUIBuilder.cs            # UI 동적 생성
│   │
│   ├── Mocks/                          # Mock 구현체
│   │   ├── ITestInterfaces.cs          # 테스트용 인터페이스
│   │   ├── MockTimeService.cs
│   │   ├── MockSaveStorage.cs
│   │   └── MockApiClient.cs
│   │
│   ├── Scenarios/                      # 테스트 시나리오 (NUnit과 공유)
│   │   ├── NavigationTestScenarios.cs
│   │   ├── SaveManagerTestScenarios.cs
│   │   └── AssetManagerTestScenarios.cs
│   │
│   ├── Runners/                        # 수동 테스트 러너
│   │   ├── SystemTestRunner.cs         # 베이스 클래스
│   │   ├── NavigationTestRunner.cs
│   │   ├── SaveManagerTestRunner.cs
│   │   └── AssetManagerTestRunner.cs
│   │
│   ├── TestWidgets/                    # 테스트용 위젯
│   │   ├── SimpleTestScreen.cs
│   │   └── SimpleTestPopup.cs
│   │
│   └── PlayMode/                       # PlayMode 테스트 인프라
│       ├── PlayModeTestBase.cs         # 베이스 클래스
│       ├── Helpers/
│       │   ├── PlayModeAssert.cs       # Unity 오브젝트 어서션
│       │   └── PrefabTestHelper.cs     # Addressables 프리팹 로드
│       └── Samples/
│           ├── NavigationPlayModeTests.cs
│           └── PrefabLoadPlayModeTests.cs
│
├── Editor/
│   ├── AI/
│   │   └── PlayModeTestSetup.cs        # SC Tools/PlayMode Tests 메뉴
│   │
│   └── Tests/                          # NUnit 단위 테스트 (Sc.Editor.Tests)
│       ├── Sc.Editor.Tests.asmdef
│       ├── SystemTestMenu.cs           # SC Tools/System Tests 메뉴
│       │
│       ├── Foundation/                 # 36개 테스트
│       │   ├── LogTests.cs
│       │   ├── ResultTests.cs
│       │   └── ErrorMessagesTests.cs
│       │
│       ├── Core/                       # 36개+ 테스트
│       │   ├── SaveStorageTests.cs
│       │   ├── SaveMigratorTests.cs
│       │   ├── MockSaveStorageTests.cs
│       │   ├── TimeServiceTests.cs
│       │   └── TimeHelperTests.cs
│       │
│       └── Common/                     # 77개+ 테스트
│           ├── LoadingServiceTests.cs
│           ├── LoadingConfigTests.cs
│           ├── RewardInfoTests.cs
│           ├── RewardProcessorTests.cs
│           ├── RewardHelperTests.cs
│           ├── ConfirmStateTests.cs
│           ├── CostConfirmStateTests.cs
│           ├── RewardPopupStateTests.cs
│           ├── SimpleItemSpawnerTests.cs
│           └── IPopupStateTests.cs
```

### 에디터 메뉴 구조

```
SC Tools/
├── System Tests/
│   ├── Navigation/
│   │   └── Run Navigation Test
│   ├── SaveManager/
│   │   └── Run SaveManager Test
│   └── AssetManager/
│       └── Run AssetManager Test
│
└── PlayMode Tests/
    ├── Create All Test Prefabs
    ├── Create Simple Screen/Popup Prefabs
    ├── Verify Test Scene
    └── Delete All Test Prefabs
```

---

## NUnit 단위 테스트

### Assembly 구성

**Sc.Editor.Tests.asmdef**
- 위치: `Assets/Scripts/Editor/Tests/`
- 참조: Sc.Foundation, Sc.Core, Sc.Common, Sc.Data, NUnit

### 테스트 파일 목록

#### Foundation (3개 파일, 36개 테스트)

| 파일 | 테스트 수 | 검증 내용 |
|------|----------|----------|
| LogTests.cs | 11개 | Log 레벨, 카테고리, Output 관리 |
| ResultTests.cs | 14개 | Success/Failure, OnSuccess/OnFailure, Map |
| ErrorMessagesTests.cs | 11개 | GetKey, GetMessage, LocalizeFunc |

#### Core (5개 파일, 36개+ 테스트)

| 파일 | 테스트 수 | 검증 내용 |
|------|----------|----------|
| SaveStorageTests.cs | 17개 | FileSaveStorage CRUD, 경로 처리 |
| SaveMigratorTests.cs | - | NeedsMigration, Migrate, Register |
| MockSaveStorageTests.cs | - | Mock 인터페이스 동작 |
| TimeServiceTests.cs | 25개 | ServerTimeUtc, SyncServerTime, GetNextResetTime |
| TimeHelperTests.cs | 20개 | FormatRemainingTime, FormatRelativeTime |

#### Common (10개 파일, 77개+ 테스트)

| 파일 | 테스트 수 | 검증 내용 |
|------|----------|----------|
| LoadingServiceTests.cs | - | 레퍼런스 카운팅, 상태 전환 |
| LoadingConfigTests.cs | - | 기본값 검증 |
| RewardInfoTests.cs | 16개 | 생성자, 팩토리 메서드, ToString |
| RewardProcessorTests.cs | 28개 | CreateDelta, ValidateRewards, CanApplyRewards |
| RewardHelperTests.cs | 17개 | FormatText, GetIconPath, GetRarityColor |
| ConfirmStateTests.cs | 12개 | 기본값, 검증, 콜백 |
| CostConfirmStateTests.cs | 22개 | 재화 검증, IsInsufficient |
| RewardPopupStateTests.cs | 13개 | 기본값, 검증, 콜백 |
| SimpleItemSpawnerTests.cs | 12개 | Spawn, Despawn, DespawnAll |
| IPopupStateTests.cs | 8개 | 기본값, 오버라이드, 호환성 |

---

## PlayMode 테스트 인프라

### 핵심 클래스

#### PlayModeTestBase

Addressables 초기화, TestCanvas 생성, 자동 정리를 담당하는 베이스 클래스.

```csharp
public abstract class PlayModeTestBase
{
    protected Canvas TestCanvas { get; private set; }
    protected PrefabTestHelper PrefabHelper { get; private set; }

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        // Addressables 초기화
        yield return Addressables.InitializeAsync();
        
        // TestCanvas 생성
        TestCanvas = CreateTestCanvas();
        PrefabHelper = new PrefabTestHelper();
        
        yield return OnSetUp();
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        yield return OnTearDown();
        
        // 자동 정리
        PrefabHelper.ReleaseAll();
        if (TestCanvas != null)
            Object.Destroy(TestCanvas.gameObject);
    }

    protected virtual IEnumerator OnSetUp() { yield break; }
    protected virtual IEnumerator OnTearDown() { yield break; }
}
```

#### PrefabTestHelper

Addressables 프리팹 로드 및 인스턴스 관리.

```csharp
public class PrefabTestHelper
{
    private List<AsyncOperationHandle> _handles = new();
    private List<GameObject> _instances = new();

    public async UniTask<T> LoadPrefabAsync<T>(string address) where T : Object
    {
        var handle = Addressables.LoadAssetAsync<T>(address);
        _handles.Add(handle);
        return await handle;
    }

    public async UniTask<T> InstantiateAsync<T>(string address, Transform parent = null) 
        where T : Component
    {
        var handle = Addressables.InstantiateAsync(address, parent);
        _handles.Add(handle);
        var go = await handle;
        _instances.Add(go);
        return go.GetComponent<T>();
    }

    public void ReleaseAll()
    {
        foreach (var instance in _instances)
            if (instance != null) Addressables.ReleaseInstance(instance);
        foreach (var handle in _handles)
            if (handle.IsValid()) Addressables.Release(handle);
        _instances.Clear();
        _handles.Clear();
    }
}
```

#### PlayModeAssert

Unity 오브젝트 전용 어서션 헬퍼.

```csharp
public static class PlayModeAssert
{
    public static void IsActive(GameObject go, string message = null)
        => Assert.IsTrue(go.activeInHierarchy, message ?? $"{go.name} should be active");

    public static void IsInactive(GameObject go, string message = null)
        => Assert.IsFalse(go.activeInHierarchy, message ?? $"{go.name} should be inactive");

    public static void HasComponent<T>(GameObject go) where T : Component
        => Assert.IsNotNull(go.GetComponent<T>(), $"{go.name} should have {typeof(T).Name}");

    public static void ChildCount(Transform t, int expected)
        => Assert.AreEqual(expected, t.childCount, $"Expected {expected} children");
}
```

### 샘플 테스트

#### NavigationPlayModeTests

기존 NavigationTestScenarios를 NUnit으로 래핑.

```csharp
[TestFixture]
public class NavigationPlayModeTests : PlayModeTestBase
{
    private NavigationTestScenarios _scenarios;

    protected override IEnumerator OnSetUp()
    {
        // NavigationManager 생성 및 시나리오 초기화
        _scenarios = new NavigationTestScenarios(NavigationManager.Instance);
        yield break;
    }

    [UnityTest]
    public IEnumerator PushPopAll_ShouldWork()
    {
        var result = default(TestResult);
        yield return _scenarios.RunPushPopAllScenario().ToCoroutine(r => result = r);
        Assert.IsTrue(result.Success, result.Message);
    }
}
```

#### PrefabLoadPlayModeTests

Addressables 프리팹 로드 검증.

```csharp
[TestFixture]
public class PrefabLoadPlayModeTests : PlayModeTestBase
{
    [UnityTest]
    public IEnumerator LoadScreenPrefab_ShouldSucceed()
    {
        ScreenWidget screen = null;
        yield return PrefabHelper.InstantiateAsync<ScreenWidget>(
            "Assets/Prefabs/Screens/TestScreen.prefab", 
            TestCanvas.transform
        ).ToCoroutine(s => screen = s);

        Assert.IsNotNull(screen);
        PlayModeAssert.IsActive(screen.gameObject);
    }
}
```

---

## 시나리오 러너 (수동 테스트)

### SystemTestRunner 베이스

```csharp
public abstract class SystemTestRunner : MonoBehaviour
{
    protected GameObject _testRoot;
    protected TestCanvas _testCanvas;

    public virtual void SetupTest()
    {
        _testRoot = new GameObject($"[TEST] {GetSystemName()}");
        _testCanvas = TestCanvasFactory.Create(_testRoot.transform);
        OnSetup();
    }

    public virtual void TeardownTest()
    {
        OnTeardown();
        if (_testRoot != null) DestroyImmediate(_testRoot);
    }

    protected abstract string GetSystemName();
    protected abstract void OnSetup();
    protected abstract void OnTeardown();
}
```

### 구현된 러너

| 러너 | 시스템 | 시나리오 |
|------|--------|----------|
| NavigationTestRunner | Navigation | Push/Pop, Visibility, Back |
| SaveManagerTestRunner | SaveManager | Save/Load, Migration |
| AssetManagerTestRunner | AssetManager | Load/Unload, Scope |

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

// 2. Mock 구현 (테스트용)
public class MockTimeService : ITimeService
{
    public long FixedTime { get; set; } = 1700000000;
    public long ServerTimeUtc => FixedTime;
    public DateTime ServerDateTime => DateTimeOffset.FromUnixTimeSeconds(FixedTime).DateTime;
}

// 3. ServiceLocator
public static class Services
{
    private static readonly Dictionary<Type, object> _services = new();
    public static void Register<T>(T service) where T : class => _services[typeof(T)] = service;
    public static T Get<T>() where T : class => _services.TryGetValue(typeof(T), out var s) ? (T)s : null;
    public static void Clear() => _services.Clear();
}
```

### 장점

- Inspector에서 Mock 교체 가능
- 테스트 시 Services.Register로 Mock 주입
- 기존 Singleton 코드와 공존 가능

---

## 관련 문서

- [AITools.md](../Editor/AITools.md) - 에디터 도구
- [UISystem.md](../Common/UISystem.md) - UI 시스템
- [Navigation.md](../Navigation.md) - Navigation 시스템
