---
type: spec
assembly: Sc.Common
class: ScreenHeader
category: UI
status: draft
version: "1.0"
dependencies: [Widget, CurrencyHUD, EventManager, DataManager]
created: 2025-01-16
updated: 2025-01-16
---

# ScreenHeader

## 역할

Screen 상단에 표시되는 공용 헤더 UI. 데이터 기반 설정으로 화면별 버튼 표시/숨김 제어.

---

## 책임

- 데이터 기반 버튼 표시/숨김 제어
- 재화 표시 (CurrencyHUD 통합)
- 버튼 클릭 시 이벤트 발행
- Screen 전환/탭 변경 시 설정 갱신

## 비책임

- 버튼 클릭 후 실제 동작 (Screen에서 처리)
- 재화 데이터 관리 (DataManager 담당)
- Screen 전환 로직 (NavigationManager 담당)

---

## 설계 결정

### 결정 1: 싱글턴 인스턴스

**선택**: 씬에 하나의 ScreenHeader 인스턴스

**근거**:
- 메모리 효율성 (Screen마다 인스턴스 생성 불필요)
- 일관된 위치/애니메이션 보장
- Screen 전환 시 설정만 변경

### 결정 2: 데이터 기반 설정

**선택**: ScriptableObject (JSON → SO 파이프라인)

**근거**:
- 기획자가 데이터 패치로 수정 가능
- 기존 마스터 데이터 파이프라인과 동일한 방식
- 코드 수정 없이 버튼 표시 조합 변경 가능

### 결정 3: Screen이 능동적으로 설정 호출

**선택**: `ScreenHeader.Configure(configId)` 방식

**근거**:
- 같은 Screen이라도 탭/상태에 따라 Header 설정 변경 가능
- Screen이 자신의 상태를 가장 잘 알고 있음
- 예: 상점 Screen에서 뽑기A 탭 → A재화, 뽑기B 탭 → B재화

### 결정 4: 이벤트 기반 동작 정의

**선택**: 버튼 클릭 시 이벤트 발행

**근거**:
- Screen과 Header의 느슨한 결합
- 여러 리스너가 동시에 처리 가능
- 테스트 용이성

---

## 인터페이스

### ScreenHeader

| 멤버 | 타입 | 설명 |
|------|------|------|
| Instance | ScreenHeader | 싱글턴 인스턴스 (씬에서 참조) |
| Configure(string) | void | Config ID로 헤더 설정 |
| Hide() | void | 헤더 전체 숨김 |
| Show() | void | 헤더 표시 |
| IsVisible | bool | 현재 표시 상태 |

### ScreenHeaderConfigData (ScriptableObject)

| 필드 | 타입 | 설명 |
|------|------|------|
| Id | string | 고유 식별자 (예: "lobby_default") |
| Title | string | 타이틀 텍스트 (선택) |
| ShowBackButton | bool | 뒤로가기 버튼 표시 |
| ShowProfileButton | bool | 프로필 버튼 표시 |
| ShowMenuButton | bool | 메뉴 버튼 표시 |
| ShowMailButton | bool | 우편함 버튼 표시 |
| ShowNoticeButton | bool | 공지사항 버튼 표시 |
| ShowCurrency | bool | 재화 표시 영역 |
| CurrencyTypes | List\<string\> | 표시할 재화 ID 목록 (추후 확정) |

### ScreenHeaderConfigDatabase (ScriptableObject)

| 멤버 | 타입 | 설명 |
|------|------|------|
| Configs | List\<ScreenHeaderConfigData\> | 전체 설정 목록 |
| GetById(string) | ScreenHeaderConfigData | ID로 설정 조회 |

---

## 이벤트 정의

```csharp
// Sc.Event/Header/HeaderEvents.cs
public struct HeaderBackClickedEvent : IEvent { }
public struct HeaderProfileClickedEvent : IEvent { }
public struct HeaderMenuClickedEvent : IEvent { }
public struct HeaderMailClickedEvent : IEvent { }
public struct HeaderNoticeClickedEvent : IEvent { }
```

---

## 동작 흐름

### Screen 진입 시

```
[NavigationManager.Push(Screen)]
    │
    ▼
Screen.OnBind(state)
    │
    ├─ Header 필요? ──No──▶ ScreenHeader.Instance.Hide()
    │                         └─ return
    └─ Yes
        │
        ▼
    ScreenHeader.Instance.Configure("config_id")
        │
        ├─ Database에서 ConfigData 조회
        ├─ 버튼별 SetActive 적용
        ├─ Title 텍스트 설정
        └─ CurrencyHUD 표시/숨김
```

