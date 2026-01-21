---
type: overview
assembly: Sc.Common
category: Shared
status: draft
version: "1.0"
dependencies: [Sc.Core, Sc.Data, Sc.Packet]
detail_docs: [UISystem, UIComponents, Pool, Utility]
created: 2025-01-14
updated: 2025-01-14
---

# Sc.Common

## 목적
모든 컨텐츠에서 공통으로 사용하는 UI, 풀링, 유틸리티 제공.

## 의존성
- **참조**: Sc.Core, Sc.Data, Sc.Packet
- **참조됨**: 모든 Contents

---

## 핵심 개념

| 개념 | 설명 |
|------|------|
| **Widget** | 모든 UI의 기본 단위. Composition 패턴 |
| **MVP 패턴** | 화면 단위 Widget에 적용. UI와 로직 분리 |
| **Object Pool** | 객체 재사용으로 GC 최소화 |
| **Extension** | 기존 타입에 메서드 추가 |

---

## 클래스 역할 정의

### UI - Widget System

| 클래스 | 역할 | 책임 | 하지 않는 것 |
|--------|------|------|--------------|
| Widget | 모든 UI 기본 클래스 | 생명주기, 계층 관리, 갱신 | 비즈니스 로직 |
| ScreenWidget | 전체 화면 Widget | 화면 단위 구성, Presenter 연결 | 팝업/패널 로직 |
| PanelWidget | 화면 내 섹션 | 패널 표시/숨김 | 전체 화면 관리 |
| PopupWidget | 모달 팝업 | 팝업 스택, 배경 딤 | 화면 전환 |
| BasePresenter\<T\> | 프레젠터 공통 기능 | 이벤트 구독, 데이터 가공, View 갱신 | Unity 컴포넌트 직접 조작 |

### UI - Components

| 클래스 | 역할 | 책임 | 하지 않는 것 |
|--------|------|------|--------------|
| UIButton | 공통 버튼 | 클릭 이벤트, 상태(활성/비활성), 효과음 | 버튼별 로직 |
| UIPopup | 팝업 베이스 | 팝업 열기/닫기, 배경 딤, 스택 | 팝업 내용 |
| UIManager | UI 스택 관리 | 팝업 스택, 최상위 UI 추적, 뒤로가기 | 개별 UI 로직 |

### Pool

| 클래스 | 역할 | 책임 | 하지 않는 것 |
|--------|------|------|--------------|
| IPoolable | 풀링 가능 계약 | OnSpawn/OnDespawn 명세 | 구현 |
| ObjectPool\<T\> | 단일 타입 풀 | 생성, 대여, 반납, 확장 | 여러 타입 관리 |
| PoolManager | 풀 중앙 관리 | 풀 등록/조회, 전역 접근점 | 개별 풀 로직 |

### Utility

| 클래스 | 역할 | 책임 | 하지 않는 것 |
|--------|------|------|--------------|
| CollectionExtensions | 컬렉션 확장 | Shuffle, Random Pick, Safe Get | 컬렉션 생성 |
| MathHelper | 수학 유틸리티 | 확률 계산, 범위 클램프, 보간 | 복잡한 수학 연산 |

### 서비스/브릿지 (구현됨)

| 클래스 | 역할 | 책임 | 하지 않는 것 |
|--------|------|------|--------------|
| PopupQueueService | 팝업 큐잉 서비스 | 보상/알림 팝업 순차 표시 | 팝업 내용 정의 |
| UIEventBridge | 이벤트-UI 브릿지 | Core 이벤트 → UI 연결 | 이벤트 정의, 비즈니스 로직 |
| LoadingService | 로딩 UI 서비스 | 로딩 표시/숨김, 진행률 | 초기화 로직 |

---

## Widget + MVP 흐름

```
┌─────────────────────────────────────────────┐
│              ScreenWidget                   │
│  ┌─────────────────────────────────────┐    │
│  │  Widget (자식들)                     │    │
│  │  ├── HeaderWidget                   │    │
│  │  │     └── CurrencyWidget           │    │
│  │  └── ContentWidget                  │    │
│  └─────────────────────────────────────┘    │
└──────────────────┬──────────────────────────┘
                   │
            ┌──────▼──────┐      ┌─────────┐
            │  Presenter  │ ←──→ │  Model  │
            └─────────────┘      └─────────┘
```

### 책임 분리

| 계층 | 알아야 하는 것 | 몰라야 하는 것 |
|------|----------------|----------------|
| **Widget** | UI 요소, 자식 Widget, 표시 갱신 | 비즈니스 로직, 데이터 구조 |
| **Presenter** | Widget 인터페이스, Model | Unity 컴포넌트, UI 세부사항 |
| **Model** | 데이터, 규칙 | Widget, Presenter |

---

## 클래스 관계도

