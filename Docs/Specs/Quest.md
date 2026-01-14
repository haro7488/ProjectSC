# Sc.Contents.Quest

## 개요
퀘스트/업적 시스템 (OutGame)

## 참조
- Sc.Common

## 패턴
- **Composite**: AND/OR 조건 조합

---

## 구조

```
Contents/OutGame/Quest/
├── Logic/
│   ├── QuestManager.cs
│   └── Conditions/
│       ├── IQuestCondition.cs
│       ├── AndCondition.cs
│       ├── OrCondition.cs
│       └── LeafConditions/
│           ├── KillCondition.cs
│           └── CollectCondition.cs
└── UI/
    ├── QuestListView.cs
    └── QuestDetailView.cs
```

---

## 클래스 목록

### Logic

| 클래스 | 설명 | 상태 |
|--------|------|------|
| QuestManager | 퀘스트 관리 | ⬜ |
| IQuestCondition | 조건 인터페이스 | ⬜ |
| AndCondition | AND 복합 조건 | ⬜ |
| OrCondition | OR 복합 조건 | ⬜ |
| KillCondition | 처치 조건 | ⬜ |
| CollectCondition | 수집 조건 | ⬜ |

### UI

| 클래스 | 설명 | 상태 |
|--------|------|------|
| QuestListView | 퀘스트 목록 | ⬜ |
| QuestDetailView | 퀘스트 상세 | ⬜ |
