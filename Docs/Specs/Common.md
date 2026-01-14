# Sc.Common

## 개요
모든 컨텐츠에서 사용하는 공통 모듈

## 참조
- Sc.Core
- Sc.Data
- Sc.Packet

---

## 구조

```
Common/
├── UI/
│   ├── Base/
│   │   ├── IView.cs
│   │   ├── IPresenter.cs
│   │   ├── BaseView.cs
│   │   └── BasePresenter.cs
│   ├── Components/
│   │   ├── UIButton.cs
│   │   └── UIPopup.cs
│   └── UIManager.cs
├── Pool/
│   ├── IPoolable.cs
│   ├── ObjectPool.cs
│   └── PoolManager.cs
└── Utility/
    ├── Extensions/
    │   └── CollectionExtensions.cs
    └── Helpers/
        └── MathHelper.cs
```

---

## 클래스 목록

### UI

| 클래스 | 설명 | 상태 |
|--------|------|------|
| IView | 뷰 인터페이스 | ⬜ |
| IPresenter | 프레젠터 인터페이스 | ⬜ |
| BaseView | 뷰 베이스 클래스 | ⬜ |
| BasePresenter\<T\> | 프레젠터 베이스 | ⬜ |
| UIButton | 공통 버튼 컴포넌트 | ⬜ |
| UIPopup | 공통 팝업 베이스 | ⬜ |
| UIManager | UI 스택 관리 | ⬜ |

### Pool

| 클래스 | 설명 | 상태 |
|--------|------|------|
| IPoolable | 풀링 가능 인터페이스 | ⬜ |
| ObjectPool\<T\> | 제네릭 오브젝트 풀 | ⬜ |
| PoolManager | 풀 중앙 관리 | ⬜ |

### Utility

| 클래스 | 설명 | 상태 |
|--------|------|------|
| CollectionExtensions | 컬렉션 확장 메서드 | ⬜ |
| MathHelper | 수학 유틸리티 | ⬜ |
