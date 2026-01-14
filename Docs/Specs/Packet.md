# Sc.Packet

## 개요
이벤트/메시지 데이터 정의

## 참조
- Sc.Data

---

## 구조

```
Packet/
├── Common/
│   └── CommonEvents.cs
├── InGame/
│   ├── BattleEvents.cs
│   └── SkillEvents.cs
└── OutGame/
    ├── LobbyEvents.cs
    ├── GachaEvents.cs
    └── QuestEvents.cs
```

---

## 클래스 목록

### Common

| 이벤트 | 설명 | 상태 |
|--------|------|------|
| SceneLoadedEvent | 씬 로드 완료 | ⬜ |
| ErrorEvent | 에러 발생 | ⬜ |

### InGame

| 이벤트 | 설명 | 상태 |
|--------|------|------|
| BattleStartEvent | 전투 시작 | ⬜ |
| BattleEndEvent | 전투 종료 | ⬜ |
| TurnStartEvent | 턴 시작 | ⬜ |
| TurnEndEvent | 턴 종료 | ⬜ |
| DamageEvent | 데미지 발생 | ⬜ |
| HealEvent | 힐 발생 | ⬜ |
| SkillUsedEvent | 스킬 사용 | ⬜ |
| BuffAppliedEvent | 버프 적용 | ⬜ |
| CharacterDeathEvent | 캐릭터 사망 | ⬜ |

### OutGame

| 이벤트 | 설명 | 상태 |
|--------|------|------|
| MenuSelectedEvent | 메뉴 선택 | ⬜ |
| GachaResultEvent | 가챠 결과 | ⬜ |
| QuestCompleteEvent | 퀘스트 완료 | ⬜ |
| RewardClaimEvent | 보상 수령 | ⬜ |
| ItemPurchasedEvent | 아이템 구매 | ⬜ |
