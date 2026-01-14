# 아키텍처

## 설계 원칙

1. **Assembly 분리**: 컴파일 시간 단축 및 의존성 명확화
2. **InGame/OutGame 분리**: 게임 영역별 모듈 독립
3. **이벤트 기반 통신**: 컨텐츠 간 직접 참조 최소화
4. **MVP 패턴**: UI와 로직 분리

---

## 폴더 구조

```
Assets/Scripts/
│
├── Data/                           # [Sc.Data] 순수 데이터 정의
│   ├── Enums/                     # 공용 열거형
│   ├── Structs/                   # 공용 구조체
│   └── ScriptableObjects/         # SO 정의
│
├── Packet/                         # [Sc.Packet] 이벤트/메시지
│   ├── Common/                    # 공통 이벤트
│   ├── InGame/                    # 인게임 이벤트
│   └── OutGame/                   # 아웃게임 이벤트
│
├── Core/                           # [Sc.Core] 핵심 시스템
│   ├── Base/                      # Singleton 등 베이스
│   ├── Managers/                  # 전역 매니저
│   └── Systems/                   # 범용 시스템
│
├── Common/                         # [Sc.Common] 공통 모듈
│   ├── UI/                        # 공통 UI (Base, Components)
│   ├── Pool/                      # 오브젝트 풀
│   └── Utility/                   # 유틸리티
│
└── Contents/
    ├── Shared/                    # 인게임/아웃게임 공통
    │   ├── Character/             # [Sc.Contents.Character]
    │   └── Inventory/             # [Sc.Contents.Inventory]
    │
    ├── InGame/                    # 인게임 전용
    │   ├── Battle/                # [Sc.Contents.Battle]
    │   └── Skill/                 # [Sc.Contents.Skill]
    │
    └── OutGame/                   # 아웃게임 전용
        ├── Lobby/                 # [Sc.Contents.Lobby]
        ├── Gacha/                 # [Sc.Contents.Gacha]
        ├── Shop/                  # [Sc.Contents.Shop]
        └── Quest/                 # [Sc.Contents.Quest]
```

---

## Assembly 의존성

```
                ┌───────────┐
                │  Sc.Data  │ ← 의존성 없음
                └─────┬─────┘
                      ↓
                ┌───────────┐
                │ Sc.Packet │ → Sc.Data
                └─────┬─────┘
                      ↓
                ┌───────────┐
                │  Sc.Core  │ → Sc.Data, Sc.Packet
                └─────┬─────┘
                      ↓
                ┌───────────┐
                │ Sc.Common │ → Sc.Core, Sc.Data, Sc.Packet
                └─────┬─────┘
                      ↓
        ┌─────────────┴─────────────┐
        ↓                           ↓
┌──────────────┐            ┌──────────────┐
│   Shared/*   │            │  InGame/*    │
│              │←───────────│  OutGame/*   │
└──────────────┘            └──────────────┘
```

### Assembly 참조 테이블

| Assembly | 참조 |
|----------|------|
| `Sc.Data` | (없음) |
| `Sc.Packet` | Sc.Data |
| `Sc.Core` | Sc.Data, Sc.Packet |
| `Sc.Common` | Sc.Core, Sc.Data, Sc.Packet |
| `Sc.Contents.Character` | Sc.Common |
| `Sc.Contents.Inventory` | Sc.Common |
| `Sc.Contents.Battle` | Sc.Common, Sc.Contents.Character |
| `Sc.Contents.Skill` | Sc.Common, Sc.Contents.Character |
| `Sc.Contents.Lobby` | Sc.Common |
| `Sc.Contents.Gacha` | Sc.Common, Sc.Contents.Character |
| `Sc.Contents.Shop` | Sc.Common, Sc.Contents.Inventory |
| `Sc.Contents.Quest` | Sc.Common |

---

## 컨텐츠 간 통신

### 원칙
- **상하관계 있음**: Assembly 직접 참조
- **상하관계 없음**: Packet 이벤트 통신

### 통신 다이어그램

```
┌─────────────┐  Packet   ┌─────────────┐
│   InGame    │◄────────►│   OutGame   │
│  (Battle)   │  Events   │  (Lobby)    │
└──────┬──────┘           └──────┬──────┘
       │                         │
       └───────────┬─────────────┘
                   ↓
            ┌──────────────┐
            │    Shared    │
            │ (Character)  │
            └──────────────┘
```

### 이벤트 예시

```csharp
// Packet/InGame/BattleEvents.cs
public struct BattleEndEvent
{
    public bool IsVictory;
    public List<RewardData> Rewards;
}

// Battle에서 발행
EventManager.Instance.Publish(new BattleEndEvent { ... });

// Lobby에서 구독
EventManager.Instance.Subscribe<BattleEndEvent>(OnBattleEnd);
```

---

## 컨텐츠 영역 분류

| 영역 | 컨텐츠 | 설명 |
|------|--------|------|
| **Shared** | Character | 캐릭터 데이터, 스탯, 팩토리 |
| **Shared** | Inventory | 인벤토리, 아이템 관리 |
| **InGame** | Battle | 전투 흐름, 턴 관리 |
| **InGame** | Skill | 스킬 실행, 버프/디버프 |
| **OutGame** | Lobby | 메인 로비, 메뉴 |
| **OutGame** | Gacha | 캐릭터 뽑기 |
| **OutGame** | Shop | 상점 |
| **OutGame** | Quest | 퀘스트, 업적 |

---

## Namespace 규칙

```csharp
namespace Sc.Data { }
namespace Sc.Data.Enums { }
namespace Sc.Data.Structs { }
namespace Sc.Packet.InGame { }
namespace Sc.Packet.OutGame { }
namespace Sc.Core { }
namespace Sc.Core.Managers { }
namespace Sc.Common { }
namespace Sc.Common.UI { }
namespace Sc.Contents.Character { }
namespace Sc.Contents.Battle { }
```
