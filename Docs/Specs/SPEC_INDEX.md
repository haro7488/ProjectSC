# 스펙 개요

## Assembly 목록

### 기반 레이어

| Assembly | 설명 | 상태 | 상세 |
|----------|------|------|------|
| Sc.Data | 순수 데이터 정의 | ⬜ | [Data.md](Data.md) |
| Sc.Packet | 이벤트/메시지 정의 | ⬜ | [Packet.md](Packet.md) |
| Sc.Core | 핵심 시스템 | ⬜ | [Core.md](Core.md) |
| Sc.Common | 공통 모듈 | ⬜ | [Common.md](Common.md) |

### Contents - Shared

| Assembly | 설명 | 패턴 | 상태 | 상세 |
|----------|------|------|------|------|
| Sc.Contents.Character | 캐릭터 시스템 | Factory, Flyweight | ⬜ | [Character.md](Character.md) |
| Sc.Contents.Inventory | 인벤토리 시스템 | - | ⬜ | [Inventory.md](Inventory.md) |

### Contents - InGame

| Assembly | 설명 | 패턴 | 상태 | 상세 |
|----------|------|------|------|------|
| Sc.Contents.Battle | 전투 시스템 | State, Command | ⬜ | [Battle.md](Battle.md) |
| Sc.Contents.Skill | 스킬/버프 시스템 | Decorator | ⬜ | [Skill.md](Skill.md) |

### Contents - OutGame

| Assembly | 설명 | 패턴 | 상태 | 상세 |
|----------|------|------|------|------|
| Sc.Contents.Lobby | 로비 시스템 | - | ⬜ | [Lobby.md](Lobby.md) |
| Sc.Contents.Gacha | 가챠 시스템 | Strategy | ⬜ | [Gacha.md](Gacha.md) |
| Sc.Contents.Shop | 상점 시스템 | - | ⬜ | [Shop.md](Shop.md) |
| Sc.Contents.Quest | 퀘스트 시스템 | Composite | ⬜ | [Quest.md](Quest.md) |

---

## 상태 범례

- ⬜ 대기 | 🔨 진행 중 | ✅ 완료
