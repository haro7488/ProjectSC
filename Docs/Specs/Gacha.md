# Sc.Contents.Gacha

## 개요
캐릭터 뽑기 시스템 (OutGame)

## 참조
- Sc.Common
- Sc.Contents.Character

## 패턴
- **Strategy**: 확률 계산 알고리즘 교체

---

## 구조

```
Contents/OutGame/Gacha/
├── Logic/
│   ├── GachaManager.cs
│   └── Strategies/
│       ├── IGachaStrategy.cs
│       ├── NormalGachaStrategy.cs
│       ├── PickupGachaStrategy.cs
│       └── PitySystem.cs
└── UI/
    ├── GachaBannerView.cs
    └── GachaResultView.cs
```

---

## 클래스 목록

### Logic

| 클래스 | 설명 | 상태 |
|--------|------|------|
| GachaManager | 가챠 실행 관리 | ⬜ |
| IGachaStrategy | 확률 전략 인터페이스 | ⬜ |
| NormalGachaStrategy | 일반 가챠 | ⬜ |
| PickupGachaStrategy | 픽업 가챠 | ⬜ |
| PitySystem | 천장 시스템 | ⬜ |

### UI

| 클래스 | 설명 | 상태 |
|--------|------|------|
| GachaBannerView | 가챠 배너 화면 | ⬜ |
| GachaResultView | 가챠 결과 화면 | ⬜ |
