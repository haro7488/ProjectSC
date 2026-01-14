# Sc.Data

## 개요
순수 데이터 정의 (의존성 없음)

## 참조
- (없음)

---

## 구조

```
Data/
├── Enums/
│   ├── CharacterEnums.cs
│   ├── BattleEnums.cs
│   ├── ItemEnums.cs
│   └── CommonEnums.cs
├── Structs/
│   ├── BaseStats.cs
│   ├── BattleResult.cs
│   └── RewardData.cs
└── ScriptableObjects/
    ├── CharacterData.cs
    ├── SkillData.cs
    ├── ItemData.cs
    ├── StageData.cs
    └── GachaPoolData.cs
```

---

## 클래스 목록

### Enums

| 클래스 | 설명 | 상태 |
|--------|------|------|
| CharacterEnums | CharacterRarity, CharacterClass, ElementType | ⬜ |
| BattleEnums | BattleState, ActionType, TargetType | ⬜ |
| ItemEnums | ItemType, ItemRarity | ⬜ |
| CommonEnums | GameState, SceneType | ⬜ |

### Structs

| 클래스 | 설명 | 상태 |
|--------|------|------|
| BaseStats | HP, ATK, DEF, SPD, CritRate, CritDamage | ⬜ |
| BattleResult | 전투 결과 데이터 | ⬜ |
| RewardData | 보상 데이터 | ⬜ |

### ScriptableObjects

| 클래스 | 설명 | 상태 |
|--------|------|------|
| CharacterData | 캐릭터 정의 | ⬜ |
| SkillData | 스킬 정의 | ⬜ |
| ItemData | 아이템 정의 | ⬜ |
| StageData | 스테이지 정의 | ⬜ |
| GachaPoolData | 가챠 풀 정의 | ⬜ |
