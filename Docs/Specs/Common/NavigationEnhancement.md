---
type: spec
assembly: Sc.Common, Sc.Core
category: Enhancement
status: draft
version: "2.0"
dependencies: [Navigation, LiveEvent, Shop]
created: 2026-01-18
updated: 2026-01-21
---

# Navigation 강화 (Phase 5.3)

## 목적

Navigation 시스템에 탭 기반 로비, 알림 배지 기능 추가

## 변경 이력

| 버전 | 날짜 | 변경 내용 |
|------|------|-----------|
| 2.0 | 2026-01-21 | Shortcut API/DeepLink 삭제, BadgeManager IBadgeProvider 패턴, LobbyScreen 탭 구조 |
| 1.0 | 2026-01-18 | 초안 |

---

## 탭 그룹 시스템

> **기존 구현 참조** - 이미 구현되어 있음

| 파일 | 위치 |
|------|------|
| `TabGroupWidget.cs` | `Assets/Scripts/Common/UI/Widgets/` |
| `TabButton.cs` | `Assets/Scripts/Common/UI/Widgets/` |

### 주요 API

```csharp
// TabGroupWidget
void SelectTab(int index);
void SetBadge(int tabIndex, bool show);
void SetBadgeCount(int tabIndex, int count);
event Action<int> OnTabChanged;

// TabButton
void SetSelected(bool selected);
void SetBadge(bool show);
void SetBadgeCount(int count);
```

---

## 알림 배지 시스템

### BadgeType

**위치**: `Assets/Scripts/Core/Enums/BadgeType.cs`

```csharp
namespace Sc.Core
{
    public enum BadgeType
    {
        Home,       // 출석, 우편
        Character,  // 레벨업 가능
        Gacha,      // 무료 가챠
        Settings,   // (미사용)
        Event,      // 수령 가능 보상
        Shop,       // 무료 상품
        Stage,      // 새 스테이지
    }
}
```

### IBadgeProvider 인터페이스

**위치**: `Assets/Scripts/Core/Services/IBadgeProvider.cs`

```csharp
namespace Sc.Core
{
    /// <summary>
    /// 배지 카운트 제공자 인터페이스
    /// 각 Contents에서 구현하여 BadgeManager에 등록
    /// </summary>
    public interface IBadgeProvider
    {
        BadgeType Type { get; }
        int CalculateBadgeCount();
    }
}
```

### BadgeManager

**위치**: `Assets/Scripts/Core/Services/BadgeManager.cs`

```csharp
using System;
using System.Collections.Generic;

namespace Sc.Core
{
    /// <summary>
    /// 배지 집계/캐시 관리자
    /// 계산 로직은 각 Contents의 IBadgeProvider에 위임
    /// </summary>
    public class BadgeManager : Singleton<BadgeManager>
    {
        public event Action<BadgeType, int> OnBadgeChanged;

        private readonly Dictionary<BadgeType, int> _counts = new();
        private readonly List<IBadgeProvider> _providers = new();

        /// <summary>
        /// Provider 등록 (각 Contents 초기화 시 호출)
        /// </summary>
        public void Register(IBadgeProvider provider)
        {
            if (!_providers.Contains(provider))
            {
                _providers.Add(provider);
            }
        }

        /// <summary>
        /// Provider 해제
        /// </summary>
        public void Unregister(IBadgeProvider provider)
        {
            _providers.Remove(provider);
        }

        /// <summary>
        /// 전체 배지 갱신 (로비 진입 시 호출)
        /// </summary>
        public void RefreshAll()
        {
            foreach (var provider in _providers)
            {
                int count = provider.CalculateBadgeCount();
                SetBadge(provider.Type, count);
            }
        }

        /// <summary>
        /// 특정 타입 배지만 갱신
        /// </summary>
        public void Refresh(BadgeType type)
        {
            var provider = _providers.Find(p => p.Type == type);
            if (provider != null)
            {
                int count = provider.CalculateBadgeCount();
                SetBadge(type, count);
            }
        }

        /// <summary>
        /// 배지 직접 설정 (Provider 없이 수동 설정 시)
        /// </summary>
        public void SetBadge(BadgeType type, int count)
        {
            int oldCount = _counts.GetValueOrDefault(type, 0);
            _counts[type] = count;

            if (oldCount != count)
            {
                OnBadgeChanged?.Invoke(type, count);
            }
        }

        /// <summary>
        /// 배지 카운트 조회
        /// </summary>
        public int GetBadge(BadgeType type)
        {
            return _counts.GetValueOrDefault(type, 0);
        }

        /// <summary>
        /// 배지 존재 여부
        /// </summary>
        public bool HasBadge(BadgeType type)
        {
            return GetBadge(type) > 0;
        }
    }
}
```

