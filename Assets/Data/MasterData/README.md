# Master Data JSON 구조

Excel → Python Export → JSON 파이프라인의 중간 산출물

## 파일 구조

```
MasterData/
├── Character.json    # 캐릭터 기본 정보
├── Skill.json        # 스킬 데이터
├── Item.json         # 아이템 데이터
├── Stage.json        # 스테이지 데이터
├── GachaPool.json    # 가챠 풀 설정
└── README.md         # 이 문서
```

## JSON 공통 형식

```json
{
  "version": "1.0.0",
  "exportedAt": "2026-01-15T12:00:00Z",
  "data": [ ... ]
}
```

---

## Excel 시트 구조 (예정)

### Character 시트

| 컬럼 | 타입 | 필수 | 설명 |
|------|------|------|------|
| Id | string | ✓ | 고유 식별자 (char_XXX) |
| Name | string | ✓ | 한글명 |
| NameEn | string | ✓ | 영문명 |
| Rarity | enum | ✓ | R, SR, SSR |
| Class | enum | ✓ | Warrior, Mage, Assassin, Healer, Tank, Archer |
| Element | enum | ✓ | Fire, Water, Dark, Light, Earth, Wind |
| BaseHP | int | ✓ | 기본 체력 |
| BaseATK | int | ✓ | 기본 공격력 |
| BaseDEF | int | ✓ | 기본 방어력 |
| BaseSPD | int | ✓ | 기본 속도 |
| CritRate | float | ✓ | 치명타 확률 (0.0~1.0) |
| CritDamage | float | ✓ | 치명타 배율 (1.0+) |
| SkillIds | string[] | ✓ | 스킬 ID 목록 (콤마 구분) |
| Description | string | | 캐릭터 설명 |

### Skill 시트

| 컬럼 | 타입 | 필수 | 설명 |
|------|------|------|------|
| Id | string | ✓ | 고유 식별자 (skill_XXX) |
| Name | string | ✓ | 한글명 |
| NameEn | string | ✓ | 영문명 |
| Type | enum | ✓ | Active, Passive, Ultimate |
| TargetType | enum | ✓ | SingleEnemy, AllEnemy, SingleAlly, AllAlly, Self |
| Element | enum | ✓ | Fire, Water, Dark, Light, Earth, Wind |
| Power | int | ✓ | 스킬 위력 |
| CoolDown | int | ✓ | 쿨다운 턴 |
| ManaCost | int | ✓ | 마나 소모량 |
| Description | string | | 스킬 설명 |

### Item 시트

| 컬럼 | 타입 | 필수 | 설명 |
|------|------|------|------|
| Id | string | ✓ | 고유 식별자 (item_XXX) |
| Name | string | ✓ | 한글명 |
| NameEn | string | ✓ | 영문명 |
| Type | enum | ✓ | Weapon, Armor, Accessory, Consumable |
| Rarity | enum | ✓ | N, R, SR, SSR |
| ATKBonus | int | ✓ | 공격력 보너스 |
| DEFBonus | int | ✓ | 방어력 보너스 |
| HPBonus | int | ✓ | 체력 보너스 |
| Description | string | | 아이템 설명 |

### Stage 시트

| 컬럼 | 타입 | 필수 | 설명 |
|------|------|------|------|
| Id | string | ✓ | 고유 식별자 (stage_X_X) |
| Name | string | ✓ | 한글명 |
| NameEn | string | ✓ | 영문명 |
| Chapter | int | ✓ | 챕터 번호 |
| StageNumber | int | ✓ | 스테이지 번호 |
| Difficulty | enum | ✓ | Easy, Normal, Hard |
| StaminaCost | int | ✓ | 스태미나 소모 |
| RecommendedPower | int | ✓ | 권장 전투력 |
| EnemyIds | string[] | ✓ | 적 ID 목록 (콤마 구분) |
| RewardGold | int | ✓ | 보상 골드 |
| RewardExp | int | ✓ | 보상 경험치 |
| RewardItemIds | string[] | | 보상 아이템 ID (콤마 구분) |
| RewardItemRates | float[] | | 드롭 확률 (콤마 구분) |
| Description | string | | 스테이지 설명 |

### GachaPool 시트

| 컬럼 | 타입 | 필수 | 설명 |
|------|------|------|------|
| Id | string | ✓ | 고유 식별자 (gacha_XXX) |
| Name | string | ✓ | 한글명 |
| NameEn | string | ✓ | 영문명 |
| Type | enum | ✓ | Standard, Pickup, Free |
| CostType | enum | ✓ | Gem, Ticket, None |
| CostAmount | int | ✓ | 1회 비용 |
| CostAmount10 | int | ✓ | 10연차 비용 |
| PityCount | int | ✓ | 천장 횟수 (0=없음) |
| CharacterIds | string[] | ✓ | 풀 캐릭터 ID (콤마 구분) |
| Rates_SSR | float | ✓ | SSR 확률 |
| Rates_SR | float | ✓ | SR 확률 |
| Rates_R | float | ✓ | R 확률 |
| RateUpCharacterId | string | | 픽업 캐릭터 ID |
| RateUpBonus | float | | 픽업 보너스 확률 |
| IsActive | bool | ✓ | 활성화 여부 |
| StartDate | datetime | | 시작 일시 |
| EndDate | datetime | | 종료 일시 |
| Description | string | | 설명 |

---

## Enum 정의

### Rarity
- N (Normal)
- R (Rare)
- SR (Super Rare)
- SSR (Super Super Rare)

### CharacterClass
- Warrior
- Mage
- Assassin
- Healer
- Tank
- Archer

### Element
- Fire
- Water
- Dark
- Light
- Earth
- Wind

### SkillType
- Active
- Passive
- Ultimate

### TargetType
- SingleEnemy
- AllEnemy
- SingleAlly
- AllAlly
- Self

### ItemType
- Weapon
- Armor
- Accessory
- Consumable

### Difficulty
- Easy
- Normal
- Hard

### GachaType
- Standard
- Pickup
- Free

### CostType
- Gold
- Gem
- Ticket
- None

---

## 데이터 검증 규칙

1. **ID 형식**: `{타입}_{번호}` 형식 준수
2. **참조 무결성**: SkillIds, EnemyIds 등은 실제 존재하는 ID 참조
3. **확률 합계**: Rates (SSR + SR + R) = 1.0
4. **필수 필드**: 필수 컬럼은 빈 값 불가
5. **Enum 값**: 정의된 Enum 값만 허용

## 사용법

```bash
# Python 스크립트로 Excel → JSON 변환 (예정)
python Tools/MasterData/export_master_data.py

# Unity에서 AssetPostprocessor가 JSON 변경 감지 → SO 자동 생성
```
