# Sc.Core

## 개요
게임 핵심 시스템

## 참조
- Sc.Data
- Sc.Packet

---

## 구조

```
Core/
├── Base/
│   └── Singleton.cs
├── Managers/
│   ├── GameManager.cs
│   ├── EventManager.cs
│   ├── ResourceManager.cs
│   ├── SceneLoader.cs
│   ├── AudioManager.cs
│   └── SaveManager.cs
└── Systems/
    └── StateMachine.cs
```

---

## 클래스 목록

### Base

| 클래스 | 설명 | 상태 |
|--------|------|------|
| Singleton\<T\> | 제네릭 싱글톤 베이스 | ⬜ |

### Managers

| 클래스 | 설명 | 상태 |
|--------|------|------|
| GameManager | 게임 상태 관리 | ⬜ |
| EventManager | 이벤트 발행/구독 (Observer) | ⬜ |
| ResourceManager | 리소스 로드/캐싱 | ⬜ |
| SceneLoader | 씬 전환 관리 | ⬜ |
| AudioManager | BGM/SFX 관리 | ⬜ |
| SaveManager | 저장/로드 (Memento) | ⬜ |

### Systems

| 클래스 | 설명 | 상태 |
|--------|------|------|
| StateMachine\<T\> | 범용 상태 머신 | ⬜ |
