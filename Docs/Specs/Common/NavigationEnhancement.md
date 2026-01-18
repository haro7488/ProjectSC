---
type: spec
assembly: Sc.Common
category: Enhancement
status: draft
version: "1.0"
dependencies: [Navigation, Phase3.StageDashboard, Phase4.LiveEvent]
created: 2026-01-18
updated: 2026-01-18
---

# Navigation 강화 (Phase 5.3)

## 목적

Navigation 시스템에 Shortcut, DeepLink, 탭 그룹, 알림 배지 기능 추가

## Phase 연계

| 연계 대상 | 적용 내용 |
|-----------|-----------|
| Phase 3 StageDashboard | 탭 그룹 패턴 참조 |
| Phase 4 LiveEvent | 알림 배지 (HasClaimableReward) |

---

## Shortcut API

### NavigationManager 확장

**위치**: `Assets/Scripts/Common/Navigation/NavigationManager.cs`

```csharp
public class NavigationManager : Singleton<NavigationManager>
{
    // 기존 메서드...

    /// <summary>
    /// 특정 화면으로 바로 이동 (스택에 추가)
    /// Shortcut은 일반 Push와 동일하지만, 명시적 의도 표현
    /// </summary>
    public void Shortcut<T>(object state = null) where T : ScreenWidget
    {
        var context = CreateScreenContext<T>(state);
        Push(context);
    }

    public async UniTask ShortcutAsync<T>(object state = null) where T : ScreenWidget
    {
        var context = CreateScreenContext<T>(state);
        await PushAsync(context);
    }

    /// <summary>
    /// 특정 화면으로 이동하면서 중간 스택 정리
    /// 예: Lobby에서 CharacterDetail로 바로 이동
    /// </summary>
    public async UniTask ShortcutWithCleanAsync<T>(object state = null) where T : ScreenWidget
    {
        // 현재 스택에서 T가 있으면 그 위를 모두 Pop
        // 없으면 일반 Push
        var context = CreateScreenContext<T>(state);
        await PushAsync(context);  // 중복 제거 정책 자동 적용
    }

    private ScreenWidget.Context CreateScreenContext<T>(object state) where T : ScreenWidget
    {
        // 리플렉션으로 CreateContext 호출
        var method = typeof(T).GetMethod("CreateContext", BindingFlags.Public | BindingFlags.Static);
        return (ScreenWidget.Context)method.Invoke(null, new[] { state });
    }
}
```

### 사용 예시

```csharp
// 어디서든 특정 화면으로 바로 이동
NavigationManager.Instance.Shortcut<CharacterDetailScreen>(new CharacterDetailState { CharacterId = "char_001" });

// 비동기 버전
await NavigationManager.Instance.ShortcutAsync<GachaScreen>(new GachaState { SelectedPoolId = "pool_pickup" });
```

---

## DeepLink 시스템

### DeepLinkParser

**위치**: `Assets/Scripts/Common/Navigation/DeepLinkParser.cs`