### 탭 변경 시

```
[ShopScreen.OnTabChanged(index)]
    │
    ├─ index == 0 ──▶ ScreenHeader.Configure("shop_gacha_a")
    ├─ index == 1 ──▶ ScreenHeader.Configure("shop_gacha_b")
    └─ index == 2 ──▶ ScreenHeader.Configure("shop_package")
```

### 버튼 클릭 시

```
[Back 버튼 클릭]
    │
    ▼
EventManager.Publish(new HeaderBackClickedEvent())
    │
    ▼
[구독 중인 Screen에서 처리]
    └─ NavigationManager.Back() 또는 커스텀 동작
```

---

## 사용 패턴

### Screen에서 Header 설정

```csharp
// GachaScreen.cs
protected override void OnBind(GachaState state)
{
    ScreenHeader.Instance.Configure("gacha_main");
}
```

### Header 숨김 (TitleScreen 등)

```csharp
// TitleScreen.cs
protected override void OnBind(TitleState state)
{
    ScreenHeader.Instance.Hide();
}
```

### 버튼 이벤트 처리

```csharp
// LobbyScreen.cs
protected override void OnShow()
{
    EventManager.Instance.Subscribe<HeaderMenuClickedEvent>(OnMenuClicked);
}

protected override void OnHide()
{
    EventManager.Instance.Unsubscribe<HeaderMenuClickedEvent>(OnMenuClicked);
}

private void OnMenuClicked(HeaderMenuClickedEvent e)
{
    SettingsPopup.Open(new SettingsState());
}
```

---

## 데이터 예시

### ScreenHeaderConfig.json

```json
[
  {
    "Id": "lobby_default",
    "Title": "",
    "ShowBackButton": false,
    "ShowProfileButton": true,
    "ShowMenuButton": true,
    "ShowMailButton": true,
    "ShowNoticeButton": true,
    "ShowCurrency": true,
    "CurrencyTypes": ["gold", "gem"]
  },
  {
    "Id": "gacha_main",
    "Title": "소환",
    "ShowBackButton": true,
    "ShowProfileButton": false,
    "ShowMenuButton": false,
    "ShowMailButton": false,
    "ShowNoticeButton": false,
    "ShowCurrency": true,
    "CurrencyTypes": ["gem"]
  },
  {
    "Id": "character_detail",
    "Title": "캐릭터 상세",
    "ShowBackButton": true,
    "ShowProfileButton": false,
    "ShowMenuButton": false,
    "ShowMailButton": false,
    "ShowNoticeButton": false,
    "ShowCurrency": false,
    "CurrencyTypes": []
  }
]
```

---

## 파일 구조

```
Assets/Scripts/
├── Data/Master/
│   ├── ScreenHeaderConfigData.cs      ← SO 정의
│   └── ScreenHeaderConfigDatabase.cs  ← Database SO
│
├── Common/UI/Widgets/
│   └── ScreenHeader.cs                ← 싱글턴 Widget
│
├── Event/Header/
│   └── HeaderEvents.cs                ← 이벤트 정의
│
└── Editor/Data/
    └── ScreenHeaderConfigImporter.cs  ← JSON → SO 변환

Assets/Data/
├── MasterData/
│   └── ScreenHeaderConfig.json        ← 기획 데이터
└── Generated/
    └── ScreenHeaderConfigDatabase.asset
```

---

## 주의사항

- Screen.OnBind()에서 Header 설정 필수 (숨김 포함)
- Header가 필요 없는 화면은 반드시 `Hide()` 호출
- 탭 변경 시 Header 재설정 필요
- CurrencyTypes는 재화 기획 확정 후 재작업 예정

---

## 확장 고려사항

### 재화 시스템 확정 후

- CurrencyType Enum 또는 CurrencyId 체계 확정
- CurrencyHUD 확장 (동적 재화 타입 지원)
- 재화별 충전 버튼 이벤트 분리

### 추가 버튼

- Config에 새 필드 추가 (ShowXxxButton)
- ScreenHeader에 버튼 추가
- 해당 이벤트 정의

---

## 관련

- [UISystem.md](UISystem.md) - Widget 기반 구조
- [UIComponents.md](UIComponents.md) - CurrencyHUD
- [Navigation.md](../Navigation.md) - Screen 전환
- [Event.md](../Event.md) - 이벤트 시스템
