# Sc.Contents.Battle

## 개요
턴제 전투 시스템 (InGame)

## 참조
- Sc.Common
- Sc.Contents.Character

## 패턴
- **State**: 전투/캐릭터 상태 관리
- **Command**: 액션 실행/취소

---

## 구조

```
Contents/InGame/Battle/
├── Logic/
│   ├── BattleManager.cs
│   ├── TurnManager.cs
│   ├── States/
│   │   ├── IBattleState.cs
│   │   ├── BattleReadyState.cs
│   │   ├── PlayerTurnState.cs
│   │   ├── EnemyTurnState.cs
│   │   └── BattleEndState.cs
│   └── Commands/
│       ├── ICommand.cs
│       ├── AttackCommand.cs
│       └── SkillCommand.cs
└── UI/
    ├── BattleHUDView.cs
    ├── SkillButtonView.cs
    └── DamagePopup.cs
```

---

## 클래스 목록

### Logic

| 클래스 | 설명 | 상태 |
|--------|------|------|
| BattleManager | 전투 흐름 제어 | ⬜ |
| TurnManager | 턴 순서 관리 | ⬜ |
| IBattleState | 상태 인터페이스 | ⬜ |
| BattleReadyState | 전투 준비 상태 | ⬜ |
| PlayerTurnState | 플레이어 턴 | ⬜ |
| EnemyTurnState | 적 턴 | ⬜ |
| BattleEndState | 전투 종료 | ⬜ |
| ICommand | 커맨드 인터페이스 | ⬜ |
| AttackCommand | 기본 공격 | ⬜ |
| SkillCommand | 스킬 사용 | ⬜ |

### UI

| 클래스 | 설명 | 상태 |
|--------|------|------|
| BattleHUDView | 전투 HUD | ⬜ |
| SkillButtonView | 스킬 버튼 UI | ⬜ |
| DamagePopup | 데미지 팝업 | ⬜ |