```csharp
public static class DeepLinkParser
{
    // URL 형식: projectsc://screen/{screenName}?{params}
    // 예:
    //   projectsc://screen/gacha?poolId=xxx
    //   projectsc://screen/character/detail?id=xxx
    //   projectsc://screen/event?eventId=xxx&tab=mission
    //   projectsc://screen/shop?category=package

    private const string SCHEME = "projectsc";
    private const string HOST_SCREEN = "screen";

    public static DeepLinkRoute Parse(string url)
    {
        if (string.IsNullOrEmpty(url)) return null;

        try
        {
            var uri = new Uri(url);

            if (uri.Scheme != SCHEME) return null;
            if (uri.Host != HOST_SCREEN) return null;

            var path = uri.AbsolutePath.TrimStart('/');
            var query = ParseQueryString(uri.Query);

            return CreateRoute(path, query);
        }
        catch
        {
            Log.Warning("DeepLink", $"Failed to parse URL: {url}");
            return null;
        }
    }

    private static DeepLinkRoute CreateRoute(string path, Dictionary<string, string> query)
    {
        return path.ToLower() switch
        {
            "gacha" => new DeepLinkRoute
            {
                ScreenType = typeof(GachaScreen),
                State = new GachaState { SelectedPoolId = query.GetValueOrDefault("poolId") }
            },
            "character/detail" => new DeepLinkRoute
            {
                ScreenType = typeof(CharacterDetailScreen),
                State = new CharacterDetailState { CharacterId = query.GetValueOrDefault("id") }
            },
            "character" or "character/list" => new DeepLinkRoute
            {
                ScreenType = typeof(CharacterListScreen),
                State = null
            },
            "shop" => new DeepLinkRoute
            {
                ScreenType = typeof(ShopScreen),
                State = new ShopState { Category = query.GetValueOrDefault("category") }
            },
            "event" => new DeepLinkRoute
            {
                ScreenType = typeof(EventDetailScreen),
                State = new EventDetailState
                {
                    EventId = query.GetValueOrDefault("eventId"),
                    InitialTab = query.GetValueOrDefault("tab")
                }
            },
            "stage" => new DeepLinkRoute
            {
                ScreenType = typeof(StageDashboardScreen),
                State = null
            },
            _ => null
        };
    }

    private static Dictionary<string, string> ParseQueryString(string query)
    {
        var result = new Dictionary<string, string>();
        if (string.IsNullOrEmpty(query)) return result;

        var pairs = query.TrimStart('?').Split('&');
        foreach (var pair in pairs)
        {
            var kv = pair.Split('=');
            if (kv.Length == 2)
            {
                result[kv[0]] = Uri.UnescapeDataString(kv[1]);
            }
        }
        return result;
    }
}

public class DeepLinkRoute
{
    public Type ScreenType;
    public object State;
}
```

### DeepLinkHandler

**위치**: `Assets/Scripts/Common/Navigation/DeepLinkHandler.cs`

```csharp
public class DeepLinkHandler : MonoBehaviour
{
    private void Awake()
    {
        Application.deepLinkActivated += OnDeepLinkActivated;

        // 앱 시작 시 전달된 URL 처리
        if (!string.IsNullOrEmpty(Application.absoluteURL))
        {
            OnDeepLinkActivated(Application.absoluteURL);
        }
    }

    private void OnDestroy()
    {
        Application.deepLinkActivated -= OnDeepLinkActivated;
    }

    private async void OnDeepLinkActivated(string url)
    {
        Log.Info("DeepLink", $"Received: {url}");

        var route = DeepLinkParser.Parse(url);
        if (route == null)
        {
            Log.Warning("DeepLink", $"Unknown route: {url}");
            return;
        }

        await NavigationManager.Instance.HandleDeepLinkAsync(route);
    }
}
```

### NavigationManager.HandleDeepLinkAsync

```csharp
public async UniTask HandleDeepLinkAsync(DeepLinkRoute route)
{
    if (route == null) return;

    // 게임 초기화 완료 대기
    await UniTask.WaitUntil(() => GameBootstrap.IsInitialized);

    // 현재 모든 팝업 닫기
    CloseAllPopups();

    // 해당 화면으로 이동
    var method = GetType().GetMethod("ShortcutAsync").MakeGenericMethod(route.ScreenType);
    await (UniTask)method.Invoke(this, new[] { route.State });
}
```

---

## 탭 그룹 시스템

### TabGroupWidget

**위치**: `Assets/Scripts/Common/UI/Widgets/TabGroupWidget.cs`

```csharp
public class TabGroupWidget : Widget
{
    [SerializeField] private TabButton[] _tabs;
    [SerializeField] private GameObject[] _contentPanels;

    public event Action<int> OnTabChanged;

    public int CurrentTabIndex { get; private set; } = 0;

    protected override void Awake()
    {
        base.Awake();

        for (int i = 0; i < _tabs.Length; i++)
        {
            int index = i;
            _tabs[i].OnClicked += () => SelectTab(index);
        }
    }

    public void SelectTab(int index)
    {
        if (index < 0 || index >= _tabs.Length) return;
        if (index == CurrentTabIndex) return;

        // 이전 탭 비활성화
        _tabs[CurrentTabIndex].SetSelected(false);
        if (_contentPanels.Length > CurrentTabIndex)
            _contentPanels[CurrentTabIndex].SetActive(false);

        // 새 탭 활성화
        CurrentTabIndex = index;
        _tabs[CurrentTabIndex].SetSelected(true);
        if (_contentPanels.Length > CurrentTabIndex)
            _contentPanels[CurrentTabIndex].SetActive(true);

        OnTabChanged?.Invoke(index);
    }

    public void SetBadge(int tabIndex, bool show)
    {
        if (tabIndex >= 0 && tabIndex < _tabs.Length)
        {
            _tabs[tabIndex].SetBadge(show);
        }
    }

    public void SetBadgeCount(int tabIndex, int count)
    {
        if (tabIndex >= 0 && tabIndex < _tabs.Length)
        {
            _tabs[tabIndex].SetBadgeCount(count);
        }
    }
}
```

