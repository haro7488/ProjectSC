# Sc.Contents.Skill

## 개요
스킬 실행 및 버프/디버프 시스템 (InGame)

## 참조
- Sc.Common
- Sc.Contents.Character

## 패턴
- **Decorator**: 스탯 수정자 래핑

---

## 구조

```
Contents/InGame/Skill/
├── Logic/
│   ├── SkillExecutor.cs
│   └── Buffs/
│       ├── IStatModifier.cs
│       ├── BuffDecorator.cs
│       └── DebuffDecorator.cs
└── UI/
    └── SkillEffectView.cs
```

---

## 클래스 목록

### Logic

| 클래스 | 설명 | 상태 |
|--------|------|------|
| SkillExecutor | 스킬 실행 관리 | ⬜ |
| IStatModifier | 스탯 수정 인터페이스 | ⬜ |
| BuffDecorator | 버프 데코레이터 | ⬜ |
| DebuffDecorator | 디버프 데코레이터 | ⬜ |

### UI

| 클래스 | 설명 | 상태 |
|--------|------|------|
| SkillEffectView | 스킬 이펙트 표시 | ⬜ |
