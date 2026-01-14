# 시스템 스펙

## 핵심 시스템 및 디자인 패턴

| 시스템 | 패턴 | 설명 |
|--------|------|------|
| **Core** | Singleton | GameManager, ResourceManager 등 전역 매니저 |
| **Character** | Factory + Flyweight | 캐릭터 생성 및 데이터 공유 |
| **Battle** | State + Command | 전투 상태 머신 및 스킬 시스템 |
| **Gacha** | Strategy | 가챠 확률 알고리즘 |
| **Buff** | Decorator | 버프/디버프 스탯 수정 |
| **Event** | Observer | 게임 이벤트 시스템 |
| **UI** | MVP | UI와 로직 분리 |
| **Quest** | Composite | 복합 퀘스트 조건 |
| **Save** | Memento | 게임 데이터 저장/로드 |
| **Pool** | Object Pool | 오브젝트 재사용 |

---

## 시스템별 상세

### Core
- `Singleton<T>`: 제네릭 싱글톤 베이스
- `GameManager`: 게임 상태 관리, 씬 전환
- `EventManager`: 전역 이벤트 발행/구독
- `ResourceManager`: Addressables 래핑

### Character
- `CharacterData`: ScriptableObject 기반 캐릭터 정보
- `CharacterFactory`: 등급/타입별 캐릭터 인스턴스 생성
- `CharacterStats`: 공유 스탯 데이터 (Flyweight)

### Battle
- `IBattleState`: 상태 인터페이스 (Idle, Attack, Skill, Stun, Dead)
- `ICommand`: 액션 인터페이스 (Execute, Undo)
- `BattleManager`: 턴 관리, 전투 흐름 제어

### Gacha
- `IGachaStrategy`: 확률 계산 전략 인터페이스
- `NormalGachaStrategy`: 일반 가챠
- `PickupGachaStrategy`: 픽업 가챠
- `PityGachaStrategy`: 천장 시스템

### Buff
- `IStatModifier`: 스탯 수정 인터페이스
- `BuffDecorator`: 버프 래핑
- `DebuffDecorator`: 디버프 래핑

### UI (MVP)
- `IView`: 뷰 인터페이스
- `IPresenter`: 프레젠터 인터페이스
- `BasePresenter<TView>`: 프레젠터 베이스

### Quest
- `IQuestCondition`: 조건 인터페이스
- `CompositeCondition`: AND/OR 복합 조건
- `LeafCondition`: 단일 조건 (킬, 수집 등)

### Save
- `IMemento`: 상태 스냅샷 인터페이스
- `SaveManager`: 저장/로드 관리
- `GameState`: 게임 상태 데이터

### Pool
- `ObjectPool<T>`: 제네릭 오브젝트 풀
- `PoolManager`: 풀 중앙 관리