### TabButton

**위치**: `Assets/Scripts/Common/UI/Widgets/TabButton.cs`

```csharp
public class TabButton : Widget
{
    [SerializeField] private Button _button;
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _label;
    [SerializeField] private GameObject _selectedIndicator;
    [SerializeField] private GameObject _badge;
    [SerializeField] private TMP_Text _badgeCount;

    [SerializeField] private Color _normalColor = Color.gray;
    [SerializeField] private Color _selectedColor = Color.white;

    public event Action OnClicked;

    private bool _isSelected;

    protected override void Awake()
    {
        base.Awake();
        _button.onClick.AddListener(() => OnClicked?.Invoke());
    }

    public void SetSelected(bool selected)
    {
        _isSelected = selected;
        _selectedIndicator?.SetActive(selected);

        var color = selected ? _selectedColor : _normalColor;
        if (_icon != null) _icon.color = color;
        if (_label != null) _label.color = color;
    }

    public void SetBadge(bool show)
    {
        _badge?.SetActive(show);
    }

    public void SetBadgeCount(int count)
    {
        if (_badge != null) _badge.SetActive(count > 0);
        if (_badgeCount != null)
        {
            _badgeCount.text = count > 99 ? "99+" : count.ToString();
        }
    }
}
```

### LobbyScreen 리팩토링

```
[LobbyScreen] (리팩토링)
├── ScreenHeader (선택적)
├── CurrencyHUD (상단)
├── TabContentArea (중앙, 탭별 컨텐츠)
│   ├── HomeTab
│   │   ├── WelcomeMessage
│   │   ├── EventBanner (Phase 4)
│   │   ├── QuickMenu
│   │   │   ├── [스테이지] → StageDashboard
│   │   │   ├── [상점] → ShopScreen
│   │   │   └── [이벤트] → LiveEventScreen
│   │   └── NoticePanel
│   ├── CharacterTab
│   │   └── CharacterListScreen (임베드)
│   ├── GachaTab
│   │   └── GachaScreen (임베드)
│   └── SettingsTab
│       └── SettingsPanel
└── TabBar (하단)
    ├── [홈] 탭 (badge: 출석/우편)
    ├── [캐릭터] 탭
    ├── [가챠] 탭 (badge: 무료가챠)
    └── [설정] 탭
```

---

## 알림 배지 시스템

### BadgeManager

**위치**: `Assets/Scripts/Core/Managers/BadgeManager.cs`