```
┌─ UI (Widget System) ─────────────────────────┐
│                                               │
│  ┌────────────────────────────────────────┐  │
│  │               Widget (Base)             │  │
│  └────────────────────────────────────────┘  │
│        ↑               ↑              ↑      │
│        │               │              │      │
│  ┌───────────┐  ┌───────────┐  ┌───────────┐│
│  │ScreenWidget│ │PanelWidget│  │PopupWidget ││
│  └───────────┘  └───────────┘  └───────────┘│
│        │                                     │
│        │ 연결                                │
│  ┌─────────────────┐         ┌───────────┐  │
│  │BasePresenter<T> │         │ UIManager │  │
│  └─────────────────┘         └───────────┘  │
└───────────────────────────────────────────────┘

┌─ Pool ────────────────────────────────────────┐
│  ┌───────────┐    ┌───────────────┐           │
│  │ IPoolable │←───│ ObjectPool<T> │           │
│  └───────────┘    └───────────────┘           │
│                          ↑                    │
│                          │ 관리               │
│                   ┌─────────────┐             │
│                   │ PoolManager │             │
│                   └─────────────┘             │
└───────────────────────────────────────────────┘

┌─ Utility ─────────────────────────────────────┐
│  ┌─────────────────────┐  ┌────────────┐      │
│  │ CollectionExtensions│  │ MathHelper │      │
│  └─────────────────────┘  └────────────┘      │
└───────────────────────────────────────────────┘
```

---

## 사용 예시

```csharp
// Widget + MVP 패턴
public class LobbyScreenWidget : ScreenWidget { }
public class LobbyPresenter : BasePresenter<LobbyScreenWidget> { }

// Widget 생명주기
widget.OnInitialize();   // 최초 1회
widget.OnBind(data);     // 데이터 주입
widget.OnShow();         // 표시
widget.OnRefresh();      // 갱신
widget.OnHide();         // 숨김

// 오브젝트 풀
var bullet = PoolManager.Instance.Spawn<Bullet>();
PoolManager.Instance.Despawn(bullet);

// 확장 메서드
var randomItem = itemList.RandomPick();
var shuffled = cardList.Shuffle();
```

---

## 설계 원칙

1. **재사용성**: 특정 컨텐츠에 종속되지 않음
2. **확장성**: 상속/구현으로 기능 확장
3. **성능**: 풀링으로 GC 최소화

---

## 상세 문서
- [UISystem.md](Common/UISystem.md) - UI 시스템 상세 (Widget, MVP, 생명주기)
- [UIComponents.md](Common/UIComponents.md) - UI 컴포넌트 상세 (Button, Popup, Manager)
- [Pool.md](Common/Pool.md) - 오브젝트 풀 상세
- [Utility.md](Common/Utility.md) - 유틸리티 상세

---

## 상태

| 분류 | 파일 수 | 스펙 | 구현 |
|------|---------|------|------|
| UI/Base | 6 | ✅ | ✅ |
| UI/Components | 8 | ✅ | ✅ |
| UI/Services | 3 | ✅ | ✅ |
| Pool | 3 | ✅ | ⬜ |
| Utility | 2 | ✅ | ⬜ |

---

## PopupQueueService

팝업 순차 표시 서비스. `IPopupQueueService` (Core) 구현체.

```csharp
// 보상 팝업 추가
popupQueue.EnqueueReward("획득 보상", rewards);

// 알림 팝업 추가
popupQueue.EnqueueNotification("알림", "완료!");

// 큐 처리
await popupQueue.ProcessQueueAsync();
```

| 메서드 | 설명 |
|--------|------|
| `EnqueueReward(title, rewards)` | 보상 팝업 큐에 추가 |
| `EnqueueNotification(title, message)` | 알림 팝업 큐에 추가 |
| `ProcessQueueAsync()` | 모든 팝업 순차 표시 |
| `Clear()` | 대기 중인 팝업 제거 |
| `PendingCount` | 대기 중인 팝업 수 |
| `IsProcessing` | 처리 중 여부 |

---

## UIEventBridge

Core 이벤트 → UI 연결 브릿지. 싱글톤으로 동작.

**처리 이벤트:**
- `ShowLoadingEvent` → `LoadingService.ShowProgress()`
- `HideLoadingEvent` → `LoadingService.Hide()`
- `LoadingProgressEvent` → `LoadingService.UpdateProgress()`
- `ShowConfirmationEvent` → `ConfirmPopup.Open()`

**사용 예시:**
```csharp
// Core에서 로딩 표시 (UI 직접 참조 없이)
EventManager.Instance.Publish(new ShowLoadingEvent {
    InitialProgress = 0f,
    Message = "로딩 중..."
});

// 확인 팝업
EventManager.Instance.Publish(new ShowConfirmationEvent {
    Title = "확인",
    Message = "삭제하시겠습니까?",
    OnConfirm = () => { /* 삭제 */ }
});
```