### Provider 구현 예시

```csharp
// Sc.Contents.Event
public class EventBadgeProvider : IBadgeProvider
{
    private readonly DataManager _dataManager;

    public BadgeType Type => BadgeType.Event;

    public EventBadgeProvider(DataManager dataManager)
    {
        _dataManager = dataManager;
    }

    public int CalculateBadgeCount()
    {
        int count = 0;
        var userData = _dataManager.UserData;

        foreach (var progress in userData.EventProgresses.Values)
        {
            count += progress.GetClaimableMissionCount();
        }

        return count;
    }
}
```

---

## LobbyScreen 리팩토링

### 구조

```
LobbyScreen
├── CurrencyHUD (상단)
├── TabContentArea (중앙, 탭별 컨텐츠)
│   ├── HomeTabContent (Panel)
│   │   ├── WelcomeMessage
│   │   ├── EventBanner
│   │   └── QuickMenu
│   │       ├── [스테이지] → InGameContentDashboard
│   │       ├── [상점] → ShopScreen
│   │       └── [이벤트] → LiveEventScreen
│   ├── CharacterTabContent (Panel)
│   │   └── 캐릭터 목록 표시
│   ├── GachaTabContent (Panel)
│   │   └── 가챠 풀 목록 표시
│   └── SettingsTabContent (Panel)
│       └── 설정 옵션들
├── TabBar (하단)
│   ├── [홈] 탭 (badge: Home)
│   ├── [캐릭터] 탭 (badge: Character)
│   ├── [가챠] 탭 (badge: Gacha)
│   └── [설정] 탭
└── LobbyEntryTaskRunner (기존 유지)
```

### TabContent 베이스 클래스

**위치**: `Assets/Scripts/Contents/OutGame/Lobby/Tabs/LobbyTabContent.cs`

```csharp
using UnityEngine;

namespace Sc.Contents.Lobby
{
    /// <summary>
    /// 로비 탭 컨텐츠 베이스 클래스
    /// </summary>
    public abstract class LobbyTabContent : MonoBehaviour
    {
        /// <summary>
        /// 탭 활성화 시 호출
        /// </summary>
        public virtual void OnTabSelected() { }

        /// <summary>
        /// 탭 비활성화 시 호출
        /// </summary>
        public virtual void OnTabDeselected() { }

        /// <summary>
        /// 데이터 갱신
        /// </summary>
        public abstract void Refresh();
    }
}
```

### HomeTabContent

**위치**: `Assets/Scripts/Contents/OutGame/Lobby/Tabs/HomeTabContent.cs`

```csharp
using UnityEngine;
using UnityEngine.UI;

namespace Sc.Contents.Lobby
{
    public class HomeTabContent : LobbyTabContent
    {
        [Header("UI References")]
        [SerializeField] private Button _stageButton;
        [SerializeField] private Button _shopButton;
        [SerializeField] private Button _eventButton;

        private void Awake()
        {
            _stageButton.onClick.AddListener(OnStageClicked);
            _shopButton.onClick.AddListener(OnShopClicked);
            _eventButton.onClick.AddListener(OnEventClicked);
        }

        public override void Refresh()
        {
            // 배너, 공지 등 갱신
        }

        private void OnStageClicked()
        {
            InGameContentDashboard.Open();
        }

        private void OnShopClicked()
        {
            ShopScreen.Open();
        }

        private void OnEventClicked()
        {
            LiveEventScreen.Open();
        }
    }
}
```

### CharacterTabContent

**위치**: `Assets/Scripts/Contents/OutGame/Lobby/Tabs/CharacterTabContent.cs`

```csharp
using System.Collections.Generic;
using UnityEngine;
using Sc.Data;

namespace Sc.Contents.Lobby
{
    public class CharacterTabContent : LobbyTabContent
    {
        [SerializeField] private Transform _listContainer;
        [SerializeField] private CharacterListItem _itemPrefab;

        private List<CharacterListItem> _items = new();

        public override void OnTabSelected()
        {
            Refresh();
        }

        public override void Refresh()
        {
            var characters = DataManager.Instance.UserData.Characters;

            // 아이템 생성/갱신 로직
            for (int i = 0; i < characters.Count; i++)
            {
                if (i >= _items.Count)
                {
                    var item = Instantiate(_itemPrefab, _listContainer);
                    _items.Add(item);
                }
                _items[i].Setup(characters[i]);
                _items[i].gameObject.SetActive(true);
            }

            // 초과 아이템 비활성화
            for (int i = characters.Count; i < _items.Count; i++)
            {
                _items[i].gameObject.SetActive(false);
            }
        }
    }
}
```