```csharp
public class BadgeManager : Singleton<BadgeManager>
{
    public event Action<BadgeType, int> OnBadgeChanged;

    private Dictionary<BadgeType, int> _badgeCounts = new();

    /// <summary>
    /// 배지 카운트 조회
    /// </summary>
    public int GetBadgeCount(BadgeType type)
    {
        return _badgeCounts.GetValueOrDefault(type, 0);
    }

    /// <summary>
    /// 배지 존재 여부
    /// </summary>
    public bool HasBadge(BadgeType type)
    {
        return GetBadgeCount(type) > 0;
    }

    /// <summary>
    /// 전체 배지 갱신 (서버 응답 후 호출)
    /// </summary>
    public void RefreshBadges(UserSaveData data, DataManager dataManager)
    {
        // Home: 출석 보상, 우편함
        int homeBadge = CalculateHomeBadge(data);

        // Character: 레벨업 가능 캐릭터
        int characterBadge = CalculateCharacterBadge(data, dataManager);

        // Gacha: 무료 가챠 가능
        int gachaBadge = CalculateGachaBadge(data);

        // Shop: 무료 상품
        int shopBadge = CalculateShopBadge(data, dataManager);

        // Event: 수령 가능 보상 (Phase 4)
        int eventBadge = CalculateEventBadge(data);

        // Stage: 새로운 스테이지 해금
        int stageBadge = CalculateStageBadge(data, dataManager);

        UpdateBadge(BadgeType.Home, homeBadge);
        UpdateBadge(BadgeType.Character, characterBadge);
        UpdateBadge(BadgeType.Gacha, gachaBadge);
        UpdateBadge(BadgeType.Shop, shopBadge);
        UpdateBadge(BadgeType.Event, eventBadge);
        UpdateBadge(BadgeType.Stage, stageBadge);
    }

    private void UpdateBadge(BadgeType type, int count)
    {
        int oldCount = _badgeCounts.GetValueOrDefault(type, 0);
        _badgeCounts[type] = count;

        if (oldCount != count)
        {
            OnBadgeChanged?.Invoke(type, count);
        }
    }

    private int CalculateHomeBadge(UserSaveData data)
    {
        int count = 0;
        // 출석 보상 수령 가능 체크
        // 우편함 미수령 체크
        return count;
    }

    private int CalculateCharacterBadge(UserSaveData data, DataManager dm)
    {
        // 레벨업 가능한 캐릭터 수 (재료 보유)
        return 0;
    }

    private int CalculateGachaBadge(UserSaveData data)
    {
        // 무료 가챠 가능 여부
        return 0;
    }

    private int CalculateShopBadge(UserSaveData data, DataManager dm)
    {
        // 무료 상품 수령 가능
        return 0;
    }

    private int CalculateEventBadge(UserSaveData data)
    {
        int count = 0;
        foreach (var progress in data.EventProgresses.Values)
        {
            count += progress.GetClaimableMissionCount();
        }
        return count;
    }

    private int CalculateStageBadge(UserSaveData data, DataManager dm)
    {
        // 새로 해금된 스테이지
        return 0;
    }
}

public enum BadgeType
{
    Home,
    Character,
    Gacha,
    Shop,
    Event,
    Stage,
}
```

### TabButton 배지 연동

```csharp
// LobbyScreen에서
private void Start()
{
    BadgeManager.Instance.OnBadgeChanged += OnBadgeChanged;
    RefreshAllBadges();
}

private void OnBadgeChanged(BadgeType type, int count)
{
    int tabIndex = type switch
    {
        BadgeType.Home => 0,
        BadgeType.Character => 1,
        BadgeType.Gacha => 2,
        _ => -1
    };

    if (tabIndex >= 0)
    {
        _tabGroup.SetBadgeCount(tabIndex, count);
    }
}

private void RefreshAllBadges()
{
    _tabGroup.SetBadgeCount(0, BadgeManager.Instance.GetBadgeCount(BadgeType.Home));
    _tabGroup.SetBadgeCount(1, BadgeManager.Instance.GetBadgeCount(BadgeType.Character));
    _tabGroup.SetBadgeCount(2, BadgeManager.Instance.GetBadgeCount(BadgeType.Gacha));
}
```

---

## 구현 체크리스트

```
Navigation 강화 (Phase 5.3):

Shortcut:
- [ ] NavigationManager.Shortcut<T> 구현
- [ ] NavigationManager.ShortcutAsync<T> 구현
- [ ] NavigationManager.ShortcutWithCleanAsync<T> 구현

DeepLink:
- [ ] DeepLinkParser.cs 생성
- [ ] DeepLinkRoute.cs 생성
- [ ] DeepLinkHandler.cs 생성
- [ ] NavigationManager.HandleDeepLinkAsync 구현
- [ ] URL 스키마 등록 (iOS/Android)

탭 그룹:
- [ ] TabGroupWidget.cs 생성
- [ ] TabButton.cs 생성
- [ ] LobbyScreen 리팩토링 (탭 구조)
- [ ] TabGroup 프리팹 생성

배지 시스템:
- [ ] BadgeManager.cs 생성
- [ ] BadgeType.cs 생성
- [ ] TabButton 배지 연동
- [ ] LobbyScreen 배지 표시

연동:
- [ ] Phase 4 EventProgress → Event 배지
- [ ] 무료 가챠 체크 → Gacha 배지
- [ ] 출석 보상 체크 → Home 배지
```

---

## 관련 문서

- [Navigation.md](../Navigation.md) - Navigation 시스템 개요
- [Stage.md](../Stage.md) - Phase 3 StageDashboard 패턴
- [LiveEvent.md](../LiveEvent.md) - Phase 4 알림 배지 연계