### LobbyScreen 수정

**위치**: `Assets/Scripts/Contents/OutGame/Lobby/LobbyScreen.cs`

```csharp
using UnityEngine;
using Sc.Common;
using Sc.Core;

namespace Sc.Contents.Lobby
{
    public class LobbyScreen : ScreenWidget
    {
        [Header("Tab System")]
        [SerializeField] private TabGroupWidget _tabGroup;
        [SerializeField] private LobbyTabContent[] _tabContents;

        [Header("Existing")]
        [SerializeField] private LobbyEntryTaskRunner _taskRunner;

        private void Awake()
        {
            _tabGroup.OnTabChanged += OnTabChanged;
            BadgeManager.Instance.OnBadgeChanged += OnBadgeChanged;
        }

        private void OnDestroy()
        {
            if (BadgeManager.Instance != null)
            {
                BadgeManager.Instance.OnBadgeChanged -= OnBadgeChanged;
            }
        }

        protected override void OnShow()
        {
            base.OnShow();

            // 배지 전체 갱신
            BadgeManager.Instance.RefreshAll();
            RefreshAllBadges();

            // 초기 탭 선택
            int initialTab = State is LobbyState lobbyState ? lobbyState.ActiveTabIndex : 0;
            _tabGroup.SelectTab(initialTab);

            // 진입 태스크 실행
            _taskRunner?.RunAsync().Forget();
        }

        private void OnTabChanged(int index)
        {
            for (int i = 0; i < _tabContents.Length; i++)
            {
                if (i == index)
                {
                    _tabContents[i].gameObject.SetActive(true);
                    _tabContents[i].OnTabSelected();
                }
                else
                {
                    _tabContents[i].OnTabDeselected();
                    _tabContents[i].gameObject.SetActive(false);
                }
            }
        }

        private void OnBadgeChanged(BadgeType type, int count)
        {
            int tabIndex = GetTabIndexForBadge(type);
            if (tabIndex >= 0)
            {
                _tabGroup.SetBadgeCount(tabIndex, count);
            }
        }

        private void RefreshAllBadges()
        {
            _tabGroup.SetBadgeCount(0, BadgeManager.Instance.GetBadge(BadgeType.Home));
            _tabGroup.SetBadgeCount(1, BadgeManager.Instance.GetBadge(BadgeType.Character));
            _tabGroup.SetBadgeCount(2, BadgeManager.Instance.GetBadge(BadgeType.Gacha));
        }

        private int GetTabIndexForBadge(BadgeType type)
        {
            return type switch
            {
                BadgeType.Home => 0,
                BadgeType.Character => 1,
                BadgeType.Gacha => 2,
                _ => -1
            };
        }
    }

    public class LobbyState
    {
        public int ActiveTabIndex { get; set; } = 0;
    }
}
```

---

## 구현 체크리스트

```
Navigation 강화 (Phase 5.3):

BadgeManager:
- [x] BadgeType.cs 생성
- [x] IBadgeProvider.cs 생성
- [x] BadgeManager.cs 생성
- [x] EventBadgeProvider 구현
- [x] GachaBadgeProvider 구현 (무료 가챠 - 플레이스홀더)
- [x] ShopBadgeProvider 구현 (무료 상품)

LobbyScreen 리팩토링:
- [x] LobbyTabContent.cs 베이스 클래스
- [x] HomeTabContent.cs
- [x] CharacterTabContent.cs
- [x] GachaTabContent.cs
- [x] SettingsTabContent.cs
- [x] LobbyScreen.cs 수정 (탭 시스템 통합)
- [x] LobbyScreen 프리팹 재구성 (에디터 도구: SC Tools/Lobby/Setup Tab System)

연동:
- [x] LobbyScreen.OnShow에서 BadgeManager.RefreshAll 호출
- [x] TabGroupWidget 배지 표시 연동
```

---

## 삭제된 기능

> 설계 검토 결과 아래 기능은 제외됨 (2026-01-21)

| 기능 | 삭제 사유 |
|------|-----------|
| **Shortcut API** | 기존 `Screen.Open()` 정적 메서드로 충분, 리플렉션 사용 불필요 |
| **DeepLink 시스템** | 푸시/외부 연동 없이 실질적 사용처 없음, 포트폴리오 범위 초과 |

---

## 관련 문서

- [Navigation.md](Navigation.md) - Navigation 시스템 개요
- [Stage.md](../Stage.md) - Stage 시스템
- [LiveEvent.md](../LiveEvent.md) - LiveEvent 시스템
